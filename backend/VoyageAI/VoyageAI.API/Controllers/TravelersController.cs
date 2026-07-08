using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyageAI.API.DTOs.Travelers;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Controllers
{
    /// <summary>
    /// API controller for traveler management operations.
    /// 
    /// This controller handles HTTP requests for traveler CRUD operations on trips.
    /// It is intentionally thin - all business logic resides in the service layer.
    /// 
    /// Architecture:
    /// HTTP Request → Validation (DataAnnotations + FluentValidation)
    ///     → TravelersController (routes to service)
    ///     → ITravelerService (business logic, authorization)
    ///     → ITravelerRepository (data access)
    ///     → Database
    /// 
    /// HTTP Endpoints:
    /// - GET /api/trips/{tripId}/travelers - Get all travelers for a trip
    /// - GET /api/trips/{tripId}/travelers/{travelerId} - Get a specific traveler
    /// - POST /api/trips/{tripId}/travelers - Create a new traveler
    /// - PUT /api/trips/{tripId}/travelers/{travelerId} - Update a traveler
    /// - DELETE /api/trips/{tripId}/travelers/{travelerId} - Delete a traveler
    /// 
    /// Authentication:
    /// All endpoints require JWT Bearer token ([Authorize])
    /// User ID is extracted from JWT token claims (sub claim)
    /// 
    /// Authorization:
    /// All endpoints verify that the authenticated user owns the trip
    /// Users can only manage travelers on trips they own
    /// 
    /// Design Principles:
    /// - No business logic in controller (belongs in service)
    /// - No database access in controller (belongs in repository)
    /// - Minimal error handling (complex cases handled by middleware)
    /// - Service calls are wrapped in try-catch for logging
    /// - Clean separation of concerns
    /// 
    /// Dependencies:
    /// - ITravelerService: Injected via constructor dependency injection
    /// 
    /// Validation:
    /// - Input validation: DataAnnotations on DTOs
    /// - Complex validation: FluentValidation (IValidator{T})
    /// - Validation failures: 400 Bad Request (handled by middleware)
    /// 
    /// Error Handling:
    /// - EntityNotFoundException → 404 Not Found
    /// - ForbiddenException → 403 Forbidden
    /// - ValidationException → 400 Bad Request or 409 Conflict
    /// - Other exceptions → 500 Internal Server Error (handled by middleware)
    /// </summary>
    [ApiController]
    [Route("api/trips/{tripId:guid}/travelers")]
    [Authorize]  // All endpoints require JWT authentication
    public class TravelersController : ControllerBase
    {
        /// <summary>
        /// The traveler service.
        /// Handles all business logic for traveler operations.
        /// </summary>
        private readonly ITravelerService _travelerService;

        /// <summary>
        /// Logger for this controller.
        /// </summary>
        private readonly ILogger<TravelersController> _logger;

        /// <summary>
        /// Initializes a new instance of the TravelersController class.
        /// 
        /// Constructor Injection:
        /// Dependencies are injected by the DI container.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped{ITravelerService, TravelerService}();
        /// </summary>
        /// <param name="travelerService">The traveler service for business logic</param>
        /// <param name="logger">Logger instance for this controller</param>
        public TravelersController(ITravelerService travelerService, ILogger<TravelersController> logger)
        {
            _travelerService = travelerService ?? throw new ArgumentNullException(nameof(travelerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all travelers for a specific trip.
        /// 
        /// HTTP Method: GET
        /// Route: GET /api/trips/{tripId}/travelers
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: User must own the trip
        /// 
        /// Path Parameters:
        /// tripId (Guid) - The trip ID
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// 
        /// Response:
        /// 200 OK - Array of traveler objects
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 403 Forbidden - User doesn't own the trip
        /// 404 Not Found - Trip not found
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body Example (200 OK):
        /// [
        ///     {
        ///         "travelerId": "550e8400-e29b-41d4-a716-446655440050",
        ///         "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///         "firstName": "John",
        ///         "lastName": "Doe",
        ///         "email": "john@example.com",
        ///         "isPrimaryTraveler": true,
        ///         "createdAt": "2025-01-06T14:30:00Z",
        ///         ...
        ///     },
        ///     {
        ///         "travelerId": "550e8400-e29b-41d4-a716-446655440051",
        ///         "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///         "firstName": "Jane",
        ///         "lastName": "Doe",
        ///         "email": "jane@example.com",
        ///         "isPrimaryTraveler": false,
        ///         ...
        ///     }
        /// ]
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<TravelerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TravelerResponse>>> GetTravelersByTrip(
            Guid tripId,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var travelers = await _travelerService.GetTravelersByTripAsync(userId, tripId, cancellationToken);

                _logger.LogInformation("Retrieved {TravelerCount} travelers for trip {TripId} by user {UserId}", 
                    travelers.Count, tripId, userId);

                return Ok(travelers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving travelers for trip {TripId}", tripId);
                throw;
            }
        }

        /// <summary>
        /// Gets a specific traveler by ID.
        /// 
        /// HTTP Method: GET
        /// Route: GET /api/trips/{tripId}/travelers/{travelerId}
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: User must own the trip
        /// 
        /// Path Parameters:
        /// tripId (Guid) - The trip ID
        /// travelerId (Guid) - The traveler ID
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// 
        /// Response:
        /// 200 OK - The requested traveler object
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 403 Forbidden - User doesn't own the trip
        /// 404 Not Found - Trip or traveler not found
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body Example (200 OK):
        /// {
        ///     "travelerId": "550e8400-e29b-41d4-a716-446655440050",
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "firstName": "John",
        ///     "lastName": "Doe",
        ///     "email": "john@example.com",
        ///     "dateOfBirth": "1985-05-15T00:00:00Z",
        ///     "passportNumber": "12345678",
        ///     "passportExpiry": "2030-12-31T00:00:00Z",
        ///     "isPrimaryTraveler": true,
        ///     "createdAt": "2025-01-06T14:30:00Z",
        ///     "updatedAt": "2025-01-06T14:30:00Z",
        ///     "age": 39,
        ///     "daysUntilPassportExpiry": 2151,
        ///     "isPassportValid": true,
        ///     ...
        /// }
        /// </summary>
        [HttpGet("{travelerId:guid}")]
        [ProducesResponseType(typeof(TravelerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TravelerResponse>> GetTravelerById(
            Guid tripId,
            Guid travelerId,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var traveler = await _travelerService.GetTravelerByIdAsync(userId, tripId, travelerId, cancellationToken);

                _logger.LogInformation("Retrieved traveler {TravelerId} from trip {TripId} by user {UserId}", 
                    travelerId, tripId, userId);

                return Ok(traveler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving traveler {TravelerId} from trip {TripId}", travelerId, tripId);
                throw;
            }
        }

        /// <summary>
        /// Creates a new traveler for a trip.
        /// 
        /// HTTP Method: POST
        /// Route: POST /api/trips/{tripId}/travelers
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: User must own the trip
        /// 
        /// Path Parameters:
        /// tripId (Guid) - The trip ID
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// Content-Type: application/json
        /// 
        /// Request Body:
        /// {
        ///     "firstName": "John",
        ///     "lastName": "Doe",
        ///     "email": "john@example.com",
        ///     "phone": "+1-555-1234",
        ///     "dateOfBirth": "1985-05-15T00:00:00Z",
        ///     "gender": "Male",
        ///     "nationality": "United States",
        ///     "passportNumber": "12345678",
        ///     "passportCountry": "United States",
        ///     "passportExpiry": "2030-12-31T00:00:00Z",
        ///     "emergencyContactName": "Jane Doe",
        ///     "emergencyContactPhone": "+1-555-5678",
        ///     "relationship": "Spouse",
        ///     "dietaryPreference": "Vegetarian",
        ///     "specialRequirements": "Window seat preference",
        ///     "frequentFlyerNumber": "AA123456789",
        ///     "knownTravelerNumber": "12345678901",
        ///     "isPrimaryTraveler": true
        /// }
        /// 
        /// Validation:
        /// - FirstName: Required, 1-100 characters
        /// - LastName: Required, 1-100 characters
        /// - Email: Optional but valid format if provided
        /// - DateOfBirth: Optional, cannot be future, max age 120 years
        /// - PassportExpiry: Optional, cannot be in the past
        /// 
        /// Response:
        /// 201 Created - Traveler successfully created
        /// 400 Bad Request - Validation failed
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 403 Forbidden - User doesn't own the trip
        /// 404 Not Found - Trip not found
        /// 409 Conflict - Email already exists for this trip
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body Example (201 Created):
        /// {
        ///     "travelerId": "550e8400-e29b-41d4-a716-446655440050",
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "firstName": "John",
        ///     "lastName": "Doe",
        ///     "email": "john@example.com",
        ///     "dateOfBirth": "1985-05-15T00:00:00Z",
        ///     "passportNumber": "12345678",
        ///     "passportExpiry": "2030-12-31T00:00:00Z",
        ///     "isPrimaryTraveler": true,
        ///     "createdAt": "2025-01-06T14:30:00Z",
        ///     "updatedAt": "2025-01-06T14:30:00Z",
        ///     ...
        /// }
        /// 
        /// Response Headers:
        /// Location: /api/trips/{tripId}/travelers/{travelerId}
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TravelerResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TravelerResponse>> CreateTraveler(
            Guid tripId,
            CreateTravelerRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var response = await _travelerService.CreateTravelerAsync(userId, tripId, request, cancellationToken);

                _logger.LogInformation("Created traveler {TravelerId} in trip {TripId} by user {UserId}", 
                    response.TravelerId, tripId, userId);

                return Created($"/api/trips/{tripId}/travelers/{response.TravelerId}", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating traveler for trip {TripId}", tripId);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing traveler (PARTIAL UPDATE supported).
        /// 
        /// HTTP Method: PUT
        /// Route: PUT /api/trips/{tripId}/travelers/{travelerId}
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: User must own the trip
        /// 
        /// Path Parameters:
        /// tripId (Guid) - The trip ID
        /// travelerId (Guid) - The traveler ID
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// Content-Type: application/json
        /// 
        /// Request Body (only include fields to update):
        /// {
        ///     "email": "newemail@example.com",
        ///     "phone": "+1-555-9999",
        ///     "dateOfBirth": "1985-05-16T00:00:00Z"
        /// }
        /// 
        /// Note:
        /// This endpoint supports PARTIAL UPDATES.
        /// Only include the fields you want to change.
        /// Fields not included in the request will not be modified.
        /// 
        /// Validation:
        /// - Same rules as CreateTravelerRequest for provided fields
        /// - Omitted fields are not validated
        /// - Email must be valid if provided
        /// - No duplicate emails in same trip
        /// 
        /// Response:
        /// 200 OK - Traveler successfully updated
        /// 400 Bad Request - Validation failed
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 403 Forbidden - User doesn't own the trip
        /// 404 Not Found - Trip or traveler not found
        /// 409 Conflict - Email already exists for this trip
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body Example (200 OK):
        /// {
        ///     "travelerId": "550e8400-e29b-41d4-a716-446655440050",
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "firstName": "John",
        ///     "lastName": "Doe",
        ///     "email": "newemail@example.com",  // Updated
        ///     "phone": "+1-555-9999",  // Updated
        ///     "dateOfBirth": "1985-05-16T00:00:00Z",  // Updated
        ///     "passportNumber": "12345678",  // Unchanged
        ///     "isPrimaryTraveler": true,  // Unchanged
        ///     "updatedAt": "2025-01-06T15:30:00Z",  // New timestamp
        ///     ...
        /// }
        /// </summary>
        [HttpPut("{travelerId:guid}")]
        [ProducesResponseType(typeof(TravelerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TravelerResponse>> UpdateTraveler(
            Guid tripId,
            Guid travelerId,
            UpdateTravelerRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var response = await _travelerService.UpdateTravelerAsync(userId, tripId, travelerId, request, cancellationToken);

                _logger.LogInformation("Updated traveler {TravelerId} in trip {TripId} by user {UserId}", 
                    travelerId, tripId, userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating traveler {TravelerId} in trip {TripId}", travelerId, tripId);
                throw;
            }
        }

        /// <summary>
        /// Deletes a traveler (soft delete).
        /// 
        /// HTTP Method: DELETE
        /// Route: DELETE /api/trips/{tripId}/travelers/{travelerId}
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: User must own the trip
        /// 
        /// Path Parameters:
        /// tripId (Guid) - The trip ID
        /// travelerId (Guid) - The traveler ID
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// 
        /// Response:
        /// 204 No Content - Traveler successfully deleted
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 403 Forbidden - User doesn't own the trip
        /// 404 Not Found - Trip or traveler not found
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body:
        /// (empty - 204 No Content)
        /// 
        /// Note:
        /// This uses SOFT DELETE - the traveler record is marked as deleted
        /// but remains in the database. This maintains referential integrity
        /// and allows for recovery if needed.
        /// </summary>
        [HttpDelete("{travelerId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTraveler(
            Guid tripId,
            Guid travelerId,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                await _travelerService.DeleteTravelerAsync(userId, tripId, travelerId, cancellationToken);

                _logger.LogInformation("Deleted traveler {TravelerId} from trip {TripId} by user {UserId}", 
                    travelerId, tripId, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting traveler {TravelerId} from trip {TripId}", travelerId, tripId);
                throw;
            }
        }

        /// <summary>
        /// Extracts the user ID from the JWT token's subject claim.
        /// 
        /// JWT tokens contain user information in claims.
        /// The subject claim ("sub") typically contains the user ID.
        /// 
        /// Returns:
        /// The user ID as a Guid
        /// 
        /// Throws:
        /// InvalidOperationException if user ID is not found in token
        /// </summary>
        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("sub");

            if (userIdClaim?.Value == null)
            {
                _logger.LogError("User ID claim not found in JWT token");
                throw new InvalidOperationException("User ID not found in token");
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogError("Invalid user ID format in JWT token: {UserId}", userIdClaim.Value);
                throw new InvalidOperationException("Invalid user ID format in token");
            }

            return userId;
        }
    }
}
