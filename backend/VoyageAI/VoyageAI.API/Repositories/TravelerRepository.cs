using Microsoft.EntityFrameworkCore;
using VoyageAI.API.Data;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;

namespace VoyageAI.API.Repositories
{
    /// <summary>
    /// Repository implementation for Traveler entity data access operations.
    /// 
    /// This concrete implementation of ITravelerRepository uses Entity Framework Core
    /// to perform all database operations related to the Traveler entity.
    /// 
    /// Design Pattern: Repository Pattern
    /// - Encapsulates all EF Core logic
    /// - Service layer only knows about ITravelerRepository interface
    /// - Enables unit testing through interface dependency
    /// 
    /// Dependency Injection:
    /// Injected as: services.AddScoped<ITravelerRepository, TravelerRepository>()
    /// 
    /// Usage:
    /// Consumed by TravelerService for all traveler-related data access.
    /// 
    /// Entity Framework Core Details:
    /// - Uses VoyageDbContext for database context
    /// - Queries are executed asynchronously for better scalability
    /// - CancellationTokens are properly propagated to database operations
    /// - AsNoTracking() is used where entities are not modified (read-only queries)
    /// - Hard delete pattern: Rows are physically removed from the database
    /// 
    /// Performance Considerations:
    /// - Indexes on TripId for efficient filtering
    /// - Index on PassportNumber for quick lookup
    /// - Index on Email for duplicate detection
    /// </summary>
    public class TravelerRepository : ITravelerRepository
    {
        /// <summary>
        /// The Entity Framework Core database context.
        /// Provides access to the Travelers table and manages entity tracking.
        /// </summary>
        private readonly VoyageDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the TravelerRepository class.
        /// 
        /// Constructor Injection:
        /// VoyageDbContext is injected by the dependency injection container.
        /// This allows the repository to perform database operations and supports testability.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped<ITravelerRepository, TravelerRepository>();
        /// </summary>
        /// <param name="dbContext">The Entity Framework Core database context</param>
        public TravelerRepository(VoyageDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Creates a new traveler in the database.
        /// The traveler is added to the DbContext but not saved until SaveChangesAsync is called.
        /// </summary>
        public async Task CreateAsync(Traveler traveler, CancellationToken cancellationToken = default)
        {
            if (traveler == null)
                throw new ArgumentNullException(nameof(traveler));

            await _dbContext.Travelers.AddAsync(traveler, cancellationToken);
        }

        /// <summary>
        /// Retrieves a traveler by its ID.
        /// Includes related Trip entity for complete traveler information.
        /// </summary>
        public async Task<Traveler?> GetByIdAsync(Guid travelerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Travelers
                .Include(t => t.Trip)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TravelerId == travelerId, cancellationToken);
        }

        /// <summary>
        /// Retrieves all travelers for a specific trip.
        /// Ordered by primary traveler first, then by creation date.
        /// Includes related Trip entity.
        /// </summary>
        public async Task<List<Traveler>> GetTravelersByTripAsync(Guid tripId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Travelers
                .Include(t => t.Trip)
                .AsNoTracking()
                .Where(t => t.TripId == tripId)
                .OrderByDescending(t => t.IsPrimaryTraveler)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Checks if a traveler with the given email already exists in a trip.
        /// Optionally excludes a specific traveler ID from the check (useful for updates).
        /// </summary>
        public async Task<bool> TravelerExistsByEmailAsync(Guid tripId, string email, Guid? excludeTravelerId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var query = _dbContext.Travelers
                .AsNoTracking()
                .Where(t => t.TripId == tripId && t.Email == email);

            if (excludeTravelerId.HasValue)
            {
                query = query.Where(t => t.TravelerId != excludeTravelerId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing traveler in the database.
        /// The traveler must already be tracked by the DbContext.
        /// The UpdatedAt and LastModifiedBy fields should have been set by the service layer.
        /// </summary>
        public async Task UpdateAsync(Traveler traveler, CancellationToken cancellationToken = default)
        {
            if (traveler == null)
                throw new ArgumentNullException(nameof(traveler));

            _dbContext.Travelers.Update(traveler);
            await Task.CompletedTask;  // Explicit async for future extensibility
        }

        /// <summary>
        /// Hard deletes a traveler from the database.
        /// Permanently removes the record.
        /// </summary>
        public async Task DeleteAsync(Traveler traveler, CancellationToken cancellationToken = default)
        {
            if (traveler == null)
                throw new ArgumentNullException(nameof(traveler));

            _dbContext.Travelers.Remove(traveler);
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
