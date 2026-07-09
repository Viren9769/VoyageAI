using Microsoft.EntityFrameworkCore;
using VoyageAI.API.Data;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;

namespace VoyageAI.API.Repositories
{
    /// <summary>
    /// Repository implementation for ItineraryDay entity data access operations.
    /// 
    /// This concrete implementation of IItineraryRepository uses Entity Framework Core
    /// to perform all database operations related to the ItineraryDay entity.
    /// 
    /// Design Pattern: Repository Pattern
    /// - Encapsulates all EF Core logic
    /// - Service layer only knows about IItineraryRepository interface
    /// - Enables unit testing through interface dependency
    /// - No business logic: only data access operations
    /// 
    /// Dependency Injection:
    /// Injected as: services.AddScoped{IItineraryRepository, ItineraryRepository}()
    /// 
    /// Usage:
    /// Consumed by ItineraryService and other services that need itinerary day data access.
    /// 
    /// Entity Framework Core Details:
    /// - Uses VoyageDbContext for database context
    /// - Queries are executed asynchronously for better scalability
    /// - CancellationTokens are properly propagated to database operations
    /// - Uses AsNoTracking() for read-only queries to improve performance
    /// - Transactions are handled by service layer
    /// </summary>
    public class ItineraryRepository : IItineraryRepository
    {
        /// <summary>
        /// The Entity Framework Core database context.
        /// Provides access to the ItineraryDays table and manages entity tracking.
        /// </summary>
        private readonly VoyageDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the ItineraryRepository class.
        /// 
        /// Constructor Injection:
        /// VoyageDbContext is injected by the dependency injection container.
        /// This allows the repository to perform database operations and supports testability.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped{IItineraryRepository, ItineraryRepository}();
        /// </summary>
        /// <param name="dbContext">The Entity Framework Core database context</param>
        public ItineraryRepository(VoyageDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Creates a new itinerary day in the database.
        /// The day is added to the DbContext but not saved until SaveChangesAsync is called.
        /// </summary>
        public async Task CreateAsync(ItineraryDay itineraryDay, CancellationToken cancellationToken = default)
        {
            if (itineraryDay == null)
                throw new ArgumentNullException(nameof(itineraryDay));

            await _dbContext.ItineraryDays.AddAsync(itineraryDay, cancellationToken);
        }

        /// <summary>
        /// Retrieves an itinerary day by its ID.
        /// Includes related Trip entity for verifying ownership.
        /// Uses AsNoTracking() for read-only queries to avoid entity tracking conflicts.
        /// Note: Does NOT filter by IsDeleted (returns soft-deleted days too).
        /// </summary>
        public async Task<ItineraryDay?> GetByIdAsync(Guid dayId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ItineraryDays
                .Include(d => d.Trip)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DayId == dayId, cancellationToken);
        }

        /// <summary>
        /// Retrieves all itinerary days for a specific trip.
        /// Orders results by DayNumber for chronological order.
        /// Uses AsNoTracking() for read-only queries to improve performance.
        /// Index usage: (TripId) for efficient querying.
        /// </summary>
        public async Task<List<ItineraryDay>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ItineraryDays
                .Where(d => d.TripId == tripId)
                .OrderBy(d => d.DayNumber)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Checks if an itinerary day number already exists in a trip.
        /// Excludes the specified day ID if provided (useful for update validation).
        /// Uses AnyAsync for efficiency.
        /// </summary>
        public async Task<bool> DayNumberExistsAsync(Guid tripId, int dayNumber, Guid? excludeDayId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ItineraryDays
                .Where(d => d.TripId == tripId && d.DayNumber == dayNumber);

            if (excludeDayId.HasValue)
            {
                query = query.Where(d => d.DayId != excludeDayId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing itinerary day.
        /// The day is updated in the DbContext but not saved until SaveChangesAsync is called.
        /// Service layer is responsible for setting audit fields (UpdatedAt, LastModifiedBy).
        /// </summary>
        public async Task UpdateAsync(ItineraryDay itineraryDay, CancellationToken cancellationToken = default)
        {
            if (itineraryDay == null)
                throw new ArgumentNullException(nameof(itineraryDay));

            // EF Core Update method marks entity for update
            // Changes will be persisted when SaveChangesAsync() is called
            _dbContext.ItineraryDays.Update(itineraryDay);
            await Task.CompletedTask; // Async method signature for consistency
        }

        /// <summary>
        /// Performs a hard delete on an itinerary day.
        /// Permanently removes the record from the database.
        /// Throws EntityNotFoundException if the day doesn't exist.
        /// </summary>
        public async Task DeleteAsync(Guid dayId, DateTime deletedAt, CancellationToken cancellationToken = default)
        {
            // First, verify the entity exists by fetching it
            var itineraryDay = await _dbContext.ItineraryDays
                .FirstOrDefaultAsync(d => d.DayId == dayId, cancellationToken);

            if (itineraryDay == null)
            {
                throw new InvalidOperationException($"Itinerary day with ID {dayId} not found");
            }

            // Perform hard delete - permanently remove from database
            _dbContext.ItineraryDays.Remove(itineraryDay);
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
