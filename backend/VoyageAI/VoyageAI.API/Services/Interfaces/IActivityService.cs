using VoyageAI.API.DTOs.Activities;

namespace VoyageAI.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for activity operations.
    /// 
    /// This interface defines the contract for all activity business logic.
    /// All complex business rules, validations, and decisions are handled by the service layer.
    /// 
    /// Separation of Concerns:
    /// - Controller: HTTP routing only, calls service
    /// - Service: Business logic, validation, authorization, audit fields
    /// - Repository: Data access only
    /// - Entity: Database schema only
    /// 
    /// Design Patterns:
    /// - Service Pattern: Encapsulates business logic
    /// - Dependency Inversion: Controllers depend on interface, not concrete class
    /// - Async Operations: All methods are async for scalability
    /// 
    /// Dependency Injection:
    /// Injected as: services.AddScoped<IActivityService, ActivityService>()
    /// 
    /// Authorization:
    /// All methods validate that the requesting user owns the trip containing the itinerary day.
    /// This is critical for security - users should only access their own activities.
    /// 
    /// Ownership Chain:
    /// Activity -> ItineraryDay -> Trip -> User
    /// Every activity access must verify the user owns the trip.
    /// 
    /// Exception Handling:
    /// This service throws exceptions instead of returning ApiResponse<T>:
    /// - EntityNotFoundException: Entity not found (404 response by middleware)
    /// - ForbiddenException: User lacks permission (403 response by middleware)
    /// - ValidationException: Validation failure (400 response by middleware)
    /// - ConflictException: Conflict like time overlap (409 response by middleware)
    /// 
    /// The GlobalExceptionMiddleware catches these and converts them to appropriate HTTP responses.
    /// 
    /// Usage in Controller:
    /// [ApiController]
    /// [Route("api/[controller]")]
    /// [Authorize]  // Requires JWT token
    /// public class ActivityController : ControllerBase
    /// {
    ///     private readonly IActivityService _activityService;
    ///     
    ///     [HttpPost("{tripId}/itinerary/{dayId}/activities")]
    ///     public async Task<ActionResult<ActivityResponse>> CreateActivity(
    ///         Guid tripId,
    ///         Guid dayId,
    ///         CreateActivityRequest request,
    ///         CancellationToken ct)
    ///     {
    ///         var userId = User.FindFirst("sub").Value;  // From JWT token
    ///         var response = await _activityService.CreateActivityAsync(
    ///             userId: Guid.Parse(userId),
    ///             tripId: tripId,
    ///             dayId: dayId,
    ///             request: request,
    ///             cancellationToken: ct);
    ///         return Created("", response);
    ///     }
    /// }
    /// </summary>
    public interface IActivityService
    {
        /// <summary>
        /// Creates a new activity for an itinerary day.
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user (UserId from JWT, never from request)
        /// 2. Load itinerary day from database
        /// 3. Verify day exists
        /// 4. Load trip for authorization check
        /// 5. Verify user owns the trip (critical security check)
        /// 6. Verify day belongs to the trip (extra validation)
        /// 7. Validate input (DataAnnotations + FluentValidation)
        /// 8. Validate start time is before end time
        /// 9. Check for time conflicts with existing activities
        /// 10. Set default status to Planned (0) if not provided
        /// 11. Calculate duration in minutes from start and end times
        /// 12. Create Activity entity from CreateActivityRequest
        /// 13. Set audit fields (CreatedAt, UpdatedAt, CreatedBy, LastModifiedBy)
        /// 14. Persist to database via repository
        /// 15. Map to ActivityResponse
        /// 16. Return success response
        /// 
        /// Error Handling:
        /// - Validation errors via FluentValidation → ValidationException → 400 Bad Request
        /// - End time before start time → ValidationException → 400 Bad Request
        /// - Time conflict with existing activities → ConflictException → 409 Conflict
        /// - Day not found → EntityNotFoundException → 404 Not Found
        /// - Trip not found → EntityNotFoundException → 404 Not Found
        /// - User doesn't own trip → ForbiddenException → 403 Forbidden
        /// 
        /// Security Considerations:
        /// - UserId is extracted from JWT token (not from request body)
        /// - Trip ownership is validated before creating activity
        /// - Day belongs to trip is verified
        /// - No way for user to create activities in other users' itineraries
        /// 
        /// Time Validation:
        /// - EndTime must be after StartTime (validated)
        /// - Activity times should fall within reasonable day hours (validated by service)
        /// - Time conflicts with other activities in the same day are checked
        /// 
        /// Default Values:
        /// - Status: Planned (0) if not provided
        /// - DurationMinutes: Calculated from EndTime - StartTime
        /// - IsDeleted: false (set by entity constructor)
        /// 
        /// Exception Response Pattern (handled by GlobalExceptionMiddleware):
        /// Returns 201 Created on success with ActivityResponse in body
        /// 
        /// Example Usage:
        /// var request = new CreateActivityRequest
        /// {
        ///     ActivityName = "Visit Eiffel Tower",
        ///     Category = 0,  // Sightseeing
        ///     StartTime = new TimeOnly(9, 0),
        ///     EndTime = new TimeOnly(11, 30),
        ///     Priority = 3,  // MustVisit
        ///     Status = 0     // Planned
        /// };
        /// var response = await _service.CreateActivityAsync(userId, tripId, dayId, request, ct);
        /// </summary>
        Task<ActivityResponse> CreateActivityAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            CreateActivityRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all activities for an itinerary day.
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user
        /// 2. Load itinerary day from database
        /// 3. Verify day exists
        /// 4. Load trip for authorization check
        /// 5. Verify user owns the trip
        /// 6. Load all activities for the day (ordered by start time)
        /// 7. Map each Activity to ActivityResponse
        /// 8. Return list
        /// 
        /// Error Handling:
        /// - Day not found → EntityNotFoundException → 404 Not Found
        /// - Trip not found → EntityNotFoundException → 404 Not Found
        /// - User doesn't own trip → ForbiddenException → 403 Forbidden
        /// 
        /// Security:
        /// - Authorization check ensures user can only see activities in their own itineraries
        /// 
        /// Response on Success (200 OK):
        /// Returns list of activities ordered by start time, ready for display in day view.
        /// </summary>
        Task<List<ActivityResponse>> GetActivitiesAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific activity.
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user
        /// 2. Load activity from database
        /// 3. Verify activity exists
        /// 4. Load related day and trip for authorization check
        /// 5. Verify user owns the trip
        /// 6. Map to ActivityResponse (includes audit trail)
        /// 7. Return activity
        /// 
        /// Error Handling:
        /// - Activity not found → EntityNotFoundException → 404 Not Found
        /// - Day not found → EntityNotFoundException → 404 Not Found
        /// - Trip not found → EntityNotFoundException → 404 Not Found
        /// - User doesn't own trip → ForbiddenException → 403 Forbidden
        /// - Activity doesn't belong to specified day → ValidationException → 400 Bad Request
        /// 
        /// Security:
        /// - Authorization check ensures user can only see activities in their own itineraries
        /// 
        /// Response on Success (200 OK):
        /// Returns full activity details including audit information.
        /// </summary>
        Task<ActivityResponse> GetActivityAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            Guid activityId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing activity.
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user
        /// 2. Load activity from database
        /// 3. Verify activity exists
        /// 4. Load related day and trip for authorization check
        /// 5. Verify user owns the trip
        /// 6. Validate input (DataAnnotations + FluentValidation)
        /// 7. If times changed, validate new end time > new start time
        /// 8. If times changed, check for conflicts with other activities
        /// 9. Update entity properties from UpdateActivityRequest (only non-null values)
        /// 10. Recalculate duration if times changed
        /// 11. Update audit fields (UpdatedAt = DateTime.UtcNow, LastModifiedBy = userId)
        /// 12. Persist to database
        /// 13. Map to ActivityResponse
        /// 14. Return updated activity
        /// 
        /// Error Handling:
        /// - Validation errors via FluentValidation → ValidationException → 400 Bad Request
        /// - End time before start time → ValidationException → 400 Bad Request
        /// - New times conflict with other activities → ConflictException → 409 Conflict
        /// - Activity not found → EntityNotFoundException → 404 Not Found
        /// - Day not found → EntityNotFoundException → 404 Not Found
        /// - Trip not found → EntityNotFoundException → 404 Not Found
        /// - User doesn't own trip → ForbiddenException → 403 Forbidden
        /// - Activity doesn't belong to specified day → ValidationException → 400 Bad Request
        /// 
        /// What Can Be Updated:
        /// - ActivityName, Description, Category, LocationName, Address
        /// - Latitude, Longitude, StartTime, EndTime
        /// - EstimatedCost, ActualCost, BookingReference
        /// - Website, Phone, Notes, Priority, Status, ImageUrl, IsAiGenerated
        /// 
        /// What Cannot Be Updated:
        /// - ActivityId: Immutable
        /// - DayId: Immutable (use DELETE + CREATE to move to different day)
        /// - CreatedAt, CreatedBy: Immutable
        /// 
        /// Partial Updates:
        /// - Only non-null properties from the request are updated
        /// - Allows updating individual fields without providing full data
        /// 
        /// Security:
        /// - Authorization ensures user can only update activities in their own itineraries
        /// - Audit fields automatically track who made changes
        /// </summary>
        Task<ActivityResponse> UpdateActivityAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            Guid activityId,
            UpdateActivityRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an activity (hard delete - permanently removes from database).
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user
        /// 2. Load activity from database
        /// 3. Verify activity exists
        /// 4. Load related day and trip for authorization check
        /// 5. Verify user owns the trip
        /// 6. Permanently delete the activity from the database
        /// 7. Persist changes to database
        /// 8. Return success response
        /// 
        /// Hard Delete:
        /// - Record IS physically removed from database
        /// - Cannot be recovered without database backup
        /// - More complete removal than soft delete
        /// - Used for permanent deletion at user request
        /// 
        /// Error Handling:
        /// - Activity not found → EntityNotFoundException → 404 Not Found
        /// - Day not found → EntityNotFoundException → 404 Not Found
        /// - Trip not found → EntityNotFoundException → 404 Not Found
        /// - User doesn't own trip → ForbiddenException → 403 Forbidden
        /// - Activity doesn't belong to specified day → ValidationException → 400 Bad Request
        /// 
        /// Response on Success (204 No Content):
        /// Empty response body. HTTP 204 indicates successful deletion.
        /// 
        /// Security:
        /// - Authorization ensures user can only delete activities in their own itineraries
        /// - Hard delete is permanent and cannot be undone
        /// </summary>
        Task DeleteActivityAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            Guid activityId,
            CancellationToken cancellationToken = default);
    }
}
