using Microsoft.EntityFrameworkCore;
using VoyageAI.API.Data;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;

namespace VoyageAI.API.Repositories
{
    /// <summary>
    /// Repository implementation for Trip entity data access operations.
    /// 
    /// This concrete implementation of ITripRepository uses Entity Framework Core
    /// to perform all database operations related to the Trip entity.
    /// 
    /// Design Pattern: Repository Pattern
    /// - Encapsulates all EF Core logic
    /// - Service layer only knows about ITripRepository interface
    /// - Enables unit testing through interface dependency
    /// 
    /// Dependency Injection:
    /// Injected as: services.AddScoped{ITripRepository, TripRepository}()
    /// 
    /// Usage:
    /// Consumed by TripService and other services that need trip data access.
    /// 
    /// Entity Framework Core Details:
    /// - Uses VoyageDbContext for database context
    /// - Queries are executed asynchronously for better scalability
    /// - CancellationTokens are properly propagated to database operations
    /// - Relationships are eagerly loaded where appropriate
    /// </summary>
    public class TripRepository : ITripRepository
    {
        /// <summary>
        /// The Entity Framework Core database context.
        /// Provides access to the Trips table and manages entity tracking.
        /// </summary>
        private readonly VoyageDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the TripRepository class.
        /// 
        /// Constructor Injection:
        /// VoyageDbContext is injected by the dependency injection container.
        /// This allows the repository to perform database operations and supports testability.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped{ITripRepository, TripRepository}();
        /// </summary>
        /// <param name="dbContext">The Entity Framework Core database context</param>
        public TripRepository(VoyageDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Creates a new trip in the database.
        /// The trip is added to the DbContext but not saved until SaveChangesAsync is called.
        /// </summary>
        public async Task CreateAsync(Trip trip, CancellationToken cancellationToken = default)
        {
            if (trip == null)
                throw new ArgumentNullException(nameof(trip));

            await _dbContext.Trips.AddAsync(trip, cancellationToken);
        }

        /// <summary>
        /// Retrieves a trip by its ID.
        /// Includes related User entity for complete trip information.
        /// </summary>
        public async Task<Trip?> GetByIdAsync(Guid tripId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Trips
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TripId == tripId, cancellationToken);
        }

        /// <summary>
        /// Retrieves all trips for a specific user.
        /// Ordered by creation date (newest first) for better UX.
        /// Includes related User entity.
        /// </summary>
        public async Task<List<Trip>> GetUserTripsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Trips
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing trip in the database.
        /// The trip must already be tracked by the DbContext.
        /// The UpdatedAt timestamp should have been set by the service layer.
        /// </summary>
        public async Task UpdateAsync(Trip trip, CancellationToken cancellationToken = default)
        {
            if (trip == null)
                throw new ArgumentNullException(nameof(trip));

            _dbContext.Trips.Update(trip);
            await Task.CompletedTask;  // Explicit async for future extensibility
        }

        /// <summary>
        /// Deletes a trip from the database.
        /// CASCADE delete behavior ensures related Travelers and ItineraryDays are also deleted.
        /// </summary>
        public async Task DeleteAsync(Trip trip, CancellationToken cancellationToken = default)
        {
            if (trip == null)
                throw new ArgumentNullException(nameof(trip));

            _dbContext.Trips.Remove(trip);
            await Task.CompletedTask;  // Explicit async for future extensibility
        }

        /// <summary>
        /// Saves all pending changes to the database in a single transaction.
        /// This implements the Unit of Work pattern.
        /// </summary>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
