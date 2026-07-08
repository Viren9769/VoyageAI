using VoyageAI.API.DTOs.Trip;

namespace VoyageAI.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for trip operations.
    /// 
    /// This interface defines the contract for all trip business logic.
    /// All complex business rules, validations, and decisions are handled by the service layer.
    /// 
    /// Separation of Concerns:
    /// - Controller: HTTP routing only, calls service
    /// - Service: Business logic, validation, orchestration
    /// - Repository: Data access only
    /// - Entity: Database schema only
    /// 
    /// Design Patterns:
    /// - Service Pattern: Encapsulates business logic
    /// - Dependency Inversion: Controllers depend on interface, not concrete class
    /// - Async Operations: All methods are async for scalability
    /// 
    /// Dependency Injection:
    /// Injected as: services.AddScoped{ITripService, TripService}()
    /// 
    /// Authorization:
    /// All methods validate that the requesting user owns the trip.
    /// This is critical for security - users should only access their own trips.
    /// 
    /// Usage in Controller:
    /// [ApiController]
    /// [Route("api/[controller]")]
    /// [Authorize]  // Requires JWT token
    /// public class TripController : ControllerBase
    /// {
    ///     private readonly ITripService _tripService;
    ///     
    ///     [HttpPost]
    ///     public async Task{ActionResult{GetTripResponse}} CreateTrip(
    ///         CreateTripRequest request,
    ///         CancellationToken ct)
    ///     {
    ///         var userId = User.FindFirst("sub").Value;  // From JWT token
    ///         var response = await _tripService.CreateTripAsync(
    ///             Guid.Parse(userId),
    ///             request,
    ///             ct);
    ///         return Created("", response);
    ///     }
    /// }
    /// </summary>
    public interface ITripService
    {
        /// <summary>
        /// Creates a new trip for the authenticated user.
        /// 
        /// Business Logic:
        /// 1. Validate input (DataAnnotations + FluentValidation)
        /// 2. Validate dates (EndDate must be after StartDate)
        /// 3. Validate budget (must be positive)
        /// 4. Create Trip entity from CreateTripRequest
        /// 5. Associate trip with authenticated user (UserId from parameter)
        /// 6. Set timestamps (CreatedAt, UpdatedAt = DateTime.UtcNow)
        /// 7. Persist trip to database via repository
        /// 8. Map Trip to GetTripResponse
        /// 9. Return response with created trip
        /// 
        /// Error Handling:
        /// - Invalid input → 400 Bad Request (validation layer)
        /// - Invalid dates → 400 Bad Request (service validation)
        /// - Database error → 500 Internal Server Error
        /// 
        /// Security Considerations:
        /// - UserId is extracted from JWT token (not from request body)
        /// - Trip is automatically associated with authenticated user
        /// - No way for user to create trips for other users
        /// 
        /// Response on Success (201 Created):
        /// {
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "userId": "550e8400-e29b-41d4-a716-446655440001",
        ///     "tripName": "Summer Europe Adventure",
        ///     "destinationCountry": "France",
        ///     "destinationCity": "Paris",
        ///     "startDate": "2025-06-01T00:00:00Z",
        ///     "endDate": "2025-06-15T00:00:00Z",
        ///     "budget": 5000.00,
        ///     "currency": "USD",
        ///     "travelStyle": "Luxury",
        ///     "status": "Planning",
        ///     "createdAt": "2025-01-06T14:30:00Z",
        ///     "updatedAt": "2025-01-06T14:30:00Z",
        ///     "durationDays": 15
        /// }
        /// 
        /// Example Usage:
        /// var request = new CreateTripRequest
        /// {
        ///     TripName = "Europe Adventure",
        ///     DestinationCountry = "France",
        ///     DestinationCity = "Paris",
        ///     StartDate = new DateTime(2025, 6, 1),
        ///     EndDate = new DateTime(2025, 6, 15),
        ///     Budget = 5000,
        ///     Currency = "USD",
        ///     TravelStyle = "Luxury"
        /// };
        /// var response = await _tripService.CreateTripAsync(userId, request, ct);
        /// </summary>
        Task<GetTripResponse> CreateTripAsync(
            Guid userId,
            CreateTripRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all trips for the authenticated user.
        /// 
        /// Business Logic:
        /// 1. Query database for trips with matching UserId
        /// 2. Order trips by CreatedAt (newest first)
        /// 3. Map each Trip entity to GetTripResponse
        /// 4. Return list of responses
        /// 
        /// Authorization:
        /// - Users can only see their own trips
        /// - Database query filters by UserId
        /// 
        /// Returns:
        /// - List of GetTripResponse (can be empty if user has no trips)
        /// - Never null
        /// 
        /// Response Format:
        /// [
        ///     {
        ///         "tripId": "...",
        ///         "tripName": "Summer Adventure",
        ///         "destinationCountry": "France",
        ///         ...
        ///     },
        ///     {
        ///         "tripId": "...",
        ///         "tripName": "Winter Ski Trip",
        ///         "destinationCountry": "Switzerland",
        ///         ...
        ///     }
        /// ]
        /// 
        /// Example Usage:
        /// var trips = await _tripService.GetUserTripsAsync(userId, ct);
        /// return Ok(trips);  // 200 OK with array of trips
        /// </summary>
        Task<List<GetTripResponse>> GetUserTripsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific trip by ID.
        /// 
        /// Business Logic:
        /// 1. Query database for trip with matching TripId
        /// 2. Verify that the requesting user owns the trip (UserId matches)
        /// 3. If trip doesn't exist → throw EntityNotFoundException → 404
        /// 4. If user is not the owner → throw ForbiddenException → 403
        /// 5. Map Trip entity to GetTripResponse
        /// 6. Return response
        /// 
        /// Authorization:
        /// - Users can only access trips they own
        /// - Owner verification prevents unauthorized access
        /// 
        /// Error Handling:
        /// - Trip not found → 404 Not Found (EntityNotFoundException)
        /// - User is not owner → 403 Forbidden (ForbiddenException)
        /// 
        /// Response on Success (200 OK):
        /// {
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "tripName": "Summer Adventure",
        ///     ...
        /// }
        /// 
        /// Example Usage:
        /// var trip = await _tripService.GetTripByIdAsync(tripId, userId, ct);
        /// return Ok(trip);  // 200 OK with trip details
        /// </summary>
        Task<GetTripResponse> GetTripByIdAsync(
            Guid tripId,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing trip.
        /// 
        /// Business Logic:
        /// 1. Query database for trip with matching TripId
        /// 2. Verify that the requesting user owns the trip
        /// 3. If trip not found → throw EntityNotFoundException → 404
        /// 4. If user is not owner → throw ForbiddenException → 403
        /// 5. Apply updates from UpdateTripRequest (only specified fields)
        /// 6. Validate updated values (dates, budget, etc.)
        /// 7. Update UpdatedAt timestamp
        /// 8. Persist changes via repository
        /// 9. Map updated Trip to GetTripResponse
        /// 10. Return response
        /// 
        /// Partial Updates:
        /// - Request properties can be null (only non-null values are updated)
        /// - null values in request are ignored (field not updated)
        /// - Allows clients to update only the fields they need
        /// 
        /// Authorization:
        /// - Users can only update trips they own
        /// 
        /// Error Handling:
        /// - Trip not found → 404 Not Found
        /// - User is not owner → 403 Forbidden
        /// - Invalid data → 400 Bad Request (validation)
        /// 
        /// Response on Success (200 OK):
        /// {
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "tripName": "Updated Trip Name",
        ///     "updatedAt": "2025-01-06T15:30:00Z",
        ///     ...
        /// }
        /// 
        /// Example Usage:
        /// var request = new UpdateTripRequest
        /// {
        ///     TripName = "Updated Name",
        ///     Budget = 6000
        /// };
        /// var response = await _tripService.UpdateTripAsync(tripId, userId, request, ct);
        /// return Ok(response);  // 200 OK with updated trip
        /// </summary>
        Task<GetTripResponse> UpdateTripAsync(
            Guid tripId,
            Guid userId,
            UpdateTripRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing trip.
        /// 
        /// Business Logic:
        /// 1. Query database for trip with matching TripId
        /// 2. Verify that the requesting user owns the trip
        /// 3. If trip not found → throw EntityNotFoundException → 404
        /// 4. If user is not owner → throw ForbiddenException → 403
        /// 5. Delete trip via repository
        /// 6. Related entities (Travelers, ItineraryDays, Activities) are cascade deleted
        /// 7. Return no content (204 No Content)
        /// 
        /// Authorization:
        /// - Users can only delete trips they own
        /// 
        /// Cascade Behavior:
        /// - Deletes related Travelers
        /// - Deletes related ItineraryDays
        /// - Deletes related Activities
        /// 
        /// Error Handling:
        /// - Trip not found → 404 Not Found
        /// - User is not owner → 403 Forbidden
        /// 
        /// Response on Success (204 No Content):
        /// (empty response body)
        /// 
        /// Example Usage:
        /// await _tripService.DeleteTripAsync(tripId, userId, ct);
        /// return NoContent();  // 204 No Content
        /// </summary>
        Task DeleteTripAsync(
            Guid tripId,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
