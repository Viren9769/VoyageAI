using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for ItineraryDay entity data access operations.
    /// 
    /// This interface abstracts all database operations related to the ItineraryDay entity.
    /// The concrete implementation (ItineraryRepository) uses Entity Framework Core,
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
    /// The ItineraryService injects this interface and calls its methods without
    /// knowing or caring about the underlying database technology (EF Core, SQL, etc.).
    /// 
    /// Async Operations:
    /// All methods are async for scalability and responsiveness.
    /// Network I/O (database queries) should never block threads.
    /// 
    /// CancellationToken:
    /// All methods accept optional CancellationToken for graceful cancellation.
    /// This allows long-running operations to be cancelled by the client.
    /// </summary>
    public interface IItineraryRepository
    {
        /// <summary>
        /// Creates a new itinerary day in the database.
        /// 
        /// Purpose:
        /// - Insert a newly created itinerary day into the database
        /// - Used when a user creates a new day in their trip's itinerary
        /// 
        /// Parameters:
        /// - itineraryDay: The ItineraryDay entity to create
        /// - cancellationToken: Allows cancellation of the database operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Database Operation:
        /// INSERT INTO "ItineraryDays" 
        /// (DayId, TripId, DayNumber, Date, Title, CreatedAt, UpdatedAt, CreatedBy, LastModifiedBy, IsDeleted) 
        /// VALUES (...)
        /// 
        /// Behavior:
        /// - Entity is added to the DbContext but not saved
        /// - SaveChangesAsync() must be called separately to persist to database
        /// - This allows multiple operations to be batched in a single transaction
        /// 
        /// Example:
        /// var day = new ItineraryDay { TripId = tripId, DayNumber = 1, ... };
        /// await _repository.CreateAsync(day, cancellationToken);
        /// await _repository.SaveChangesAsync(cancellationToken);
        /// </summary>
        Task CreateAsync(ItineraryDay itineraryDay, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an itinerary day by its ID (including soft-deleted days).
        /// 
        /// Purpose:
        /// - Get a specific itinerary day when the user requests it
        /// - Validate that the day exists before updating/deleting
        /// 
        /// Parameters:
        /// - dayId: The unique identifier of the itinerary day
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - ItineraryDay object if found
        /// - null if day doesn't exist
        /// 
        /// Database Query:
        /// SELECT * FROM "ItineraryDays" WHERE "DayId" = @dayId
        /// 
        /// Note:
        /// - Does NOT filter by IsDeleted (returns soft-deleted days too)
        /// - Service layer is responsible for checking IsDeleted if needed
        /// - Uses AsNoTracking() for read-only queries to improve performance
        /// 
        /// Example:
        /// var day = await _repository.GetByIdAsync(dayId, cancellationToken);
        /// if (day == null) throw new EntityNotFoundException("Day not found");
        /// </summary>
        Task<ItineraryDay?> GetByIdAsync(Guid dayId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all itinerary days for a specific trip (excluding soft-deleted days).
        /// 
        /// Purpose:
        /// - Get all days in a trip's itinerary
        /// - Display the itinerary timeline for the user
        /// - Filter to show only active (non-deleted) days
        /// 
        /// Parameters:
        /// - tripId: The ID of the trip
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - List of ItineraryDay objects
        /// - Empty list if trip has no days or all are deleted
        /// 
        /// Database Query:
        /// SELECT * FROM "ItineraryDays" 
        /// WHERE "TripId" = @tripId AND "IsDeleted" = false
        /// ORDER BY "DayNumber" ASC
        /// 
        /// Note:
        /// - Automatically filters out soft-deleted days (IsDeleted = false)
        /// - Results are ordered by DayNumber for chronological order
        /// - Uses AsNoTracking() for read-only queries to improve performance
        /// - Index usage: (TripId, IsDeleted)
        /// 
        /// Example:
        /// var days = await _repository.GetByTripIdAsync(tripId, cancellationToken);
        /// if (!days.Any()) { /* No itinerary yet */ }
        /// </summary>
        Task<List<ItineraryDay>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an itinerary day number already exists in a trip (excluding soft-deleted days).
        /// 
        /// Purpose:
        /// - Validate that DayNumber is unique within the trip
        /// - Prevent duplicate day numbers during create/update operations
        /// - This check is done in the service layer, but repository provides the primitive operation
        /// 
        /// Parameters:
        /// - tripId: The ID of the trip
        /// - dayNumber: The day number to check
        /// - excludeDayId: Optional DayId to exclude (for update operations, exclude the current day)
        /// - cancellationToken: Allows cancellation
        /// 
        /// Returns:
        /// - true if a day with this number exists in the trip
        /// - false if no such day exists
        /// 
        /// Database Query:
        /// SELECT COUNT(*) FROM "ItineraryDays" 
        /// WHERE "TripId" = @tripId AND "DayNumber" = @dayNumber AND "IsDeleted" = false
        /// [AND "DayId" != @excludeDayId]
        /// 
        /// Note:
        /// - Filters out soft-deleted days
        /// - When excludeDayId is provided, that day is excluded from the check
        /// - Useful for updates: allows same day number if we're updating that day
        /// - Uses Count() for efficiency (no need to fetch the actual entity)
        /// 
        /// Example (create):
        /// bool exists = await _repository.DayNumberExistsAsync(tripId, 1, null, ct);
        /// if (exists) throw new ValidationException("Day 1 already exists");
        /// 
        /// Example (update):
        /// bool exists = await _repository.DayNumberExistsAsync(tripId, 2, currentDayId, ct);
        /// if (exists) throw new ValidationException("Day 2 already exists in this trip");
        /// </summary>
        Task<bool> DayNumberExistsAsync(Guid tripId, int dayNumber, Guid? excludeDayId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing itinerary day.
        /// 
        /// Purpose:
        /// - Modify an existing day's details
        /// - Update is not persisted until SaveChangesAsync() is called
        /// 
        /// Parameters:
        /// - itineraryDay: The ItineraryDay entity with updated values
        /// - cancellationToken: Allows cancellation of the operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Behavior:
        /// - Entity is updated in the DbContext but not saved
        /// - SaveChangesAsync() must be called separately
        /// - Service layer is responsible for updating audit fields (UpdatedAt, LastModifiedBy)
        /// 
        /// Example:
        /// day.Title = "New Title";
        /// day.UpdatedAt = DateTime.UtcNow;
        /// day.LastModifiedBy = userId;
        /// await _repository.UpdateAsync(day, cancellationToken);
        /// await _repository.SaveChangesAsync(cancellationToken);
        /// </summary>
        Task UpdateAsync(ItineraryDay itineraryDay, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a soft delete on an itinerary day.
        /// 
        /// Purpose:
        /// - Mark a day as deleted without physically removing from database
        /// - Maintain audit trail of all changes
        /// - Allow potential recovery of deleted data
        /// 
        /// Parameters:
        /// - dayId: The ID of the day to delete
        /// - deletedAt: The timestamp when deletion occurred (usually DateTime.UtcNow)
        /// - cancellationToken: Allows cancellation of the operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Behavior:
        /// - Sets IsDeleted = true and DeletedAt = deletedAt
        /// - Does NOT physically remove the record from the database
        /// - Change is not persisted until SaveChangesAsync() is called
        /// - Service layer is responsible for providing the deletedAt timestamp
        /// 
        /// Example:
        /// await _repository.DeleteAsync(dayId, DateTime.UtcNow, cancellationToken);
        /// await _repository.SaveChangesAsync(cancellationToken);
        /// </summary>
        Task DeleteAsync(Guid dayId, DateTime deletedAt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves all changes made to the context to the database.
        /// 
        /// Purpose:
        /// - Persist all pending changes (create, update, delete) in a single transaction
        /// - Ensure atomicity: all changes succeed or all fail together
        /// - Handle database-level validation and constraints
        /// 
        /// Parameters:
        /// - cancellationToken: Allows cancellation of the database save
        /// 
        /// Returns:
        /// - int: Number of entities changed in the database
        /// 
        /// Behavior:
        /// - Commits all pending changes to the database as an atomic transaction
        /// - If any error occurs, all changes are rolled back
        /// - Throws DbUpdateException on database constraint violations
        /// 
        /// Example:
        /// await _repository.CreateAsync(day, ct);
        /// int changes = await _repository.SaveChangesAsync(ct);
        /// // changes = 1 (one entity was inserted)
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
