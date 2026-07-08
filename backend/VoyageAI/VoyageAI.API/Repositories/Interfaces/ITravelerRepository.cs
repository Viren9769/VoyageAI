using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Traveler entity data access operations.
    /// 
    /// This interface abstracts all database operations related to the Traveler entity.
    /// The concrete implementation (TravelerRepository) uses Entity Framework Core,
    /// but the service layer depends only on this interface.
    /// 
    /// Design Pattern: Repository Pattern
    /// - Abstraction of data access logic
    /// - Testability (can mock in unit tests)
    /// - Decoupling from EF Core implementation
    /// - Single Responsibility Principle
    /// - Dependency Inversion (depend on abstractions, not concrete classes)
    /// 
    /// Critical Security Note:
    /// The service layer (not repository) is responsible for verifying:
    /// 1. Authenticated user exists
    /// 2. Trip exists
    /// 3. Trip belongs to authenticated user
    /// 4. Traveler belongs to that Trip
    /// 
    /// The repository performs data operations only.
    /// </summary>
    public interface ITravelerRepository
    {
        /// <summary>
        /// Creates a new traveler in the database.
        /// 
        /// Purpose:
        /// - Insert a newly created traveler for a trip
        /// - Called when a user adds a traveler to their trip
        /// 
        /// Parameters:
        /// - traveler: The Traveler entity to create
        /// - cancellationToken: Allows cancellation of the database operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Database Operation:
        /// INSERT INTO "Travelers" (TravelerId, TripId, FirstName, LastName, ...) VALUES (...)
        /// 
        /// Example:
        /// var traveler = new Traveler { TravelerId = Guid.NewGuid(), TripId = tripId, FirstName = "John" };
        /// await _travelerRepository.CreateAsync(traveler, cancellationToken);
        /// await _travelerRepository.SaveChangesAsync(cancellationToken);
        /// </summary>
        Task CreateAsync(Traveler traveler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a traveler by its ID.
        /// 
        /// Purpose:
        /// - Get a specific traveler when the user requests or modifies it
        /// - Validate that the traveler exists before updating/deleting
        /// 
        /// Parameters:
        /// - travelerId: The unique identifier of the traveler
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - Traveler object if found
        /// - null if traveler doesn't exist or is deleted (soft delete)
        /// 
        /// Database Query:
        /// SELECT * FROM "Travelers" WHERE "TravelerId" = @travelerId AND "IsDeleted" = false
        /// 
        /// Note:
        /// Query automatically excludes deleted travelers (IsDeleted = true).
        /// 
        /// Example:
        /// var traveler = await _travelerRepository.GetByIdAsync(travelerId, cancellationToken);
        /// if (traveler == null) throw new EntityNotFoundException("Traveler not found");
        /// </summary>
        Task<Traveler?> GetByIdAsync(Guid travelerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all travelers for a specific trip.
        /// 
        /// Purpose:
        /// - Get all travelers added to a trip
        /// - Display travelers in the trip details view
        /// - Ensure travelers are only returned for the trip they belong to
        /// 
        /// Parameters:
        /// - tripId: The ID of the trip
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - List of Traveler objects for the trip
        /// - Empty list if trip has no travelers
        /// - Excludes deleted travelers automatically
        /// 
        /// Database Query:
        /// SELECT * FROM "Travelers" 
        /// WHERE "TripId" = @tripId AND "IsDeleted" = false 
        /// ORDER BY "IsPrimaryTraveler" DESC, "CreatedAt" ASC
        /// 
        /// Note:
        /// - Primary travelers are listed first
        /// - Results ordered by creation date
        /// - Deleted travelers are automatically excluded
        /// 
        /// Example:
        /// var travelers = await _travelerRepository.GetTravelersByTripAsync(tripId, cancellationToken);
        /// return travelers.Select(t => _mapper.Map<TravelerResponse>(t)).ToList();
        /// </summary>
        Task<List<Traveler>> GetTravelersByTripAsync(Guid tripId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a traveler with the given email already exists in a trip.
        /// 
        /// Purpose:
        /// - Prevent duplicate travelers (same person added twice to same trip)
        /// - Validation before creating a new traveler
        /// 
        /// Parameters:
        /// - tripId: The ID of the trip
        /// - email: The email address to check
        /// - excludeTravelerId: (Optional) Exclude a specific traveler ID from check (for updates)
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - true if a traveler with this email exists in the trip
        /// - false otherwise
        /// 
        /// Database Query:
        /// SELECT COUNT(*) FROM "Travelers" 
        /// WHERE "TripId" = @tripId AND "Email" = @email AND "IsDeleted" = false
        /// 
        /// Example:
        /// bool exists = await _travelerRepository.TravelerExistsByEmailAsync(tripId, "john@example.com", null, ct);
        /// if (exists) throw new ValidationException("Traveler with this email already exists");
        /// </summary>
        Task<bool> TravelerExistsByEmailAsync(Guid tripId, string email, Guid? excludeTravelerId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing traveler in the database.
        /// 
        /// Purpose:
        /// - Persist changes to a traveler object
        /// - Used when user modifies traveler details
        /// 
        /// Parameters:
        /// - traveler: The Traveler entity with updated values
        /// - cancellationToken: Allows cancellation of the database operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Database Operation:
        /// UPDATE "Travelers" SET ... WHERE "TravelerId" = @travelerId
        /// 
        /// Note:
        /// - The entity must already be tracked by the DbContext (attached)
        /// - Typically used after modifying properties of a traveler fetched via GetByIdAsync
        /// - UpdatedAt and LastModifiedBy should be updated by the service layer
        /// 
        /// Example:
        /// var traveler = await _travelerRepository.GetByIdAsync(travelerId, ct);
        /// traveler.FirstName = "Jane";
        /// traveler.UpdatedAt = DateTime.UtcNow;
        /// traveler.LastModifiedBy = userId;
        /// await _travelerRepository.UpdateAsync(traveler, ct);
        /// await _travelerRepository.SaveChangesAsync(ct);
        /// </summary>
        Task UpdateAsync(Traveler traveler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft deletes a traveler from the database.
        /// 
        /// Purpose:
        /// - Mark a traveler as deleted without physically removing the record
        /// - Maintains data integrity and audit trail
        /// - Allows recovery if needed
        /// 
        /// Parameters:
        /// - traveler: The Traveler entity to soft delete
        /// - cancellationToken: Allows cancellation of the database operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Database Operation:
        /// UPDATE "Travelers" SET "IsDeleted" = true, "DeletedAt" = @utcNow WHERE "TravelerId" = @travelerId
        /// 
        /// Soft Delete Pattern:
        /// - IsDeleted flag is set to true
        /// - DeletedAt timestamp records when deletion occurred
        /// - Record remains in database but is excluded from queries
        /// - All future queries automatically exclude deleted travelers
        /// 
        /// Notes:
        /// - This is NOT a physical delete
        /// - Deleted travelers can be restored by setting IsDeleted = false
        /// - All queries must filter IsDeleted = false
        /// 
        /// Example:
        /// var traveler = await _travelerRepository.GetByIdAsync(travelerId, ct);
        /// traveler.IsDeleted = true;
        /// traveler.DeletedAt = DateTime.UtcNow;
        /// traveler.LastModifiedBy = userId;
        /// await _travelerRepository.UpdateAsync(traveler, ct);
        /// await _travelerRepository.SaveChangesAsync(ct);
        /// </summary>
        Task DeleteAsync(Traveler traveler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves all pending changes to the database.
        /// 
        /// Purpose:
        /// - Persist all changes made to entities tracked by the DbContext
        /// - This is called as a separate operation (Unit of Work pattern)
        /// 
        /// Parameters:
        /// - cancellationToken: Allows cancellation of the save operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Database Operation:
        /// Executes a transaction to save all pending INSERT, UPDATE, DELETE operations
        /// 
        /// Usage Pattern:
        /// // Create or modify entities
        /// await _travelerRepository.CreateAsync(traveler1, ct);
        /// await _travelerRepository.CreateAsync(traveler2, ct);
        /// // Then save all changes in one operation
        /// await _travelerRepository.SaveChangesAsync(ct);
        /// 
        /// Error Handling:
        /// - DbUpdateException if constraint violations occur
        /// - OperationCanceledException if cancellation is requested
        /// 
        /// Example:
        /// var traveler = new Traveler { TravelerId = Guid.NewGuid(), TripId = tripId, FirstName = "John" };
        /// await _travelerRepository.CreateAsync(traveler, cancellationToken);
        /// await _travelerRepository.SaveChangesAsync(cancellationToken);  // Actually saves to DB
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
