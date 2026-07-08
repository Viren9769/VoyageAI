using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Trip entity data access operations.
    /// 
    /// This interface abstracts all database operations related to the Trip entity.
    /// The concrete implementation (TripRepository) uses Entity Framework Core,
    /// but the service layer depends only on this interface.
    /// 
    /// Design Pattern: Repository Pattern
    /// - Abstraction of data access logic
    /// - Testability (can mock in unit tests)
    /// - Decoupling from EF Core implementation
    /// - Single Responsibility Principle
    /// - Dependency Inversion (depend on abstractions, not concrete classes)
    /// 
    /// Usage in Services:
    /// The TripService injects this interface and calls its methods without
    /// knowing or caring about the underlying database technology (EF Core, SQL, etc.).
    /// </summary>
    public interface ITripRepository
    {
        /// <summary>
        /// Creates a new trip in the database.
        /// 
        /// Purpose:
        /// - Insert a newly created trip into the database
        /// - Used when a user creates a new trip
        /// 
        /// Parameters:
        /// - trip: The Trip entity to create
        /// - cancellationToken: Allows cancellation of the database operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Database Operation:
        /// INSERT INTO "Trips" (TripId, UserId, TripName, ...) VALUES (...)
        /// 
        /// Example:
        /// var trip = new Trip { TripName = "Europe", UserId = userId };
        /// await _tripRepository.CreateAsync(trip, cancellationToken);
        /// await _tripRepository.SaveChangesAsync(cancellationToken);
        /// </summary>
        Task CreateAsync(Trip trip, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a trip by its ID.
        /// 
        /// Purpose:
        /// - Get a specific trip when the user requests it or modifies it
        /// - Validate that the trip exists before updating/deleting
        /// 
        /// Parameters:
        /// - tripId: The unique identifier of the trip
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - Trip object if found
        /// - null if trip doesn't exist
        /// 
        /// Database Query:
        /// SELECT * FROM "Trips" WHERE "TripId" = @tripId
        /// 
        /// Example:
        /// var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
        /// if (trip == null) throw new EntityNotFoundException("Trip not found");
        /// </summary>
        Task<Trip?> GetByIdAsync(Guid tripId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all trips for a specific user.
        /// 
        /// Purpose:
        /// - Get all trips created by a user
        /// - Display user's trips in the dashboard
        /// - Ensure users can only see their own trips
        /// 
        /// Parameters:
        /// - userId: The ID of the user who owns the trips
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - List of Trip objects for the user
        /// - Empty list if user has no trips
        /// 
        /// Database Query:
        /// SELECT * FROM "Trips" WHERE "UserId" = @userId ORDER BY "CreatedAt" DESC
        /// 
        /// Example:
        /// var trips = await _tripRepository.GetUserTripsAsync(userId, cancellationToken);
        /// return trips.Select(t => _mapper.Map{GetTripResponse}(t)).ToList();
        /// </summary>
        Task<List<Trip>> GetUserTripsAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing trip in the database.
        /// 
        /// Purpose:
        /// - Persist changes to a trip object
        /// - Used when user modifies trip details
        /// 
        /// Parameters:
        /// - trip: The Trip entity with updated values
        /// - cancellationToken: Allows cancellation of the database operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Database Operation:
        /// UPDATE "Trips" SET ... WHERE "TripId" = @tripId
        /// 
        /// Note:
        /// - The entity must already be tracked by the DbContext (attached)
        /// - Typically used after modifying properties of a trip fetched via GetByIdAsync
        /// - UpdatedAt timestamp should be updated by the service layer
        /// 
        /// Example:
        /// var trip = await _tripRepository.GetByIdAsync(tripId, ct);
        /// trip.TripName = "New Name";
        /// trip.UpdatedAt = DateTime.UtcNow;
        /// await _tripRepository.UpdateAsync(trip, ct);
        /// await _tripRepository.SaveChangesAsync(ct);
        /// </summary>
        Task UpdateAsync(Trip trip, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a trip from the database.
        /// 
        /// Purpose:
        /// - Remove a trip that the user no longer wants
        /// - Handles cleanup of related data (travelers, itinerary days, activities)
        /// 
        /// Parameters:
        /// - trip: The Trip entity to delete
        /// - cancellationToken: Allows cancellation of the database operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Database Operation:
        /// DELETE FROM "Trips" WHERE "TripId" = @tripId
        /// 
        /// Cascade Behavior:
        /// - Related Travelers are deleted (OnDelete = Cascade)
        /// - Related ItineraryDays are deleted (OnDelete = Cascade)
        /// - Related Activities are deleted (OnDelete = Cascade)
        /// 
        /// Note:
        /// - Ensure authorization checks are in place before calling this method
        /// - Only the trip owner should be able to delete it
        /// 
        /// Example:
        /// var trip = await _tripRepository.GetByIdAsync(tripId, ct);
        /// if (trip.UserId != userId) throw new ForbiddenException("Not trip owner");
        /// await _tripRepository.DeleteAsync(trip, ct);
        /// await _tripRepository.SaveChangesAsync(ct);
        /// </summary>
        Task DeleteAsync(Trip trip, CancellationToken cancellationToken = default);

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
        /// await _tripRepository.CreateAsync(trip1, ct);
        /// await _tripRepository.CreateAsync(trip2, ct);
        /// // Then save all changes in one operation
        /// await _tripRepository.SaveChangesAsync(ct);
        /// 
        /// Error Handling:
        /// - DbUpdateException if constraint violations occur
        /// - OperationCanceledException if cancellation is requested
        /// 
        /// Example:
        /// var trip = new Trip { TripName = "Adventure", UserId = userId };
        /// await _tripRepository.CreateAsync(trip, cancellationToken);
        /// await _tripRepository.SaveChangesAsync(cancellationToken);  // Actually saves to DB
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
