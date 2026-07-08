using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyageAI.API.DTOs.Trip;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Controllers
{
    /// <summary>
    /// API controller for trip management operations.
    /// 
    /// This controller handles HTTP requests for trip CRUD operations.
    /// It is intentionally thin - all business logic resides in the service layer.
    /// 
    /// Architecture:
    /// HTTP Request → Validation (DataAnnotations + FluentValidation)
    ///     → TripController (routes to service)
    ///     → ITripService (business logic, authorization)
    ///     → ITripRepository (data access)
    ///     → Database
    /// 
    /// HTTP Endpoints:
    /// - GET /api/trips - Get all trips for authenticated user
    /// - GET /api/trips/{id} - Get a specific trip
    /// - POST /api/trips - Create a new trip
    /// - PUT /api/trips/{id} - Update a trip
    /// - DELETE /api/trips/{id} - Delete a trip
    /// 
    /// Authentication:
    /// All endpoints require JWT Bearer token ([Authorize])
    /// User ID is extracted from JWT token claims (subject/sub claim)
    /// 
    /// Design Principles:
    /// - No business logic in controller (belongs in service)
    /// - No database access in controller (belongs in repository)
    /// - Minimal error handling (complex cases handled by middleware)
    /// - Service calls are wrapped in try-catch for specific business exceptions
    /// - Clean separation of concerns
    /// 
    /// Dependencies:
    /// - ITripService: Injected via constructor dependency injection
    /// 
    /// Validation:
    /// - Input validation: DataAnnotations on DTOs
    /// - Complex validation: FluentValidation (IValidator{T})
    /// - Validation failures: 400 Bad Request (handled by middleware)
    /// 
    /// Error Handling:
    /// - EntityNotFoundException → 404 Not Found
    /// - ValidationException → 400 Bad Request
    /// - Other exceptions → 500 Internal Server Error (handled by middleware)
    /// 
    /// Authorization:
    /// All endpoints require [Authorize] attribute
    /// Users can only access their own trips (enforced in TripService)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // All endpoints require JWT authentication
    public class TripsController : ControllerBase
    {
        /// <summary>
        /// The trip service.
        /// Handles all business logic for trip operations.
        /// </summary>
        private readonly ITripService _tripService;

        /// <summary>
        /// Logger for this controller.
        /// </summary>
        private readonly ILogger<TripsController> _logger;

        /// <summary>
        /// Initializes a new instance of the TripsController class.
        /// 
        /// Constructor Injection:
        /// Dependencies are injected by the DI container.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped{ITripService, TripService}();
        /// </summary>
        /// <param name="tripService">The trip service for business logic</param>
        /// <param name="logger">Logger instance for this controller</param>
        public TripsController(ITripService tripService, ILogger<TripsController> logger)
        {
            _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all trips for the authenticated user.
        /// 
        /// HTTP Method: GET
        /// Route: GET /api/trips
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: Users see only their own trips
        /// 
        /// Query Parameters:
        /// None
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// 
        /// Response:
        /// 200 OK - Array of trip objects
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body Example (200 OK):
        /// [
        ///     {
        ///         "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///         "userId": "550e8400-e29b-41d4-a716-446655440001",
        ///         "tripName": "Summer Europe Adventure",
        ///         "destinationCountry": "France",
        ///         "destinationCity": "Paris",
        ///         "startDate": "2025-06-01T00:00:00Z",
        ///         "endDate": "2025-06-15T00:00:00Z",
        ///         "budget": 5000.00,
        ///         "currency": "USD",
        ///         "travelStyle": "Luxury",
        ///         "description": "Amazing trip to Paris",
        ///         "status": "Planning",
        ///         "createdAt": "2025-01-06T14:30:00Z",
        ///         "updatedAt": "2025-01-06T14:30:00Z",
        ///         "durationDays": 15
        ///     },
        ///     ...
        /// ]
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<GetTripResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetTripResponse>>> GetUserTrips(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var trips = await _tripService.GetUserTripsAsync(userId, cancellationToken);

                _logger.LogInformation("Retrieved {TripCount} trips for user {UserId}", trips.Count, userId);

                return Ok(trips);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trips");
                throw;
            }
        }

        /// <summary>
        /// Gets a specific trip by ID.
        /// 
        /// HTTP Method: GET
        /// Route: GET /api/trips/{id}
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: User must own the trip
        /// 
        /// Path Parameters:
        /// id (Guid) - The trip ID to retrieve
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// 
        /// Response:
        /// 200 OK - The requested trip object
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 404 Not Found - Trip not found or user doesn't own it
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body Example (200 OK):
        /// {
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "userId": "550e8400-e29b-41d4-a716-446655440001",
        ///     "tripName": "Summer Europe Adventure",
        ///     ...
        /// }
        /// 
        /// Response Body Example (404 Not Found):
        /// {
        ///     "error": "Trip with ID 550e8400-e29b-41d4-a716-446655440000 not found"
        /// }
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GetTripResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetTripResponse>> GetTripById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var trip = await _tripService.GetTripByIdAsync(id, userId, cancellationToken);

                _logger.LogInformation("Retrieved trip {TripId} for user {UserId}", id, userId);

                return Ok(trip);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trip {TripId}", id);
                throw;
            }
        }

        /// <summary>
        /// Creates a new trip for the authenticated user.
        /// 
        /// HTTP Method: POST
        /// Route: POST /api/trips
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: User is automatically associated (from JWT)
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// Content-Type: application/json
        /// 
        /// Request Body:
        /// {
        ///     "tripName": "Summer Europe Adventure",
        ///     "destinationCountry": "France",
        ///     "destinationCity": "Paris",
        ///     "startDate": "2025-06-01T00:00:00Z",
        ///     "endDate": "2025-06-15T00:00:00Z",
        ///     "budget": 5000.00,
        ///     "currency": "USD",
        ///     "travelStyle": "Luxury",
        ///     "description": "Amazing trip to Paris",
        ///     "coverImageUrl": "https://example.com/image.jpg",
        ///     "status": "Planning"
        /// }
        /// 
        /// Validation:
        /// - All required fields must be provided
        /// - Budget must be greater than 0
        /// - EndDate must be on or after StartDate
        /// - Dates must not be in the past
        /// 
        /// Response:
        /// 201 Created - Trip successfully created
        /// 400 Bad Request - Validation failed
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body Example (201 Created):
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
        ///     "description": "Amazing trip to Paris",
        ///     "coverImageUrl": "https://example.com/image.jpg",
        ///     "status": "Planning",
        ///     "createdAt": "2025-01-06T14:30:00Z",
        ///     "updatedAt": "2025-01-06T14:30:00Z",
        ///     "durationDays": 15
        /// }
        /// 
        /// Response Headers:
        /// Location: /api/trips/{tripId}
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(GetTripResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetTripResponse>> CreateTrip(
            CreateTripRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var response = await _tripService.CreateTripAsync(userId, request, cancellationToken);

                _logger.LogInformation("Created trip {TripId} for user {UserId}", response.TripId, userId);

                return Created($"/api/trips/{response.TripId}", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trip");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing trip with PARTIAL UPDATE support.
        /// 
        /// HTTP Method: PUT
        /// Route: PUT /api/trips/{id}
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: User must own the trip
        /// 
        /// ═══════════════════════════════════════════════════════════════════════════
        /// RECOMMENDED WORKFLOW FOR FRONTEND:
        /// ═══════════════════════════════════════════════════════════════════════════
        /// 
        /// Step 1: Get Current Trip Data (Pre-fill the edit form)
        /// ─────────────────────────────────────────────────────
        /// GET /api/trips/{tripId}
        /// 
        /// Step 2: Display Data in Edit Form
        /// ─────────────────────────────────────────────────────
        /// User sees all current values pre-filled in the form
        /// User modifies ONLY the fields they want to change
        /// Other fields remain as-is
        /// 
        /// Step 3: Send Only Changed Fields
        /// ─────────────────────────────────────────────────────
        /// PUT /api/trips/{tripId}
        /// 
        /// This endpoint with only the fields the user modified
        /// The API auto-merges your changes with existing data
        /// 
        /// ═══════════════════════════════════════════════════════════════════════════
        /// 
        /// Path Parameters:
        /// id (Guid) - The trip ID to update
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// Content-Type: application/json
        /// 
        /// Request Body (ALL FIELDS OPTIONAL - PARTIAL UPDATE):
        /// ────────────────────────────────────────────────────
        /// You can send ONLY the fields you want to update!
        /// Fields you don't include are automatically preserved from the database.
        /// 
        /// Example 1: Update only budget and status
        /// {
        ///     "budget": 6000.00,
        ///     "status": "Confirmed"
        /// }
        /// 
        /// Example 2: Update only trip name
        /// {
        ///     "tripName": "Updated Trip Name"
        /// }
        /// 
        /// Example 3: Update destination and description
        /// {
        ///     "destinationCountry": "Italy",
        ///     "destinationCity": "Rome",
        ///     "description": "Amazing trip to Rome and Vatican City"
        /// }
        /// 
        /// Validation:
        /// - All provided values must be valid
        /// - Budget must be greater than 0 (if provided)
        /// - Dates must be valid and EndDate >= StartDate (if provided)
        /// - URL formats must be valid (if provided)
        /// 
        /// Response:
        /// 200 OK - Trip successfully updated (returns full trip object)
        /// 400 Bad Request - Validation failed
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 404 Not Found - Trip not found or user doesn't own it
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body Example (200 OK):
        /// {
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "userId": "550e8400-e29b-41d4-a716-446655440001",
        ///     "tripName": "Summer Europe Adventure 2025",      // Unchanged
        ///     "destinationCountry": "France",                  // Unchanged
        ///     "destinationCity": "Paris",                      // Unchanged
        ///     "startDate": "2025-06-01T00:00:00Z",             // Unchanged
        ///     "endDate": "2025-06-15T00:00:00Z",               // Unchanged
        ///     "budget": 6000.00,                               // UPDATED ✓
        ///     "currency": "USD",                               // Unchanged
        ///     "travelStyle": "Luxury",                         // Unchanged
        ///     "status": "Confirmed",                           // UPDATED ✓
        ///     "description": "Amazing summer trip to Paris",   // Unchanged
        ///     "coverImageUrl": "https://example.com/image.jpg",// Unchanged
        ///     "createdAt": "2025-01-06T14:30:00Z",
        ///     "updatedAt": "2025-01-06T15:30:00Z",             // Auto-updated by server
        ///     "durationDays": 15
        /// }
        /// 
        /// KEY BENEFITS:
        /// ─────────────
        /// ✓ Fetch current data first (GET) before editing
        /// ✓ Pre-fill form with current values so user knows what's there
        /// ✓ User only needs to send changed fields in PUT request
        /// ✓ No need to re-enter fields you're not changing
        /// ✓ Less data transfer over the network
        /// ✓ Less error-prone - automatic merging prevents accidental overwrites
        /// ✓ Better UX - feels like a normal form edit experience
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(GetTripResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetTripResponse>> UpdateTrip(
            Guid id,
            UpdateTripRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var response = await _tripService.UpdateTripAsync(id, userId, request, cancellationToken);

                _logger.LogInformation("Updated trip {TripId} for user {UserId}", id, userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trip {TripId}", id);
                throw;
            }
        }

        /// <summary>
        /// Deletes an existing trip.
        /// 
        /// HTTP Method: DELETE
        /// Route: DELETE /api/trips/{id}
        /// Authentication: Required (JWT Bearer token)
        /// Authorization: User must own the trip
        /// 
        /// Path Parameters:
        /// id (Guid) - The trip ID to delete
        /// 
        /// Request Headers:
        /// Authorization: Bearer {jwt_token}
        /// 
        /// Response:
        /// 204 No Content - Trip successfully deleted
        /// 401 Unauthorized - Missing or invalid JWT token
        /// 404 Not Found - Trip not found or user doesn't own it
        /// 500 Internal Server Error - Server error
        /// 
        /// Response Body:
        /// (empty)
        /// 
        /// Cascade Behavior:
        /// Deleting a trip also deletes:
        /// - Related Travelers
        /// - Related ItineraryDays
        /// - Related Activities
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTrip(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserIdFromToken();
                await _tripService.DeleteTripAsync(id, userId, cancellationToken);

                _logger.LogInformation("Deleted trip {TripId} for user {UserId}", id, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting trip {TripId}", id);
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
