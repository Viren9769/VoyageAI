using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Activities;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Controllers
{
    /// <summary>
    /// API controller for activity management operations.
    /// 
    /// This controller handles HTTP requests for activity CRUD operations within itinerary days.
    /// It is intentionally thin - all business logic resides in the service layer.
    /// 
    /// Architecture:
    /// HTTP Request → Validation (DataAnnotations + FluentValidation)
    ///     → ActivityController (routes to service)
    ///     → IActivityService (business logic, authorization)
    ///     → IActivityRepository (data access)
    ///     → Database
    /// 
    /// Exception Handling:
    /// Service methods throw exceptions which are caught by GlobalExceptionMiddleware:
    /// - EntityNotFoundException → 404 Not Found
    /// - ForbiddenException → 403 Forbidden
    /// - ValidationException → 400 Bad Request
    /// - ConflictException → 409 Conflict
    /// 
    /// HTTP Endpoints:
    /// - POST /api/trips/{tripId}/itinerary/{dayId}/activities - Create a new activity
    /// - GET /api/trips/{tripId}/itinerary/{dayId}/activities - Get all activities for a day
    /// - GET /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId} - Get a specific activity
    /// - PUT /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId} - Update an activity
    /// - DELETE /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId} - Delete an activity
    /// 
    /// Authentication:
    /// All endpoints require JWT Bearer token ([Authorize])
    /// User ID is extracted from JWT token claims (subject/sub claim)
    /// 
    /// Route Structure:
    /// Nested resource pattern:
    /// - /api/trips/{tripId} - Parent resource (trip)
    /// - /api/trips/{tripId}/itinerary/{dayId} - Child resource (itinerary day)
    /// - /api/trips/{tripId}/itinerary/{dayId}/activities - Grandchild resource (activities)
    /// 
    /// This enables:
    /// - Clear resource hierarchy
    /// - Proper authorization at each level (verify trip, then day, then activity)
    /// - Meaningful API endpoints
    /// 
    /// Design Principles:
    /// - No business logic in controller (belongs in service)
    /// - No database access in controller (belongs in repository)
    /// - Minimal error handling (complex cases handled by middleware)
    /// - All input validation uses DataAnnotations and FluentValidation
    /// - Clean separation of concerns
    /// - UserID always extracted from JWT, never from request
    /// </summary>
    [ApiController]
    [Route("api/trips/{tripId}/itinerary/{dayId}/activities")]
    [Authorize]
    public class ActivityController : ControllerBase
    {
        /// <summary>
        /// Service for activity business logic.
        /// Handles creation, retrieval, update, and deletion of activities.
        /// </summary>
        private readonly IActivityService _activityService;

        /// <summary>
        /// Logger for this controller.
        /// </summary>
        private readonly ILogger<ActivityController> _logger;

        /// <summary>
        /// Initializes a new instance of the ActivityController class.
        /// 
        /// Constructor Injection:
        /// All dependencies are injected by the DI container.
        /// </summary>
        public ActivityController(IActivityService activityService, ILogger<ActivityController> logger)
        {
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new activity for an itinerary day.
        /// 
        /// HTTP: POST /api/trips/{tripId}/itinerary/{dayId}/activities
        /// 
        /// Request Body:
        /// {
        ///     "activityName": "Visit Eiffel Tower",
        ///     "category": 0,
        ///     "startTime": "09:00:00",
        ///     "endTime": "11:30:00",
        ///     "priority": 3,
        ///     "locationName": "Eiffel Tower, Paris",
        ///     "latitude": 48.8584,
        ///     "longitude": 2.2945,
        ///     "estimatedCost": 15.00,
        ///     "description": "Ascend to the top of the iconic Eiffel Tower"
        /// }
        /// 
        /// Response (201 Created):
        /// {
        ///     "activityId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "dayId": "550e8400-e29b-41d4-a716-446655440001",
        ///     "activityName": "Visit Eiffel Tower",
        ///     "category": 0,
        ///     "startTime": "09:00:00",
        ///     "endTime": "11:30:00",
        ///     "priority": 3,
        ///     "status": 0,
        ///     "durationMinutes": 150,
        ///     "createdAt": "2025-01-06T14:30:00Z",
        ///     "updatedAt": "2025-01-06T14:30:00Z",
        ///     "isDeleted": false,
        ///     "deletedAt": null
        /// }
        /// 
        /// Error Responses:
        /// - 400 Bad Request: Validation failed or invalid time range
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip or itinerary day not found
        /// - 409 Conflict: Activity time conflicts with existing activities
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="dayId">The ID of the itinerary day</param>
        /// <param name="request">The activity creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created activity response</returns>
        [HttpPost]
        public async Task<ActionResult<ActivityResponse>> CreateActivity(
            Guid tripId,
            Guid dayId,
            [FromBody] CreateActivityRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"POST /api/trips/{tripId}/itinerary/{dayId}/activities - Creating activity");

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
            {
                _logger.LogWarning("User ID not found in JWT token");
                return Unauthorized();
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning($"Invalid user ID format in JWT token: {userIdClaim.Value}");
                return Unauthorized();
            }

            // Call service (exceptions are caught by GlobalExceptionMiddleware)
            var response = await _activityService.CreateActivityAsync(userId, tripId, dayId, request, cancellationToken);

            _logger.LogInformation("Activity created successfully");
            return Created($"/api/trips/{tripId}/itinerary/{dayId}/activities/{response.ActivityId}", response);
        }

        /// <summary>
        /// Retrieves all activities for an itinerary day.
        /// 
        /// HTTP: GET /api/trips/{tripId}/itinerary/{dayId}/activities
        /// 
        /// Response (200 OK):
        /// Returns array of activities:
        /// [
        ///     {
        ///         "activityId": "...",
        ///         "dayId": "...",
        ///         "activityName": "Visit Eiffel Tower",
        ///         "category": 0,
        ///         "startTime": "09:00:00",
        ///         "endTime": "11:30:00",
        ///         "priority": 3,
        ///         "status": 0,
        ///         "durationMinutes": 150,
        ///         "estimatedCost": 15.00,
        ///         "updatedAt": "2025-01-06T14:30:00Z",
        ///         "isDeleted": false
        ///     },
        ///     ...
        /// ]
        /// 
        /// Error Responses:
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip or itinerary day not found
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="dayId">The ID of the itinerary day</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of activities for the day</returns>
        [HttpGet]
        public async Task<ActionResult<List<ActivityResponse>>> GetActivities(
            Guid tripId,
            Guid dayId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"GET /api/trips/{tripId}/itinerary/{dayId}/activities - Retrieving activities");

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            // Call service (exceptions are caught by GlobalExceptionMiddleware)
            var response = await _activityService.GetActivitiesAsync(userId, tripId, dayId, cancellationToken);

            _logger.LogInformation("Activities retrieved successfully");
            return Ok(response);
        }

        /// <summary>
        /// Retrieves a specific activity.
        /// 
        /// HTTP: GET /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId}
        /// 
        /// Response (200 OK):
        /// {
        ///     "activityId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "dayId": "550e8400-e29b-41d4-a716-446655440001",
        ///     "activityName": "Visit Eiffel Tower",
        ///     "category": 0,
        ///     "startTime": "09:00:00",
        ///     "endTime": "11:30:00",
        ///     "priority": 3,
        ///     "status": 0,
        ///     "durationMinutes": 150,
        ///     "estimatedCost": 15.00,
        ///     "actualCost": 15.00,
        ///     "createdAt": "2025-01-06T14:30:00Z",
        ///     "updatedAt": "2025-01-06T14:30:00Z",
        ///     "createdBy": "...",
        ///     "lastModifiedBy": "...",
        ///     "isDeleted": false,
        ///     "deletedAt": null
        /// }
        /// 
        /// Error Responses:
        /// - 400 Bad Request: Activity doesn't belong to the day
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip, day, or activity not found
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="dayId">The ID of the itinerary day</param>
        /// <param name="activityId">The ID of the activity</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Activity details</returns>
        [HttpGet("{activityId}")]
        public async Task<ActionResult<ActivityResponse>> GetActivity(
            Guid tripId,
            Guid dayId,
            Guid activityId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"GET /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId} - Retrieving activity");

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            // Call service (exceptions are caught by GlobalExceptionMiddleware)
            var response = await _activityService.GetActivityAsync(userId, tripId, dayId, activityId, cancellationToken);

            _logger.LogInformation("Activity retrieved successfully");
            return Ok(response);
        }

        /// <summary>
        /// Updates an existing activity.
        /// 
        /// HTTP: PUT /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId}
        /// 
        /// Request Body (all fields optional for partial updates):
        /// {
        ///     "activityName": "Updated Activity Name",
        ///     "status": 1,
        ///     "priority": 2,
        ///     "startTime": "10:00:00",
        ///     "endTime": "12:00:00"
        /// }
        /// 
        /// Response (200 OK):
        /// Returns updated activity details with all fields
        /// 
        /// Error Responses:
        /// - 400 Bad Request: Validation failed, invalid time range, or activity doesn't belong to day
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip, day, or activity not found
        /// - 409 Conflict: Activity time conflicts with existing activities
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="dayId">The ID of the itinerary day</param>
        /// <param name="activityId">The ID of the activity</param>
        /// <param name="request">The activity update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated activity response</returns>
        [HttpPut("{activityId}")]
        public async Task<ActionResult<ActivityResponse>> UpdateActivity(
            Guid tripId,
            Guid dayId,
            Guid activityId,
            [FromBody] UpdateActivityRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"PUT /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId} - Updating activity");

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            // Call service (exceptions are caught by GlobalExceptionMiddleware)
            var response = await _activityService.UpdateActivityAsync(userId, tripId, dayId, activityId, request, cancellationToken);

            _logger.LogInformation("Activity updated successfully");
            return Ok(response);
        }

        /// <summary>
        /// Deletes an activity (hard delete - permanent removal).
        /// 
        /// HTTP: DELETE /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId}
        /// 
        /// Response (204 No Content):
        /// Empty response body.
        /// 
        /// Error Responses:
        /// - 400 Bad Request: Activity doesn't belong to the day
        /// - 403 Forbidden: User doesn't own the trip
        /// - 404 Not Found: Trip, day, or activity not found
        /// </summary>
        /// <param name="tripId">The ID of the trip</param>
        /// <param name="dayId">The ID of the itinerary day</param>
        /// <param name="activityId">The ID of the activity</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{activityId}")]
        public async Task<ActionResult> DeleteActivity(
            Guid tripId,
            Guid dayId,
            Guid activityId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"DELETE /api/trips/{tripId}/itinerary/{dayId}/activities/{activityId} - Deleting activity");

            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            // Call service (exceptions are caught by GlobalExceptionMiddleware)
            await _activityService.DeleteActivityAsync(userId, tripId, dayId, activityId, cancellationToken);

            _logger.LogInformation("Activity deleted successfully");
            return NoContent();
        }
    }
}
