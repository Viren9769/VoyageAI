using AutoMapper;
using VoyageAI.API.Common.Exceptions;
using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Activities;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Models.Enums;
using VoyageAI.API.Repositories.Interfaces;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Services
{
    /// <summary>
    /// Service implementation for activity operations.
    /// 
    /// This class implements all business logic for activity management.
    /// It handles creation, retrieval, update, and deletion with proper authorization.
    /// 
    /// Design Pattern: Service Layer
    /// - Encapsulates all activity business logic
    /// - Depends on interfaces (IActivityRepository, ITripRepository, IMapper) for loose coupling
    /// - Enables unit testing through dependency injection
    /// - Centralizes authorization checks
    /// - Throws exceptions which are caught by GlobalExceptionMiddleware (no ApiResponse return)
    /// 
    /// Dependencies:
    /// - IActivityRepository: Data access for Activity entity
    /// - IItineraryRepository: Data access for ItineraryDay entity
    /// - ITripRepository: Data access for Trip entity (for ownership verification)
    /// - IMapper: AutoMapper for DTO conversions
    /// - ILogger: Logging for debugging and monitoring
    /// 
    /// Security Features:
    /// - Authorization checks on all operations (users can only access their own activities)
    /// - Validates day belongs to trip
    /// - Validates activity belongs to day
    /// - Checks for time conflicts between activities
    /// - Proper error messages without information leakage
    /// - Audit fields track who created/modified each activity
    /// 
    /// Business Rules:
    /// - Each Activity belongs to exactly ONE ItineraryDay
    /// - Status defaults to Planned when creating
    /// - End time must be after start time
    /// - Time conflicts with other activities are detected
    /// - Duration is auto-calculated from start/end times
    /// - Only trip owner can manage activities
    /// - Hard delete: activities are permanently removed when deleted
    /// 
    /// Exception Handling:
    /// - EntityNotFoundException: Thrown when entity not found (404 response by middleware)
    /// - ForbiddenException: Thrown when user lacks permission (403 response by middleware)
    /// - ValidationException: Thrown on validation failures (400 response by middleware)
    /// - ConflictException: Thrown on conflicts like time overlap (409 response by middleware)
    /// 
    /// Dependency Injection:
    /// services.AddScoped<IActivityService, ActivityService>();
    /// </summary>
    public class ActivityService : IActivityService
    {
        /// <summary>
        /// Repository for activity data access operations.
        /// Handles CREATE, READ, UPDATE, DELETE operations for Activity entity.
        /// </summary>
        private readonly IActivityRepository _activityRepository;

        /// <summary>
        /// Repository for itinerary day data access operations.
        /// Used to verify itinerary day exists and validate ownership chain.
        /// </summary>
        private readonly IItineraryRepository _itineraryRepository;

        /// <summary>
        /// Repository for trip data access operations.
        /// Used to verify trip ownership before allowing activity operations.
        /// </summary>
        private readonly ITripRepository _tripRepository;

        /// <summary>
        /// AutoMapper instance for mapping between DTOs and entities.
        /// Maps CreateActivityRequest/UpdateActivityRequest DTOs to Activity entity and back.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Generic logger for this service.
        /// Used to log important events: activity creation, updates, authorization failures, errors.
        /// </summary>
        private readonly ILogger<ActivityService> _logger;

        /// <summary>
        /// Initializes a new instance of the ActivityService class.
        /// 
        /// Constructor Injection:
        /// All dependencies are injected by the DI container.
        /// This enables loose coupling and testability.
        /// </summary>
        public ActivityService(
            IActivityRepository activityRepository,
            IItineraryRepository itineraryRepository,
            ITripRepository tripRepository,
            IMapper mapper,
            ILogger<ActivityService> logger)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _itineraryRepository = itineraryRepository ?? throw new ArgumentNullException(nameof(itineraryRepository));
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new activity for an itinerary day.
        /// </summary>
        public async Task<ActivityResponse> CreateActivityAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            CreateActivityRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Creating activity for day {dayId} by user {userId}");

            // 1. Verify user owns the trip
            var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
            if (trip == null)
            {
                _logger.LogWarning($"Trip {tripId} not found");
                throw new EntityNotFoundException("Trip not found");
            }

            if (trip.UserId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to access trip {tripId} they don't own");
                throw new ForbiddenException("You don't have permission to access this trip");
            }

            // 2. Verify itinerary day exists and belongs to trip
            var day = await _itineraryRepository.GetByIdAsync(dayId, cancellationToken);
            if (day == null)
            {
                _logger.LogWarning($"Itinerary day {dayId} not found");
                throw new EntityNotFoundException("Itinerary day not found");
            }

            if (day.TripId != tripId)
            {
                _logger.LogWarning($"Itinerary day {dayId} does not belong to trip {tripId}");
                throw new ValidationException("Itinerary day does not belong to this trip");
            }

            // 3. Validate time range (end time > start time)
            if (request.EndTime <= request.StartTime)
            {
                throw new ValidationException("End time must be after start time");
            }

            // 4. Check for time conflicts with existing activities
            var conflicts = await _activityRepository.GetActivitiesInTimeRangeAsync(
                dayId,
                request.StartTime,
                request.EndTime,
                null,
                cancellationToken);

            if (conflicts.Any())
            {
                throw new ActivityTimeConflictException("Activity time conflicts with existing activities. Please choose a different time slot.");
            }

            // 5. Map request to entity
            var activity = _mapper.Map<Activity>(request);

            // 6. Set required fields not in request
            activity.ActivityId = Guid.NewGuid();
            activity.DayId = dayId;
            activity.CreatedAt = DateTime.UtcNow;
            activity.UpdatedAt = DateTime.UtcNow;
            activity.CreatedBy = userId;
            activity.LastModifiedBy = userId;
            activity.IsDeleted = false;

            // 7. Set default status if not provided
            if (request.Status.HasValue)
            {
                activity.Status = request.Status.Value;
            }
            else
            {
                activity.Status = (int)ActivityStatus.Planned;
            }

            // 8. Calculate duration in minutes
            var startMinutes = request.StartTime.Hour * 60 + request.StartTime.Minute;
            var endMinutes = request.EndTime.Hour * 60 + request.EndTime.Minute;
            activity.DurationMinutes = endMinutes - startMinutes;

            // 9. Persist to database
            await _activityRepository.CreateAsync(activity, cancellationToken);
            await _activityRepository.SaveChangesAsync(cancellationToken);

            // 10. Map to response
            var response = _mapper.Map<ActivityResponse>(activity);

            _logger.LogInformation($"Activity {activity.ActivityId} created successfully for day {dayId} by user {userId}");
            return response;
        }

        /// <summary>
        /// Retrieves all activities for an itinerary day.
        /// </summary>
        public async Task<List<ActivityResponse>> GetActivitiesAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Retrieving activities for day {dayId} by user {userId}");

            // 1. Verify user owns the trip
            var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
            if (trip == null)
            {
                throw new EntityNotFoundException("Trip not found");
            }

            if (trip.UserId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to access trip {tripId} they don't own");
                throw new ForbiddenException("You don't have permission to access this trip");
            }

            // 2. Verify itinerary day exists and belongs to trip
            var day = await _itineraryRepository.GetByIdAsync(dayId, cancellationToken);
            if (day == null)
            {
                throw new EntityNotFoundException("Itinerary day not found");
            }

            if (day.TripId != tripId)
            {
                throw new ValidationException("Itinerary day does not belong to this trip");
            }

            // 3. Get all activities for the day
            var activities = await _activityRepository.GetByDayIdAsync(dayId, cancellationToken);

            // 4. Map to responses
            var responses = _mapper.Map<List<ActivityResponse>>(activities);

            return responses;
        }

        /// <summary>
        /// Retrieves a specific activity.
        /// </summary>
        public async Task<ActivityResponse> GetActivityAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            Guid activityId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Retrieving activity {activityId} by user {userId}");

            // 1. Verify user owns the trip
            var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
            if (trip == null)
            {
                throw new EntityNotFoundException("Trip not found");
            }

            if (trip.UserId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to access trip {tripId} they don't own");
                throw new ForbiddenException("You don't have permission to access this trip");
            }

            // 2. Verify itinerary day exists and belongs to trip
            var day = await _itineraryRepository.GetByIdAsync(dayId, cancellationToken);
            if (day == null)
            {
                throw new EntityNotFoundException("Itinerary day not found");
            }

            if (day.TripId != tripId)
            {
                throw new ValidationException("Itinerary day does not belong to this trip");
            }

            // 3. Get activity
            var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);
            if (activity == null)
            {
                throw new EntityNotFoundException("Activity not found");
            }

            // 4. Verify activity belongs to the day
            if (activity.DayId != dayId)
            {
                throw new ValidationException("Activity does not belong to the specified itinerary day");
            }

            // 5. Map to response
            var response = _mapper.Map<ActivityResponse>(activity);

            return response;
        }

        /// <summary>
        /// Updates an existing activity.
        /// </summary>
        public async Task<ActivityResponse> UpdateActivityAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            Guid activityId,
            UpdateActivityRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Updating activity {activityId} by user {userId}");

            // 1. Verify user owns the trip
            var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
            if (trip == null)
            {
                throw new EntityNotFoundException("Trip not found");
            }

            if (trip.UserId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to access trip {tripId} they don't own");
                throw new ForbiddenException("You don't have permission to access this trip");
            }

            // 2. Verify itinerary day exists and belongs to trip
            var day = await _itineraryRepository.GetByIdAsync(dayId, cancellationToken);
            if (day == null)
            {
                throw new EntityNotFoundException("Itinerary day not found");
            }

            if (day.TripId != tripId)
            {
                throw new ValidationException("Itinerary day does not belong to this trip");
            }

            // 3. Get activity
            var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);
            if (activity == null)
            {
                throw new EntityNotFoundException("Activity not found");
            }

            // 4. Verify activity belongs to the day
            if (activity.DayId != dayId)
            {
                throw new ValidationException("Activity does not belong to the specified itinerary day");
            }

            // 5. Validate time range if times are being updated
            if (request.StartTime.HasValue && request.EndTime.HasValue)
            {
                if (request.EndTime <= request.StartTime)
                {
                    throw new ValidationException("End time must be after start time");
                }
            }
            else if (request.EndTime.HasValue)
            {
                if (request.EndTime <= activity.StartTime)
                {
                    throw new ValidationException("End time must be after start time");
                }
            }
            else if (request.StartTime.HasValue)
            {
                if (activity.EndTime <= request.StartTime)
                {
                    throw new ValidationException("End time must be after start time");
                }
            }

            // 6. Check for time conflicts if times are being updated
            var newStartTime = request.StartTime ?? activity.StartTime;
            var newEndTime = request.EndTime ?? activity.EndTime;

            if (request.StartTime.HasValue || request.EndTime.HasValue)
            {
                var conflicts = await _activityRepository.GetActivitiesInTimeRangeAsync(
                    dayId,
                    newStartTime,
                    newEndTime,
                    activityId, // Exclude current activity
                    cancellationToken);

                if (conflicts.Any())
                {
                    throw new ActivityTimeConflictException("Activity time conflicts with existing activities. Please choose a different time slot.");
                }
            }

            // 7. Map request to entity (only non-null properties)
            _mapper.Map(request, activity);

            // 8. Update audit fields
            activity.UpdatedAt = DateTime.UtcNow;
            activity.LastModifiedBy = userId;

            // 9. Recalculate duration if times changed
            if (request.StartTime.HasValue || request.EndTime.HasValue)
            {
                var startMinutes = activity.StartTime.Hour * 60 + activity.StartTime.Minute;
                var endMinutes = activity.EndTime.Hour * 60 + activity.EndTime.Minute;
                activity.DurationMinutes = endMinutes - startMinutes;
            }

            // 10. Persist to database
            await _activityRepository.UpdateAsync(activity, cancellationToken);
            await _activityRepository.SaveChangesAsync(cancellationToken);

            // 11. Map to response
            var response = _mapper.Map<ActivityResponse>(activity);

            _logger.LogInformation($"Activity {activityId} updated successfully by user {userId}");
            return response;
        }

        /// <summary>
        /// Deletes an activity (hard delete - permanent removal).
        /// </summary>
        public async Task DeleteActivityAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            Guid activityId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Deleting activity {activityId} by user {userId}");

            // 1. Verify user owns the trip
            var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
            if (trip == null)
            {
                throw new EntityNotFoundException("Trip not found");
            }

            if (trip.UserId != userId)
            {
                _logger.LogWarning($"User {userId} attempted to access trip {tripId} they don't own");
                throw new ForbiddenException("You don't have permission to access this trip");
            }

            // 2. Verify itinerary day exists and belongs to trip
            var day = await _itineraryRepository.GetByIdAsync(dayId, cancellationToken);
            if (day == null)
            {
                throw new EntityNotFoundException("Itinerary day not found");
            }

            if (day.TripId != tripId)
            {
                throw new ValidationException("Itinerary day does not belong to this trip");
            }

            // 3. Get activity
            var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);
            if (activity == null)
            {
                throw new EntityNotFoundException("Activity not found");
            }

            // 4. Verify activity belongs to the day
            if (activity.DayId != dayId)
            {
                throw new ValidationException("Activity does not belong to the specified itinerary day");
            }

            // 5. Delete activity (hard delete - permanent removal)
            await _activityRepository.DeleteAsync(activityId, DateTime.UtcNow, cancellationToken);
            await _activityRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Activity {activityId} deleted successfully by user {userId}");
        }
    }
}
