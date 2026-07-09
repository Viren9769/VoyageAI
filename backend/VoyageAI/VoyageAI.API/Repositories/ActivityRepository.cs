using Microsoft.EntityFrameworkCore;
using VoyageAI.API.Data;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;

namespace VoyageAI.API.Repositories
{
    /// <summary>
    /// Repository implementation for Activity entity data access operations.
    /// 
    /// This concrete implementation of IActivityRepository uses Entity Framework Core
    /// to perform all database operations related to the Activity entity.
    /// 
    /// Design Pattern: Repository Pattern
    /// - Encapsulates all EF Core logic
    /// - Service layer only knows about IActivityRepository interface
    /// - Enables unit testing through interface dependency
    /// - No business logic: only data access operations
    /// 
    /// Dependency Injection:
    /// Injected as: services.AddScoped<IActivityRepository, ActivityRepository>()
    /// 
    /// Usage:
    /// Consumed by ActivityService and other services that need activity data access.
    /// 
    /// Entity Framework Core Details:
    /// - Uses VoyageDbContext for database context
    /// - Queries are executed asynchronously for better scalability
    /// - CancellationTokens are properly propagated to database operations
    /// - Uses AsNoTracking() for read-only queries to improve performance
    /// - Implements hard delete pattern (physical removal from database)
    /// - Transactions are handled by service layer
    /// 
    /// Hard Delete:
    /// - Activities are physically removed from the database
    /// - Cannot be recovered without database backups
    /// - More complete removal than soft delete
    /// - Used for permanent deletion at user request
    /// 
    /// Database Indexes:
    /// - (DayId)
    /// - (Category)
    /// - (Status)
    /// - (Priority)
    /// - (StartTime)
    /// - (DayId, IsDeleted) - for soft-deleted compatibility
    /// </summary>
    public class ActivityRepository : IActivityRepository
    {
        /// <summary>
        /// The Entity Framework Core database context.
        /// Provides access to the Activities table and manages entity tracking.
        /// </summary>
        private readonly VoyageDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ActivityRepository class.
        /// 
        /// Constructor Injection:
        /// VoyageDbContext is injected by the dependency injection container.
        /// This allows the repository to perform database operations and supports testability.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped<IActivityRepository, ActivityRepository>();
        /// </summary>
        /// <param name="dbContext">The Entity Framework Core database context</param>
        public ActivityRepository(VoyageDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Creates a new activity in the database.
        /// The activity is added to the DbContext but not saved until SaveChangesAsync is called.
        /// </summary>
        public async Task CreateAsync(Activity activity, CancellationToken cancellationToken = default)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            await _dbContext.Activities.AddAsync(activity, cancellationToken);
        }

        /// <summary>
        /// Retrieves an activity by its ID.
        /// Includes related ItineraryDay entity for verifying ownership and getting day details.
        /// Uses AsNoTracking() for read-only queries to avoid entity tracking conflicts.
        /// Note: Does NOT filter by IsDeleted (returns soft-deleted activities too).
        /// </summary>
        public async Task<Activity?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .Include(a => a.ItineraryDay)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ActivityId == activityId, cancellationToken);
        }

        /// <summary>
        /// Retrieves all activities for a specific itinerary day (excluding soft-deleted).
        /// Orders results by StartTime for chronological order.
        /// Uses AsNoTracking() for read-only queries to improve performance.
        /// Index usage: (DayId, IsDeleted) for efficient querying.
        /// </summary>
        public async Task<List<Activity>> GetByDayIdAsync(Guid dayId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .Where(a => a.DayId == dayId && !a.IsDeleted)
                .OrderBy(a => a.StartTime)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves all activities for a specific itinerary day filtered by status (excluding soft-deleted).
        /// Orders results by StartTime for chronological order.
        /// Uses AsNoTracking() for read-only queries to improve performance.
        /// Index usage: (DayId, Status, IsDeleted).
        /// </summary>
        public async Task<List<Activity>> GetByDayIdAndStatusAsync(Guid dayId, int status, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .Where(a => a.DayId == dayId && a.Status == status && !a.IsDeleted)
                .OrderBy(a => a.StartTime)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves activities for a specific itinerary day filtered by category (excluding soft-deleted).
        /// Orders results by StartTime for chronological order.
        /// Uses AsNoTracking() for read-only queries to improve performance.
        /// Index usage: (DayId, Category, IsDeleted).
        /// </summary>
        public async Task<List<Activity>> GetByDayIdAndCategoryAsync(Guid dayId, int category, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .Where(a => a.DayId == dayId && a.Category == category && !a.IsDeleted)
                .OrderBy(a => a.StartTime)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves all activities for a trip across all days (excluding soft-deleted).
        /// Joins with ItineraryDays to filter by trip.
        /// Orders by day date then activity start time.
        /// Uses AsNoTracking() for read-only queries to improve performance.
        /// </summary>
        public async Task<List<Activity>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .Include(a => a.ItineraryDay)
                .Where(a => a.ItineraryDay.TripId == tripId && !a.IsDeleted)
                .OrderBy(a => a.ItineraryDay.Date)
                .ThenBy(a => a.StartTime)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Checks if an activity with specific name already exists in a day (excluding soft-deleted).
        /// Excludes the specified activity ID if provided (useful for update validation).
        /// Uses AnyAsync for efficiency.
        /// </summary>
        public async Task<bool> ActivityNameExistsAsync(Guid dayId, string activityName, Guid? excludeActivityId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(activityName))
                return false;

            var query = _dbContext.Activities
                .Where(a => a.DayId == dayId && a.ActivityName == activityName && !a.IsDeleted);

            if (excludeActivityId.HasValue)
            {
                query = query.Where(a => a.ActivityId != excludeActivityId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves activities for a specific day within a time range (excluding soft-deleted).
        /// Used to detect schedule conflicts or overlaps.
        /// Excludes the specified activity ID if provided (useful for update validation).
        /// Uses overlap logic: activity overlaps if it starts before requested end and ends after requested start.
        /// </summary>
        public async Task<List<Activity>> GetActivitiesInTimeRangeAsync(
            Guid dayId,
            TimeOnly startTime,
            TimeOnly endTime,
            Guid? excludeActivityId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Activities
                .Where(a => a.DayId == dayId &&
                    a.StartTime < endTime &&
                    a.EndTime > startTime &&
                    !a.IsDeleted);

            if (excludeActivityId.HasValue)
            {
                query = query.Where(a => a.ActivityId != excludeActivityId.Value);
            }

            return await query
                .OrderBy(a => a.StartTime)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing activity.
        /// The activity is updated in the DbContext but not saved until SaveChangesAsync is called.
        /// Service layer is responsible for setting audit fields (UpdatedAt, LastModifiedBy).
        /// </summary>
        public async Task UpdateAsync(Activity activity, CancellationToken cancellationToken = default)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity));

            // EF Core Update method marks entity for update
            // Changes will be persisted when SaveChangesAsync() is called
            _dbContext.Activities.Update(activity);
            await Task.CompletedTask; // Async method signature for consistency
        }

        /// <summary>
        /// Performs a hard delete on an activity.
        /// Permanently removes the activity from the database.
        /// The activity is marked for deletion in the DbContext but not saved until SaveChangesAsync is called.
        /// Throws InvalidOperationException if the activity doesn't exist.
        /// </summary>
        public async Task DeleteAsync(Guid activityId, DateTime deletedAt, CancellationToken cancellationToken = default)
        {
            // First, verify the entity exists by fetching it
            var activity = await _dbContext.Activities
                .FirstOrDefaultAsync(a => a.ActivityId == activityId, cancellationToken);

            if (activity == null)
            {
                throw new InvalidOperationException($"Activity with ID {activityId} not found");
            }

            // Perform hard delete - permanently remove from database
            _dbContext.Activities.Remove(activity);
        }

        /// <summary>
        /// Saves all changes made to the context to the database.
        /// Persists all pending operations (create, update, delete) in a single transaction.
        /// Returns the number of entities changed.
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
