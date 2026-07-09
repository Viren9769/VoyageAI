using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Itinerary;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Controllers
{
    /// <summary>
    /// API controller for itinerary day management operations.
    /// 
    /// This controller handles HTTP requests for itinerary day CRUD operations.
    /// It is intentionally thin - all business logic resides in the service layer.
    /// 
    /// Architecture:
    /// HTTP Request → Validation (DataAnnotations + FluentValidation)
    ///     → ItineraryController (routes to service)
    ///     → IItineraryService (business logic, authorization)
    ///     → IItineraryRepository (data access)
    ///     → Database
    /// 
    /// HTTP Endpoints:
    /// - POST /api/trips/{tripId}/itinerary - Create a new itinerary day
    /// - GET /api/trips/{tripId}/itinerary - Get all days for a trip
    /// - GET /api/trips/{tripId}/itinerary/{dayId} - Get a specific day
    /// - PUT /api/trips/{tripId}/itinerary/{dayId} - Update a day
    /// - DELETE /api/trips/{tripId}/itinerary/{dayId} - Delete a day
    /// 
    /// Authentication:
    /// All endpoints require JWT Bearer token ([Authorize])
    /// User ID is extracted from JWT token claims (subject/sub claim)
    /// 
    /// Design Principles:
    /// - No business logic in controller (belongs in service)
    /// - No database access in controller (belongs in repository)
    /// - Minimal error handling (complex cases handled by middleware)
    /// - All input validation uses DataAnnotations and FluentValidation
    /// - Clean separation of concerns
    /// - UserID always extracted from JWT, never from request
    /// 
    /// Dependencies:
    /// - IItineraryService: Injected via constructor dependency injection
    /// 
    /// Validation:
    /// - Input validation: DataAnnotations on DTOs
    /// - Complex validation: FluentValidation (IValidator{T})
    /// - Validation failures: 400 Bad Request (handled by middleware)
    /// 
    /// Error Handling:
    /// - EntityNotFoundException → 404 Not Found
    /// - ForbiddenException → 403 Forbidden
    /// - ValidationException → 400 Bad Request
    /// - Other exceptions → 500 Internal Server Error (handled by middleware)
    /// </summary>
    [ApiController]
    [Route("api/trips/{tripId}/itinerary")]
    [Authorize]
    public class ItineraryController : ControllerBase
    {
        /// <summary>
        /// Service for itinerary day business logic.
        /// Handles creation, retrieval, update, and deletion of itinerary days.
        /// </summary>
        private readonly IItineraryService _itineraryService;

        /// <summary>
        /// Logger for this controller.
        /// </summary>
        private readonly ILogger<ItineraryController> _logger;

        /// <summary>
        /// Initializes a new instance of the ItineraryController class.
        /// 
        /// Constructor Injection:
        /// All dependencies are injected by the DI container.
        /// </summary>
        public ItineraryController(IItineraryService itineraryService, ILogger<ItineraryController> logger)
        {
            _itineraryService = itineraryService ?? throw new ArgumentNullException(nameof(itineraryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new itinerary day for a trip.
        /// 
        /// HTTP: POST /api/trips/{tripId}/itinerary
        /// 
        /// Request Body:
        /// {
        ///     "dayNumber": 1,
        ///     "date": "2025-06-01T00:00:00Z",
        ///     "title": "Arrival in Paris",
        ///     "summary": "Arrive at CDG airport",
        ///     "notes": "Pick up rental car",
        ///     "estimatedBudget": 200.0,
        ///     "actualBudget": 0.0,
        ///     "weatherSummary": "Sunny, 22°C"
        /// }
        /// 
        /// Response (201 Created):
        /// {
        ///     "data": { /* ItineraryDayResponse with audit fields */ },
        ///     "success": true,
        ///     "message": "Itinerary day created successfully"
        /// }
        /// 
        /// Error Responses:
        /// - 400 Bad Request: Validation failed or date outside trip range
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip not found
        /// - 409 Conflict: Day number already exists
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="request">The itinerary day creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created itinerary day response</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ItineraryDayResponse>>> CreateItineraryDay(
            Guid tripId,
            [FromBody] CreateItineraryDayRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("POST /api/trips/{TripId}/itinerary - Creating itinerary day", tripId);

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
            {
                _logger.LogWarning("User ID not found in JWT token");
                return Unauthorized(new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = "User ID not found in token"
                });
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Invalid user ID format in JWT token: {UserIdClaim}", userIdClaim.Value);
                return Unauthorized(new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = "Invalid user ID format"
                });
            }

            // Call service
            var response = await _itineraryService.CreateItineraryDayAsync(userId, tripId, request, cancellationToken);

            // Return appropriate status code based on response
            if (response.Success)
            {
                _logger.LogInformation("Itinerary day created successfully");
                return Created($"/api/trips/{tripId}/itinerary/{response.Data?.DayId}", response);
            }

            // Service returns error response with proper message
            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            if (response.Message?.Contains("permission", StringComparison.OrdinalIgnoreCase) == true)
                return Forbid();

            if (response.Message?.Contains("already exists", StringComparison.OrdinalIgnoreCase) == true)
                return Conflict(response);

            return BadRequest(response);
        }

        /// <summary>
        /// Retrieves all itinerary days for a trip.
        /// 
        /// HTTP: GET /api/trips/{tripId}/itinerary
        /// 
        /// Response (200 OK):
        /// {
        ///     "data": [
        ///         {
        ///             "dayId": "...",
        ///             "tripId": "...",
        ///             "dayNumber": 1,
        ///             "date": "2025-06-01T00:00:00Z",
        ///             "title": "Arrival in Paris",
        ///             "summary": "Arrive at CDG airport",
        ///             "estimatedBudget": 200.0,
        ///             "actualBudget": 150.0,
        ///             "weatherSummary": "Sunny",
        ///             "updatedAt": "2025-01-06T14:30:00Z",
        ///             "isDeleted": false
        ///         },
        ///         ...
        ///     ],
        ///     "success": true,
        ///     "message": "Itinerary days retrieved successfully"
        /// }
        /// 
        /// Error Responses:
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip not found
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of itinerary days for the trip</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ItinerarySummaryResponse>>>> GetItineraryDays(
            Guid tripId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/trips/{TripId}/itinerary - Getting itinerary days", tripId);

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
            {
                _logger.LogWarning("User ID not found in JWT token");
                return Unauthorized(new ApiResponse<List<ItinerarySummaryResponse>>
                {
                    Success = false,
                    Message = "User ID not found in token"
                });
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Invalid user ID format in JWT token: {UserIdClaim}", userIdClaim.Value);
                return Unauthorized(new ApiResponse<List<ItinerarySummaryResponse>>
                {
                    Success = false,
                    Message = "Invalid user ID format"
                });
            }

            // Call service
            var response = await _itineraryService.GetItineraryDaysAsync(userId, tripId, cancellationToken);

            // Return appropriate status code based on response
            if (response.Success)
            {
                _logger.LogInformation("Retrieved {Count} itinerary days", response.Data?.Count ?? 0);
                return Ok(response);
            }

            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            if (response.Message?.Contains("permission", StringComparison.OrdinalIgnoreCase) == true)
                return Forbid();

            return BadRequest(response);
        }

        /// <summary>
        /// Retrieves a specific itinerary day.
        /// 
        /// HTTP: GET /api/trips/{tripId}/itinerary/{dayId}
        /// 
        /// Response (200 OK):
        /// {
        ///     "data": {
        ///         "dayId": "...",
        ///         "tripId": "...",
        ///         "dayNumber": 1,
        ///         "date": "2025-06-01T00:00:00Z",
        ///         "title": "Arrival in Paris",
        ///         "summary": "Arrive at CDG airport",
        ///         "notes": "Pick up rental car at Terminal 2C",
        ///         "estimatedBudget": 200.0,
        ///         "actualBudget": 150.0,
        ///         "weatherSummary": "Sunny, 22°C",
        ///         "createdAt": "2025-01-06T14:30:00Z",
        ///         "updatedAt": "2025-01-06T14:30:00Z",
        ///         "createdBy": "...",
        ///         "lastModifiedBy": "...",
        ///         "isDeleted": false,
        ///         "deletedAt": null
        ///     },
        ///     "success": true,
        ///     "message": "Itinerary day retrieved successfully"
        /// }
        /// 
        /// Error Responses:
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip or day not found
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="dayId">The ID of the itinerary day</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The specific itinerary day</returns>
        [HttpGet("{dayId}")]
        public async Task<ActionResult<ApiResponse<ItineraryDayResponse>>> GetItineraryDay(
            Guid tripId,
            Guid dayId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/trips/{TripId}/itinerary/{DayId} - Getting itinerary day", tripId, dayId);

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
            {
                _logger.LogWarning("User ID not found in JWT token");
                return Unauthorized(new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = "User ID not found in token"
                });
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Invalid user ID format in JWT token: {UserIdClaim}", userIdClaim.Value);
                return Unauthorized(new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = "Invalid user ID format"
                });
            }

            // Call service
            var response = await _itineraryService.GetItineraryDayAsync(userId, tripId, dayId, cancellationToken);

            // Return appropriate status code based on response
            if (response.Success)
            {
                _logger.LogInformation("Retrieved itinerary day {DayId}", dayId);
                return Ok(response);
            }

            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            if (response.Message?.Contains("permission", StringComparison.OrdinalIgnoreCase) == true)
                return Forbid();

            return BadRequest(response);
        }

        /// <summary>
        /// Updates an existing itinerary day.
        /// 
        /// HTTP: PUT /api/trips/{tripId}/itinerary/{dayId}
        /// 
        /// Request Body:
        /// {
        ///     "dayNumber": 1,
        ///     "date": "2025-06-01T00:00:00Z",
        ///     "title": "Arrival in Paris - Updated",
        ///     "summary": "Arrive at CDG airport and check in",
        ///     "notes": "Pick up rental car updated info",
        ///     "estimatedBudget": 250.0,
        ///     "actualBudget": 200.0,
        ///     "weatherSummary": "Sunny, 24°C, possible afternoon rain"
        /// }
        /// 
        /// Response (200 OK):
        /// {
        ///     "data": { /* Updated ItineraryDayResponse */ },
        ///     "success": true,
        ///     "message": "Itinerary day updated successfully"
        /// }
        /// 
        /// Error Responses:
        /// - 400 Bad Request: Validation failed or date outside trip range
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip or day not found
        /// - 409 Conflict: Day number already exists
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="dayId">The ID of the itinerary day to update</param>
        /// <param name="request">The update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated itinerary day response</returns>
        [HttpPut("{dayId}")]
        public async Task<ActionResult<ApiResponse<ItineraryDayResponse>>> UpdateItineraryDay(
            Guid tripId,
            Guid dayId,
            [FromBody] UpdateItineraryDayRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("PUT /api/trips/{TripId}/itinerary/{DayId} - Updating itinerary day", tripId, dayId);

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
            {
                _logger.LogWarning("User ID not found in JWT token");
                return Unauthorized(new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = "User ID not found in token"
                });
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Invalid user ID format in JWT token: {UserIdClaim}", userIdClaim.Value);
                return Unauthorized(new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = "Invalid user ID format"
                });
            }

            // Call service
            var response = await _itineraryService.UpdateItineraryDayAsync(userId, tripId, dayId, request, cancellationToken);

            // Return appropriate status code based on response
            if (response.Success)
            {
                _logger.LogInformation("Itinerary day updated successfully");
                return Ok(response);
            }

            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            if (response.Message?.Contains("permission", StringComparison.OrdinalIgnoreCase) == true)
                return Forbid();

            if (response.Message?.Contains("already exists", StringComparison.OrdinalIgnoreCase) == true)
                return Conflict(response);

            return BadRequest(response);
        }

        /// <summary>
        /// Deletes an itinerary day.
        /// 
        /// HTTP: DELETE /api/trips/{tripId}/itinerary/{dayId}
        /// 
        /// Response (204 No Content):
        /// Empty response body.
        /// 
        /// Error Responses:
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip or day not found
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="dayId">The ID of the itinerary day to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{dayId}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteItineraryDay(
            Guid tripId,
            Guid dayId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("DELETE /api/trips/{TripId}/itinerary/{DayId} - Deleting itinerary day", tripId, dayId);

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
            {
                _logger.LogWarning("User ID not found in JWT token");
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User ID not found in token"
                });
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Invalid user ID format in JWT token: {UserIdClaim}", userIdClaim.Value);
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid user ID format"
                });
            }

            // Call service
            var response = await _itineraryService.DeleteItineraryDayAsync(userId, tripId, dayId, cancellationToken);

            // Return appropriate status code based on response
            if (response.Success)
            {
                _logger.LogInformation("Itinerary day deleted successfully");
                return NoContent();
            }

            if (response.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(response);

            if (response.Message?.Contains("permission", StringComparison.OrdinalIgnoreCase) == true)
                return Forbid();

            return BadRequest(response);
        }
    }
}
