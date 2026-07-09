using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Itinerary;

namespace VoyageAI.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for itinerary day operations.
    /// 
    /// This interface defines the contract for all itinerary day business logic.
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
    /// Injected as: services.AddScoped{IItineraryService, ItineraryService}()
    /// 
    /// Authorization:
    /// All methods validate that the requesting user owns the trip containing the itinerary.
    /// This is critical for security - users should only access their own itineraries.
    /// 
    /// Return Type:
    /// All methods return ApiResponse{T} which includes success/error status,
    /// error messages, and the actual data payload.
    /// This standardizes API responses and enables consistent error handling.
    /// 
    /// Usage in Controller:
    /// [ApiController]
    /// [Route("api/[controller]")]
    /// [Authorize]  // Requires JWT token
    /// public class ItineraryController : ControllerBase
    /// {
    ///     private readonly IItineraryService _itineraryService;
    ///     
    ///     [HttpPost("{tripId}/itinerary")]
    ///     public async Task{ActionResult{ApiResponse{ItineraryDayResponse}}} CreateItineraryDay(
    ///         Guid tripId,
    ///         CreateItineraryDayRequest request,
    ///         CancellationToken ct)
    ///     {
    ///         var userId = User.FindFirst("sub").Value;  // From JWT token
    ///         var response = await _itineraryService.CreateItineraryDayAsync(
    ///             Guid.Parse(userId),
    ///             tripId,
    ///             request,
    ///             ct);
    ///         return Created("", response);
    ///     }
    /// }
    /// </summary>
    public interface IItineraryService
    {
        /// <summary>
        /// Creates a new itinerary day for a trip.
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user (UserId from JWT, never from request)
        /// 2. Load trip from database
        /// 3. Verify trip exists
        /// 4. Verify user owns the trip (TripId.UserId == userId)
        /// 5. Validate input (DataAnnotations + FluentValidation)
        /// 6. Validate date falls within trip's StartDate and EndDate
        /// 7. Validate DayNumber is unique within the trip
        /// 8. Create ItineraryDay entity from CreateItineraryDayRequest
        /// 9. Set audit fields (CreatedAt, UpdatedAt, CreatedBy, LastModifiedBy)
        /// 10. Set soft delete fields (IsDeleted = false, DeletedAt = null)
        /// 11. Persist to database via repository
        /// 12. Map to ItineraryDayResponse
        /// 13. Return success response
        /// 
        /// Error Handling:
        /// - Invalid input → 400 Bad Request (validation layer)
        /// - Invalid date range → 400 Bad Request
        /// - Duplicate DayNumber → 409 Conflict
        /// - Trip not found → 404 Not Found
        /// - User doesn't own trip → 403 Forbidden
        /// - Database error → 500 Internal Server Error
        /// 
        /// Security Considerations:
        /// - UserId is extracted from JWT token (not from request body)
        /// - Trip ownership is validated before creating day
        /// - No way for user to create days in other users' trips
        /// 
        /// Response on Success (201 Created):
        /// {
        ///     "dayId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440001",
        ///     "dayNumber": 1,
        ///     "date": "2025-06-01T00:00:00Z",
        ///     "title": "Arrival in Paris",
        ///     "summary": "Arrive at Charles de Gaulle, check into hotel",
        ///     "notes": "Flight arrives at 2 PM. Hotel: Le Marais, confirmation #ABC123",
        ///     "estimatedBudget": 200.00,
        ///     "actualBudget": 0.00,
        ///     "weatherSummary": "Sunny, 22°C",
        ///     "createdAt": "2025-01-06T14:30:00Z",
        ///     "updatedAt": "2025-01-06T14:30:00Z",
        ///     "createdBy": "550e8400-e29b-41d4-a716-446655440002",
        ///     "lastModifiedBy": "550e8400-e29b-41d4-a716-446655440002",
        ///     "isDeleted": false,
        ///     "deletedAt": null
        /// }
        /// 
        /// Example Usage:
        /// var request = new CreateItineraryDayRequest
        /// {
        ///     DayNumber = 1,
        ///     Date = new DateTime(2025, 6, 1),
        ///     Title = "Arrival in Paris",
        ///     ...
        /// };
        /// var response = await _service.CreateItineraryDayAsync(userId, tripId, request, ct);
        /// </summary>
        Task<ApiResponse<ItineraryDayResponse>> CreateItineraryDayAsync(
            Guid userId,
            Guid tripId,
            CreateItineraryDayRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all itinerary days for a trip.
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user
        /// 2. Load trip from database
        /// 3. Verify trip exists
        /// 4. Verify user owns the trip
        /// 5. Load all itinerary days for the trip (excluding soft-deleted)
        /// 6. Map each ItineraryDay to ItinerarySummaryResponse
        /// 7. Return list ordered by DayNumber
        /// 
        /// Error Handling:
        /// - Trip not found → 404 Not Found
        /// - User doesn't own trip → 403 Forbidden
        /// 
        /// Security:
        /// - Authorization check ensures user can only see their own trip's itinerary
        /// 
        /// Response on Success (200 OK):
        /// {
        ///     "data": [
        ///         {
        ///             "dayId": "550e8400-e29b-41d4-a716-446655440000",
        ///             "tripId": "550e8400-e29b-41d4-a716-446655440001",
        ///             "dayNumber": 1,
        ///             "date": "2025-06-01T00:00:00Z",
        ///             "title": "Arrival in Paris",
        ///             "summary": "Arrive at CDG, check in",
        ///             "estimatedBudget": 200.00,
        ///             "actualBudget": 150.00,
        ///             "weatherSummary": "Sunny, 22°C",
        ///             "updatedAt": "2025-01-06T14:30:00Z",
        ///             "isDeleted": false
        ///         },
        ///         ...
        ///     ],
        ///     "success": true
        /// }
        /// </summary>
        Task<ApiResponse<List<ItinerarySummaryResponse>>> GetItineraryDaysAsync(
            Guid userId,
            Guid tripId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific itinerary day.
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user
        /// 2. Load itinerary day from database
        /// 3. Verify day exists
        /// 4. Load trip for authorization check
        /// 5. Verify user owns the trip
        /// 6. Map to ItineraryDayResponse (includes audit trail)
        /// 7. Return day
        /// 
        /// Error Handling:
        /// - Day not found → 404 Not Found
        /// - Trip not found → 404 Not Found
        /// - User doesn't own trip → 403 Forbidden
        /// 
        /// Security:
        /// - Authorization check ensures user can only see days in their own trips
        /// 
        /// Response on Success (200 OK):
        /// {
        ///     "data": {
        ///         "dayId": "550e8400-e29b-41d4-a716-446655440000",
        ///         "tripId": "550e8400-e29b-41d4-a716-446655440001",
        ///         "dayNumber": 1,
        ///         "date": "2025-06-01T00:00:00Z",
        ///         "title": "Arrival in Paris",
        ///         "summary": "Arrive at Charles de Gaulle, check into hotel",
        ///         "notes": "Flight arrives at 2 PM. Hotel confirmation #ABC123",
        ///         "estimatedBudget": 200.00,
        ///         "actualBudget": 150.00,
        ///         "weatherSummary": "Sunny, 22°C",
        ///         "createdAt": "2025-01-06T14:30:00Z",
        ///         "updatedAt": "2025-01-06T14:35:00Z",
        ///         "createdBy": "550e8400-...",
        ///         "lastModifiedBy": "550e8400-...",
        ///         "isDeleted": false,
        ///         "deletedAt": null
        ///     },
        ///     "success": true
        /// }
        /// </summary>
        Task<ApiResponse<ItineraryDayResponse>> GetItineraryDayAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing itinerary day.
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user
        /// 2. Load itinerary day from database
        /// 3. Verify day exists
        /// 4. Load trip for authorization check
        /// 5. Verify user owns the trip
        /// 6. Validate input (DataAnnotations + FluentValidation)
        /// 7. Validate new date falls within trip's StartDate and EndDate
        /// 8. If DayNumber changed, verify new DayNumber is unique (excluding current day)
        /// 9. Update entity properties from UpdateItineraryDayRequest
        /// 10. Update audit fields (UpdatedAt = DateTime.UtcNow, LastModifiedBy = userId)
        /// 11. Persist to database
        /// 12. Map to ItineraryDayResponse
        /// 13. Return updated day
        /// 
        /// Error Handling:
        /// - Invalid input → 400 Bad Request
        /// - Invalid date range → 400 Bad Request
        /// - Duplicate DayNumber → 409 Conflict
        /// - Day not found → 404 Not Found
        /// - Trip not found → 404 Not Found
        /// - User doesn't own trip → 403 Forbidden
        /// 
        /// What Can Be Updated:
        /// - DayNumber, Date, Title, Summary, Notes
        /// - EstimatedBudget, ActualBudget, WeatherSummary
        /// 
        /// What Cannot Be Updated:
        /// - DayId: Immutable
        /// - TripId: Immutable (use DELETE + CREATE to move to different trip)
        /// - CreatedAt, CreatedBy: Immutable
        /// - IsDeleted, DeletedAt: Use DELETE endpoint instead
        /// 
        /// Security:
        /// - Authorization ensures user can only update days in their own trips
        /// - Audit fields automatically track who made changes
        /// </summary>
        Task<ApiResponse<ItineraryDayResponse>> UpdateItineraryDayAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            UpdateItineraryDayRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an itinerary day (soft delete).
        /// 
        /// Business Logic:
        /// 1. Verify authenticated user
        /// 2. Load itinerary day from database
        /// 3. Verify day exists
        /// 4. Load trip for authorization check
        /// 5. Verify user owns the trip
        /// 6. Mark as soft-deleted (IsDeleted = true, DeletedAt = DateTime.UtcNow)
        /// 7. Persist to database
        /// 8. Return success response
        /// 
        /// Soft Delete:
        /// - Record is NOT physically removed from database
        /// - IsDeleted flag prevents inclusion in normal queries
        /// - Data is preserved for audit and potential recovery
        /// - Useful for maintaining referential integrity with Activities
        /// 
        /// Error Handling:
        /// - Day not found → 404 Not Found
        /// - Trip not found → 404 Not Found
        /// - User doesn't own trip → 403 Forbidden
        /// 
        /// Response on Success (204 No Content):
        /// Empty response body. HTTP 204 indicates successful deletion.
        /// </summary>
        Task<ApiResponse<object>> DeleteItineraryDayAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            CancellationToken cancellationToken = default);
    }
}
