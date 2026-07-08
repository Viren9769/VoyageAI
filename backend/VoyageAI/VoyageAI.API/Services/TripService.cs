using AutoMapper;
using VoyageAI.API.Common.Exceptions;
using VoyageAI.API.DTOs.Trip;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Services
{
    /// <summary>
    /// Service for trip operations (CRUD).
    /// 
    /// This class implements all business logic for trip management.
    /// It handles trip creation, retrieval, update, and deletion with proper authorization.
    /// 
    /// Design Pattern: Service Layer
    /// - Encapsulates all trip business logic
    /// - Depends on interfaces (ITripRepository, IMapper) for loose coupling
    /// - Enables unit testing through dependency injection
    /// 
    /// Dependencies:
    /// - ITripRepository: Data access for Trip entity
    /// - IMapper: AutoMapper for DTO conversions
    /// - ILogger: Logging for debugging and monitoring
    /// 
    /// Security Features:
    /// - Authorization checks on all operations (users can only access their own trips)
    /// - Validates dates and budgets
    /// - Proper error messages without information leakage
    /// 
    /// Dependency Injection:
    /// services.AddScoped{ITripService, TripService}();
    /// </summary>
    public class TripService : ITripService
    {
        /// <summary>
        /// Repository for trip data access operations.
        /// Handles all CREATE, READ, UPDATE, DELETE operations for the Trip entity.
        /// </summary>
        private readonly ITripRepository _tripRepository;

        /// <summary>
        /// AutoMapper instance for mapping between DTOs and entities.
        /// Maps CreateTripRequest/UpdateTripRequest DTOs to Trip entity and back.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Generic logger for this service.
        /// Used to log important events: trip creation, updates, errors.
        /// </summary>
        private readonly ILogger<TripService> _logger;

        /// <summary>
        /// Initializes a new instance of the TripService class.
        /// 
        /// Constructor Injection:
        /// All dependencies are injected by the DI container.
        /// This enables loose coupling and testability.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped{ITripRepository, TripRepository}();
        /// services.AddScoped{ITripService, TripService}();
        /// services.AddAutoMapper(typeof(Program));
        /// </summary>
        /// <param name="tripRepository">Repository for trip data operations</param>
        /// <param name="mapper">AutoMapper for DTO conversions</param>
        /// <param name="logger">Logger instance for this service</param>
        public TripService(
            ITripRepository tripRepository,
            IMapper mapper,
            ILogger<TripService> logger)
        {
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new trip for the authenticated user.
        /// </summary>
        public async Task<GetTripResponse> CreateTripAsync(
            Guid userId,
            CreateTripRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate dates
                if (request.EndDate < request.StartDate)
                {
                    throw new ValidationException("End date must be on or after start date");
                }

                // Validate budget
                if (request.Budget < 0)
                {
                    throw new ValidationException("Budget must be a positive value");
                }

                // Map request to entity
                var trip = _mapper.Map<Trip>(request);
                trip.UserId = userId;
                trip.CreatedAt = DateTime.UtcNow;
                trip.UpdatedAt = DateTime.UtcNow;

                // Persist to database
                await _tripRepository.CreateAsync(trip, cancellationToken);
                await _tripRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Trip created successfully. TripId: {TripId}, UserId: {UserId}, TripName: {TripName}",
                    trip.TripId, userId, trip.TripName);

                // Map entity to response
                return _mapper.Map<GetTripResponse>(trip);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating trip for user {UserId}. TripName: {TripName}",
                    userId, request.TripName);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all trips for the authenticated user.
        /// </summary>
        public async Task<List<GetTripResponse>> GetUserTripsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var trips = await _tripRepository.GetUserTripsAsync(userId, cancellationToken);

                _logger.LogInformation(
                    "Retrieved {TripCount} trips for user {UserId}",
                    trips.Count, userId);

                return _mapper.Map<List<GetTripResponse>>(trips);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrieving trips for user {UserId}",
                    userId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific trip by ID with authorization check.
        /// </summary>
        public async Task<GetTripResponse> GetTripByIdAsync(
            Guid tripId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);

                if (trip == null)
                {
                    _logger.LogWarning(
                        "Trip not found. TripId: {TripId}, UserId: {UserId}",
                        tripId, userId);
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found");
                }

                // Authorization check: ensure user owns this trip
                if (trip.UserId != userId)
                {
                    _logger.LogWarning(
                        "Unauthorized access attempt. TripId: {TripId}, UserId: {UserId}, TripOwnerId: {TripOwnerId}",
                        tripId, userId, trip.UserId);
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found");  // Don't reveal ownership
                }

                return _mapper.Map<GetTripResponse>(trip);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrieving trip. TripId: {TripId}, UserId: {UserId}",
                    tripId, userId);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing trip with authorization check.
        /// </summary>
        public async Task<GetTripResponse> UpdateTripAsync(
            Guid tripId,
            Guid userId,
            UpdateTripRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);

                if (trip == null)
                {
                    _logger.LogWarning(
                        "Trip not found for update. TripId: {TripId}, UserId: {UserId}",
                        tripId, userId);
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found");
                }

                // Authorization check: ensure user owns this trip
                if (trip.UserId != userId)
                {
                    _logger.LogWarning(
                        "Unauthorized update attempt. TripId: {TripId}, UserId: {UserId}, TripOwnerId: {TripOwnerId}",
                        tripId, userId, trip.UserId);
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found");  // Don't reveal ownership
                }

                // Validate dates if provided
                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    if (request.EndDate < request.StartDate)
                    {
                        throw new ValidationException("End date must be on or after start date");
                    }
                }
                else if (request.EndDate.HasValue && request.StartDate == null)
                {
                    if (request.EndDate < trip.StartDate)
                    {
                        throw new ValidationException("End date must be on or after start date");
                    }
                }
                else if (request.StartDate.HasValue && request.EndDate == null)
                {
                    if (trip.EndDate < request.StartDate)
                    {
                        throw new ValidationException("End date must be on or after start date");
                    }
                }

                // Validate budget if provided
                if (request.Budget.HasValue && request.Budget < 0)
                {
                    throw new ValidationException("Budget must be a positive value");
                }

                // Apply updates (only non-null values from request)
                if (!string.IsNullOrWhiteSpace(request.TripName))
                    trip.TripName = request.TripName;

                if (!string.IsNullOrWhiteSpace(request.DestinationCountry))
                    trip.DestinationCountry = request.DestinationCountry;

                if (!string.IsNullOrWhiteSpace(request.DestinationCity))
                    trip.DestinationCity = request.DestinationCity;

                if (request.StartDate.HasValue)
                    trip.StartDate = request.StartDate.Value;

                if (request.EndDate.HasValue)
                    trip.EndDate = request.EndDate.Value;

                if (request.Budget.HasValue)
                    trip.Budget = request.Budget.Value;

                if (!string.IsNullOrWhiteSpace(request.Currency))
                    trip.Currency = request.Currency;

                if (!string.IsNullOrWhiteSpace(request.TravelStyle))
                    trip.TravelStyle = request.TravelStyle;

                if (!string.IsNullOrWhiteSpace(request.Description))
                    trip.Description = request.Description;

                if (!string.IsNullOrWhiteSpace(request.CoverImageUrl))
                    trip.CoverImageUrl = request.CoverImageUrl;

                if (!string.IsNullOrWhiteSpace(request.Status))
                    trip.Status = request.Status;

                // Update timestamp
                trip.UpdatedAt = DateTime.UtcNow;

                // Persist changes
                await _tripRepository.UpdateAsync(trip, cancellationToken);
                await _tripRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Trip updated successfully. TripId: {TripId}, UserId: {UserId}",
                    tripId, userId);

                return _mapper.Map<GetTripResponse>(trip);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating trip. TripId: {TripId}, UserId: {UserId}",
                    tripId, userId);
                throw;
            }
        }

        /// <summary>
        /// Deletes a trip with authorization check.
        /// </summary>
        public async Task DeleteTripAsync(
            Guid tripId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);

                if (trip == null)
                {
                    _logger.LogWarning(
                        "Trip not found for deletion. TripId: {TripId}, UserId: {UserId}",
                        tripId, userId);
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found");
                }

                // Authorization check: ensure user owns this trip
                if (trip.UserId != userId)
                {
                    _logger.LogWarning(
                        "Unauthorized delete attempt. TripId: {TripId}, UserId: {UserId}, TripOwnerId: {TripOwnerId}",
                        tripId, userId, trip.UserId);
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found");  // Don't reveal ownership
                }

                // Delete trip (cascade delete handles related entities)
                await _tripRepository.DeleteAsync(trip, cancellationToken);
                await _tripRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Trip deleted successfully. TripId: {TripId}, UserId: {UserId}",
                    tripId, userId);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting trip. TripId: {TripId}, UserId: {UserId}",
                    tripId, userId);
                throw;
            }
        }
    }
}
