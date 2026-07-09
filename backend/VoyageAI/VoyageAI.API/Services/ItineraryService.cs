using AutoMapper;
using VoyageAI.API.Common.Exceptions;
using VoyageAI.API.Common.Models;
using VoyageAI.API.DTOs.Itinerary;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Services
{
    /// <summary>
    /// Service for itinerary day operations (CRUD).
    /// 
    /// This class implements all business logic for itinerary day management.
    /// It handles creation, retrieval, update, and deletion with proper authorization.
    /// 
    /// Design Pattern: Service Layer
    /// - Encapsulates all itinerary day business logic
    /// - Depends on interfaces (IItineraryRepository, ITripRepository, IMapper) for loose coupling
    /// - Enables unit testing through dependency injection
    /// - Centralizes authorization checks
    /// 
    /// Dependencies:
    /// - IItineraryRepository: Data access for ItineraryDay entity
    /// - ITripRepository: Data access for Trip entity (for ownership verification)
    /// - IMapper: AutoMapper for DTO conversions
    /// - ILogger: Logging for debugging and monitoring
    /// 
    /// Security Features:
    /// - Authorization checks on all operations (users can only access their own trips' itineraries)
    /// - Validates dates and budgets
    /// - Validates DayNumber uniqueness per trip
    /// - Proper error messages without information leakage
    /// - Audit fields track who created/modified each day
    /// 
    /// Business Rules:
    /// - Each ItineraryDay belongs to exactly ONE Trip
    /// - Date must fall within Trip's StartDate and EndDate
    /// - DayNumber must be unique within a Trip
    /// - Only trip owner can manage itinerary days
    /// - Soft delete: days are never physically deleted
    /// 
    /// Dependency Injection:
    /// services.AddScoped{IItineraryService, ItineraryService}();
    /// </summary>
    public class ItineraryService : IItineraryService
    {
        /// <summary>
        /// Repository for itinerary day data access operations.
        /// Handles CREATE, READ, UPDATE, DELETE operations for ItineraryDay entity.
        /// </summary>
        private readonly IItineraryRepository _itineraryRepository;

        /// <summary>
        /// Repository for trip data access operations.
        /// Used to verify trip ownership before allowing itinerary operations.
        /// </summary>
        private readonly ITripRepository _tripRepository;

        /// <summary>
        /// AutoMapper instance for mapping between DTOs and entities.
        /// Maps CreateItineraryDayRequest/UpdateItineraryDayRequest DTOs to ItineraryDay entity and back.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Generic logger for this service.
        /// Used to log important events: itinerary day creation, updates, authorization failures, errors.
        /// </summary>
        private readonly ILogger<ItineraryService> _logger;

        /// <summary>
        /// Initializes a new instance of the ItineraryService class.
        /// 
        /// Constructor Injection:
        /// All dependencies are injected by the DI container.
        /// This enables loose coupling and testability.
        /// </summary>
        public ItineraryService(
            IItineraryRepository itineraryRepository,
            ITripRepository tripRepository,
            IMapper mapper,
            ILogger<ItineraryService> logger)
        {
            _itineraryRepository = itineraryRepository ?? throw new ArgumentNullException(nameof(itineraryRepository));
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new itinerary day for a trip.
        /// </summary>
        public async Task<ApiResponse<ItineraryDayResponse>> CreateItineraryDayAsync(
            Guid userId,
            Guid tripId,
            CreateItineraryDayRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Creating itinerary day for trip {TripId} by user {UserId}", tripId, userId);

                // 1. Load trip and verify ownership
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                if (trip == null)
                {
                    _logger.LogWarning("Trip {TripId} not found", tripId);
                    throw new EntityNotFoundException("Trip not found");
                }

                if (trip.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to access trip {TripId} they don't own", userId, tripId);
                    throw new ForbiddenException("You don't have permission to manage this trip's itinerary");
                }

                // 2. Validate date falls within trip date range
                if (request.Date < trip.StartDate || request.Date > trip.EndDate)
                {
                    _logger.LogWarning("Itinerary day date {Date} falls outside trip date range {StartDate}-{EndDate}", 
                        request.Date, trip.StartDate, trip.EndDate);
                    throw new ValidationException("Date must fall within the trip's start and end dates");
                }

                // 3. Validate DayNumber is unique within the trip
                bool dayNumberExists = await _itineraryRepository.DayNumberExistsAsync(tripId, request.DayNumber, null, cancellationToken);
                if (dayNumberExists)
                {
                    _logger.LogWarning("Day number {DayNumber} already exists in trip {TripId}", request.DayNumber, tripId);
                    throw new ValidationException($"Day number {request.DayNumber} already exists in this trip");
                }

                // 4. Map request to entity
                var itineraryDay = _mapper.Map<ItineraryDay>(request);
                itineraryDay.TripId = tripId;

                // 5. Set audit fields
                itineraryDay.CreatedAt = DateTime.UtcNow;
                itineraryDay.UpdatedAt = DateTime.UtcNow;
                itineraryDay.CreatedBy = userId;
                itineraryDay.LastModifiedBy = userId;
                itineraryDay.IsDeleted = false;
                itineraryDay.DeletedAt = null;

                // 6. Persist to database
                await _itineraryRepository.CreateAsync(itineraryDay, cancellationToken);
                await _itineraryRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created itinerary day {DayId} for trip {TripId}", itineraryDay.DayId, tripId);

                // 7. Map to response and return
                var response = _mapper.Map<ItineraryDayResponse>(itineraryDay);
                return new ApiResponse<ItineraryDayResponse>
                {
                    Data = response,
                    Success = true,
                    Message = "Itinerary day created successfully"
                };
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError(ex, "Entity not found while creating itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex, "Authorization failed while creating itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation failed while creating itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the itinerary day",
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
        }

        /// <summary>
        /// Retrieves all itinerary days for a trip.
        /// </summary>
        public async Task<ApiResponse<List<ItinerarySummaryResponse>>> GetItineraryDaysAsync(
            Guid userId,
            Guid tripId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Retrieving itinerary days for trip {TripId} by user {UserId}", tripId, userId);

                // 1. Load trip and verify ownership
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                if (trip == null)
                {
                    _logger.LogWarning("Trip {TripId} not found", tripId);
                    throw new EntityNotFoundException("Trip not found");
                }

                if (trip.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to access trip {TripId} they don't own", userId, tripId);
                    throw new ForbiddenException("You don't have permission to access this trip's itinerary");
                }

                // 2. Load all itinerary days for the trip
                var days = await _itineraryRepository.GetByTripIdAsync(tripId, cancellationToken);

                // 3. Map to responses
                var responses = _mapper.Map<List<ItinerarySummaryResponse>>(days);

                _logger.LogInformation("Retrieved {Count} itinerary days for trip {TripId}", responses.Count, tripId);

                return new ApiResponse<List<ItinerarySummaryResponse>>
                {
                    Data = responses,
                    Success = true,
                    Message = "Itinerary days retrieved successfully"
                };
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError(ex, "Entity not found while retrieving itinerary days");
                return new ApiResponse<List<ItinerarySummaryResponse>>
                {
                    Data = new List<ItinerarySummaryResponse>(),
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex, "Authorization failed while retrieving itinerary days");
                return new ApiResponse<List<ItinerarySummaryResponse>>
                {
                    Data = new List<ItinerarySummaryResponse>(),
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving itinerary days");
                return new ApiResponse<List<ItinerarySummaryResponse>>
                {
                    Data = new List<ItinerarySummaryResponse>(),
                    Success = false,
                    Message = "An unexpected error occurred while retrieving itinerary days",
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
        }

        /// <summary>
        /// Retrieves a specific itinerary day.
        /// </summary>
        public async Task<ApiResponse<ItineraryDayResponse>> GetItineraryDayAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Retrieving itinerary day {DayId} from trip {TripId} by user {UserId}", dayId, tripId, userId);

                // 1. Load itinerary day
                var day = await _itineraryRepository.GetByIdAsync(dayId, cancellationToken);
                if (day == null)
                {
                    _logger.LogWarning("Itinerary day {DayId} not found", dayId);
                    throw new EntityNotFoundException("Itinerary day not found");
                }

                // 2. Verify day belongs to the specified trip
                if (day.TripId != tripId)
                {
                    _logger.LogWarning("Itinerary day {DayId} does not belong to trip {TripId}", dayId, tripId);
                    throw new EntityNotFoundException("Itinerary day not found in this trip");
                }

                // 3. Verify trip ownership
                var trip = day.Trip;
                if (trip == null)
                {
                    trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                    if (trip == null)
                    {
                        _logger.LogWarning("Trip {TripId} not found", tripId);
                        throw new EntityNotFoundException("Trip not found");
                    }
                }

                if (trip.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to access trip {TripId} they don't own", userId, tripId);
                    throw new ForbiddenException("You don't have permission to access this itinerary day");
                }

                // 4. Map to response
                var response = _mapper.Map<ItineraryDayResponse>(day);

                _logger.LogInformation("Retrieved itinerary day {DayId}", dayId);

                return new ApiResponse<ItineraryDayResponse>
                {
                    Data = response,
                    Success = true,
                    Message = "Itinerary day retrieved successfully"
                };
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError(ex, "Entity not found while retrieving itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex, "Authorization failed while retrieving itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the itinerary day",
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
        }

        /// <summary>
        /// Updates an existing itinerary day.
        /// </summary>
        public async Task<ApiResponse<ItineraryDayResponse>> UpdateItineraryDayAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            UpdateItineraryDayRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating itinerary day {DayId} in trip {TripId} by user {UserId}", dayId, tripId, userId);

                // 1. Load itinerary day
                var day = await _itineraryRepository.GetByIdAsync(dayId, cancellationToken);
                if (day == null)
                {
                    _logger.LogWarning("Itinerary day {DayId} not found", dayId);
                    throw new EntityNotFoundException("Itinerary day not found");
                }

                // 2. Verify day belongs to the specified trip
                if (day.TripId != tripId)
                {
                    _logger.LogWarning("Itinerary day {DayId} does not belong to trip {TripId}", dayId, tripId);
                    throw new EntityNotFoundException("Itinerary day not found in this trip");
                }

                // 3. Verify trip ownership
                var trip = day.Trip;
                if (trip == null)
                {
                    trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                    if (trip == null)
                    {
                        _logger.LogWarning("Trip {TripId} not found", tripId);
                        throw new EntityNotFoundException("Trip not found");
                    }
                }

                if (trip.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to access trip {TripId} they don't own", userId, tripId);
                    throw new ForbiddenException("You don't have permission to update this itinerary day");
                }

                // 4. Validate date falls within trip date range
                if (request.Date < trip.StartDate || request.Date > trip.EndDate)
                {
                    _logger.LogWarning("Updated itinerary day date {Date} falls outside trip date range {StartDate}-{EndDate}", 
                        request.Date, trip.StartDate, trip.EndDate);
                    throw new ValidationException("Date must fall within the trip's start and end dates");
                }

                // 5. If DayNumber changed, validate uniqueness
                if (day.DayNumber != request.DayNumber)
                {
                    bool dayNumberExists = await _itineraryRepository.DayNumberExistsAsync(tripId, request.DayNumber, dayId, cancellationToken);
                    if (dayNumberExists)
                    {
                        _logger.LogWarning("Day number {DayNumber} already exists in trip {TripId}", request.DayNumber, tripId);
                        throw new ValidationException($"Day number {request.DayNumber} already exists in this trip");
                    }
                }

                // 6. Update entity from request
                _mapper.Map(request, day);

                // 7. Update audit fields
                day.UpdatedAt = DateTime.UtcNow;
                day.LastModifiedBy = userId;

                // 8. Persist to database
                await _itineraryRepository.UpdateAsync(day, cancellationToken);
                await _itineraryRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated itinerary day {DayId}", dayId);

                // 9. Map to response
                var response = _mapper.Map<ItineraryDayResponse>(day);
                return new ApiResponse<ItineraryDayResponse>
                {
                    Data = response,
                    Success = true,
                    Message = "Itinerary day updated successfully"
                };
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError(ex, "Entity not found while updating itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex, "Authorization failed while updating itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation failed while updating itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating itinerary day");
                return new ApiResponse<ItineraryDayResponse>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the itinerary day",
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
        }

        /// <summary>
        /// Deletes an itinerary day (soft delete).
        /// </summary>
        public async Task<ApiResponse<object>> DeleteItineraryDayAsync(
            Guid userId,
            Guid tripId,
            Guid dayId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Deleting itinerary day {DayId} from trip {TripId} by user {UserId}", dayId, tripId, userId);

                // 1. Load itinerary day
                var day = await _itineraryRepository.GetByIdAsync(dayId, cancellationToken);
                if (day == null)
                {
                    _logger.LogWarning("Itinerary day {DayId} not found", dayId);
                    throw new EntityNotFoundException("Itinerary day not found");
                }

                // 2. Verify day belongs to the specified trip
                if (day.TripId != tripId)
                {
                    _logger.LogWarning("Itinerary day {DayId} does not belong to trip {TripId}", dayId, tripId);
                    throw new EntityNotFoundException("Itinerary day not found in this trip");
                }

                // 3. Verify trip ownership
                var trip = day.Trip;
                if (trip == null)
                {
                    trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                    if (trip == null)
                    {
                        _logger.LogWarning("Trip {TripId} not found", tripId);
                        throw new EntityNotFoundException("Trip not found");
                    }
                }

                if (trip.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to access trip {TripId} they don't own", userId, tripId);
                    throw new ForbiddenException("You don't have permission to delete this itinerary day");
                }

                // 4. Hard delete - permanently remove from database
                await _itineraryRepository.DeleteAsync(dayId, DateTime.UtcNow, cancellationToken);
                await _itineraryRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted itinerary day {DayId}", dayId);

                return new ApiResponse<object>
                {
                    Data = null,
                    Success = true,
                    Message = "Itinerary day deleted successfully"
                };
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError(ex, "Entity not found while deleting itinerary day");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError(ex, "Authorization failed while deleting itinerary day");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting itinerary day");
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "An unexpected error occurred while deleting the itinerary day",
                    Errors = new List<ApiError> { new ApiError(ex.Message) }
                };
            }
        }
    }
}
