using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Activity entity data access operations.
    /// 
    /// This interface abstracts all database operations related to the Activity entity.
    /// The concrete implementation (ActivityRepository) uses Entity Framework Core,
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
    /// The ActivityService injects this interface and calls its methods without
    /// knowing or caring about the underlying database technology (EF Core, SQL, etc.).
    /// 
    /// Async Operations:
    /// All methods are async for scalability and responsiveness.
    /// Network I/O (database queries) should never block threads.
    /// 
    /// CancellationToken:
    /// All methods accept optional CancellationToken for graceful cancellation.
    /// This allows long-running operations to be cancelled by the client.
    /// 
    /// Soft Delete:
    /// - Activities are never physically deleted
    /// - Instead, IsDeleted is set to true and DeletedAt records the time
    /// - Queries must filter out deleted activities when needed
    /// - Some queries return all records including soft-deleted, others exclude them
    /// </summary>
    public interface IActivityRepository
    {
        /// <summary>
        /// Creates a new activity in the database.
        /// 
        /// Purpose:
        /// - Insert a newly created activity into the database
        /// - Used when a user creates a new activity in their itinerary day
        /// 
        /// Parameters:
        /// - activity: The Activity entity to create
        /// - cancellationToken: Allows cancellation of the database operation
        /// 
        /// Returns:
        /// - Task representing the asynchronous operation
        /// 
        /// Database Operation:
        /// INSERT INTO "Activities" 
        /// (ActivityId, DayId, ActivityName, Category, StartTime, EndTime, Priority, Status, 
        ///  CreatedAt, UpdatedAt, CreatedBy, LastModifiedBy, IsDeleted) 
        /// VALUES (...)
        /// 
        /// Behavior:
        /// - Entity is added to the DbContext but not saved
        /// - SaveChangesAsync() must be called separately to persist to database
        /// - This allows multiple operations to be batched in a single transaction
        /// 
        /// Example:
        /// var activity = new Activity { DayId = dayId, ActivityName = "Eiffel Tower", ... };
        /// await _repository.CreateAsync(activity, cancellationToken);
        /// await _repository.SaveChangesAsync(cancellationToken);
        /// </summary>
        Task CreateAsync(Activity activity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an activity by its ID (including soft-deleted activities).
        /// 
        /// Purpose:
        /// - Get a specific activity when the user requests it
        /// - Validate that the activity exists before updating/deleting
        /// 
        /// Parameters:
        /// - activityId: The unique identifier of the activity
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - Activity object if found
        /// - null if activity doesn't exist
        /// 
        /// Database Query:
        /// SELECT * FROM "Activities" WHERE "ActivityId" = @activityId
        /// 
        /// Note:
        /// - Does NOT filter by IsDeleted (returns soft-deleted activities too)
        /// - Service layer is responsible for checking IsDeleted if needed
        /// - Uses AsNoTracking() for read-only queries to improve performance
        /// - Includes related ItineraryDay for ownership verification
        /// 
        /// Example:
        /// var activity = await _repository.GetByIdAsync(activityId, cancellationToken);
        /// if (activity == null) throw new EntityNotFoundException("Activity not found");
        /// </summary>
        Task<Activity?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all activities for a specific itinerary day (excluding soft-deleted activities).
        /// 
        /// Purpose:
        /// - Get all activities in an itinerary day
        /// - Display activities in chronological order for the user
        /// - Filter to show only active (non-deleted) activities
        /// 
        /// Parameters:
        /// - dayId: The ID of the itinerary day
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - List of Activity objects
        /// - Empty list if day has no activities or all are deleted
        /// 
        /// Database Query:
        /// SELECT * FROM "Activities" 
        /// WHERE "DayId" = @dayId AND "IsDeleted" = false
        /// ORDER BY "StartTime" ASC
        /// 
        /// Note:
        /// - Automatically filters out soft-deleted activities (IsDeleted = false)
        /// - Results are ordered by StartTime for chronological order
        /// - Uses AsNoTracking() for read-only queries to improve performance
        /// - Index usage: (DayId, IsDeleted)
        /// 
        /// Example:
        /// var activities = await _repository.GetByDayIdAsync(dayId, cancellationToken);
        /// if (!activities.Any()) { /* Empty day */ }
        /// </summary>
        Task<List<Activity>> GetByDayIdAsync(Guid dayId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all activities for a specific itinerary day filtered by status (excluding soft-deleted).
        /// 
        /// Purpose:
        /// - Get activities filtered by current status (Planned, Booked, Completed, etc.)
        /// - Display activities grouped by status
        /// - Analysis of activity completion rates
        /// 
        /// Parameters:
        /// - dayId: The ID of the itinerary day
        /// - status: The status to filter by (integer enum value)
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - List of Activity objects matching the status
        /// - Empty list if no activities with that status exist
        /// 
        /// Database Query:
        /// SELECT * FROM "Activities" 
        /// WHERE "DayId" = @dayId AND "Status" = @status AND "IsDeleted" = false
        /// ORDER BY "StartTime" ASC
        /// 
        /// Index usage: (DayId, Status, IsDeleted)
        /// 
        /// Example:
        /// var plannedActivities = await _repository.GetByDayIdAndStatusAsync(dayId, (int)ActivityStatus.Planned, ct);
        /// var completedActivities = await _repository.GetByDayIdAndStatusAsync(dayId, (int)ActivityStatus.Completed, ct);
        /// </summary>
        Task<List<Activity>> GetByDayIdAndStatusAsync(Guid dayId, int status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves activities for a specific itinerary day filtered by category (excluding soft-deleted).
        /// 
        /// Purpose:
        /// - Get activities grouped by category (Sightseeing, Restaurant, etc.)
        /// - Display activities by type
        /// - Cost analysis by category
        /// 
        /// Parameters:
        /// - dayId: The ID of the itinerary day
        /// - category: The category to filter by (integer enum value)
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - List of Activity objects matching the category
        /// - Empty list if no activities with that category exist
        /// 
        /// Database Query:
        /// SELECT * FROM "Activities" 
        /// WHERE "DayId" = @dayId AND "Category" = @category AND "IsDeleted" = false
        /// ORDER BY "StartTime" ASC
        /// 
        /// Index usage: (DayId, Category, IsDeleted)
        /// 
        /// Example:
        /// var restaurants = await _repository.GetByDayIdAndCategoryAsync(dayId, (int)ActivityCategory.Restaurant, ct);
        /// var sightseeing = await _repository.GetByDayIdAndCategoryAsync(dayId, (int)ActivityCategory.Sightseeing, ct);
        /// </summary>
        Task<List<Activity>> GetByDayIdAndCategoryAsync(Guid dayId, int category, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all activities for a trip across all days (excluding soft-deleted).
        /// 
        /// Purpose:
        /// - Get all activities in an entire trip
        /// - Used for trip-wide cost analysis
        /// - Display full itinerary across multiple days
        /// 
        /// Parameters:
        /// - tripId: The ID of the trip
        /// - cancellationToken: Allows cancellation of the database query
        /// 
        /// Returns:
        /// - List of Activity objects across all days of the trip
        /// - Empty list if trip has no activities
        /// 
        /// Database Query (conceptual):
        /// SELECT a.* FROM "Activities" a
        /// INNER JOIN "ItineraryDays" d ON a."DayId" = d."DayId"
        /// WHERE d."TripId" = @tripId AND a."IsDeleted" = false
        /// ORDER BY d."Date" ASC, a."StartTime" ASC
        /// 
        /// Note:
        /// - Must join with ItineraryDays to filter by trip
        /// - Results ordered by day date then activity start time
        /// 
        /// Example:
        /// var allActivities = await _repository.GetByTripIdAsync(tripId, cancellationToken);
        /// var totalCost = allActivities.Sum(a => a.EstimatedCost);
        /// </summary>
        Task<List<Activity>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an activity with specific name already exists in a day (excluding soft-deleted).
        /// 
        /// Purpose:
        /// - Validate that activity names are unique within a day (optional business rule)
        /// - Prevent duplicate activities
        /// 
        /// Parameters:
        /// - dayId: The ID of the itinerary day
        /// - activityName: The name to check
        /// - excludeActivityId: Optional ActivityId to exclude (for update operations)
        /// - cancellationToken: Allows cancellation
        /// 
        /// Returns:
        /// - true if an activity with this name exists in the day
        /// - false if no such activity exists
        /// 
        /// Database Query:
        /// SELECT COUNT(*) FROM "Activities" 
        /// WHERE "DayId" = @dayId AND "ActivityName" = @activityName AND "IsDeleted" = false
        /// [AND "ActivityId" != @excludeActivityId]
        /// 
        /// Note:
        /// - Case-sensitive comparison (depends on database collation)
        /// - Can be made case-insensitive if needed
        /// - When excludeActivityId is provided, that activity is excluded
        /// 
        /// Example (create):
        /// bool exists = await _repository.ActivityNameExistsAsync(dayId, "Eiffel Tower", null, ct);
        /// if (exists) throw new ValidationException("Activity already added for this day");
        /// 
        /// Example (update):
        /// bool exists = await _repository.ActivityNameExistsAsync(dayId, newName, currentActivityId, ct);
        /// if (exists) throw new ValidationException("Another activity has this name");
        /// </summary>
        Task<bool> ActivityNameExistsAsync(Guid dayId, string activityName, Guid? excludeActivityId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves activities for a specific day within a time range.
        /// 
        /// Purpose:
        /// - Find schedule conflicts or overlaps
        /// - Check available time slots for new activities
        /// - Validate that activities don't overlap
        /// 
        /// Parameters:
        /// - dayId: The ID of the itinerary day
        /// - startTime: The start time to search from
        /// - endTime: The end time to search to
        /// - excludeActivityId: Optional ActivityId to exclude (for update operations)
        /// - cancellationToken: Allows cancellation
        /// 
        /// Returns:
        /// - List of Activity objects that overlap with the time range
        /// - Empty list if no conflicts
        /// 
        /// Database Query:
        /// SELECT * FROM "Activities" 
        /// WHERE "DayId" = @dayId 
        /// AND "IsDeleted" = false
        /// AND (("StartTime" < @endTime AND "EndTime" > @startTime))
        /// [AND "ActivityId" != @excludeActivityId]
        /// ORDER BY "StartTime" ASC
        /// 
        /// Note:
        /// - Uses overlap logic: activity overlaps if it starts before requested end and ends after requested start
        /// - Can be used for schedule conflict detection
        /// 
        /// Example:
        /// var conflicts = await _repository.GetActivitiesInTimeRangeAsync(
        ///     dayId, new TimeOnly(10, 0), new TimeOnly(12, 0), null, ct);
        /// if (conflicts.Any()) { /* Schedule conflict detected */ }
        /// </summary>
        Task<List<Activity>> GetActivitiesInTimeRangeAsync(
            Guid dayId, 
            TimeOnly startTime, 
            TimeOnly endTime, 
            Guid? excludeActivityId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing activity.
        /// 
        /// Purpose:
        /// - Modify an existing activity's details
        /// - Update is not persisted until SaveChangesAsync() is called
        /// 
        /// Parameters:
        /// - activity: The Activity entity with updated values
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
        /// activity.ActivityName = "New Name";
        /// activity.Status = (int)ActivityStatus.Booked;
        /// activity.UpdatedAt = DateTime.UtcNow;
        /// activity.LastModifiedBy = userId;
        /// await _repository.UpdateAsync(activity, cancellationToken);
        /// await _repository.SaveChangesAsync(cancellationToken);
        /// </summary>
        Task UpdateAsync(Activity activity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a soft delete on an activity.
        /// 
        /// Purpose:
        /// - Mark an activity as deleted without physically removing from database
        /// - Maintain audit trail of all changes
        /// - Allow potential recovery of deleted data
        /// 
        /// Parameters:
        /// - activityId: The ID of the activity to delete
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
        /// await _repository.DeleteAsync(activityId, DateTime.UtcNow, cancellationToken);
        /// await _repository.SaveChangesAsync(cancellationToken);
        /// </summary>
        Task DeleteAsync(Guid activityId, DateTime deletedAt, CancellationToken cancellationToken = default);

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
        /// await _repository.CreateAsync(activity, ct);
        /// int changes = await _repository.SaveChangesAsync(ct);
        /// // changes = 1 (one entity was inserted)
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
