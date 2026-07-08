using AutoMapper;
using VoyageAI.API.Common.Exceptions;
using VoyageAI.API.DTOs.Travelers;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Services
{
    /// <summary>
    /// Service for traveler operations (CRUD).
    /// 
    /// This class implements all business logic for traveler management.
    /// It handles traveler creation, retrieval, update, and deletion with proper authorization and soft deletes.
    /// 
    /// Design Pattern: Service Layer
    /// - Encapsulates all traveler business logic
    /// - Depends on interfaces (ITravelerRepository, ITripRepository, IMapper) for loose coupling
    /// - Enables unit testing through dependency injection
    /// 
    /// Dependencies:
    /// - ITravelerRepository: Data access for Traveler entity
    /// - ITripRepository: Data access for Trip entity (ownership verification)
    /// - IMapper: AutoMapper for DTO conversions
    /// - ILogger: Logging for debugging and monitoring
    /// 
    /// Security Features:
    /// - Authorization checks on all operations (users can only access travelers on their trips)
    /// - Trip ownership verification before any traveler operation
    /// - Traveler membership verification (traveler must belong to trip)
    /// - Soft delete pattern (never physically delete travelers)
    /// - Duplicate email check (prevent adding same person twice to same trip)
    /// - Audit fields (CreatedAt, CreatedBy, UpdatedAt, LastModifiedBy)
    /// 
    /// Dependency Injection:
    /// services.AddScoped{ITravelerService, TravelerService}();
    /// </summary>
    public class TravelerService : ITravelerService
    {
        /// <summary>
        /// Repository for traveler data access operations.
        /// Handles all CREATE, READ, UPDATE, DELETE operations for the Traveler entity.
        /// </summary>
        private readonly ITravelerRepository _travelerRepository;

        /// <summary>
        /// Repository for trip data access operations.
        /// Used to verify trip ownership before any traveler operation.
        /// </summary>
        private readonly ITripRepository _tripRepository;

        /// <summary>
        /// AutoMapper instance for mapping between DTOs and entities.
        /// Maps CreateTravelerRequest/UpdateTravelerRequest DTOs to Traveler entity and back.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Generic logger for this service.
        /// Used to log important events: traveler creation, updates, deletions, authorization failures.
        /// </summary>
        private readonly ILogger<TravelerService> _logger;

        /// <summary>
        /// Initializes a new instance of the TravelerService class.
        /// 
        /// Constructor Injection:
        /// All dependencies are injected by the DI container.
        /// This enables loose coupling and testability.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped{ITravelerRepository, TravelerRepository}();
        /// services.AddScoped{ITripRepository, TripRepository}();
        /// services.AddScoped{ITravelerService, TravelerService}();
        /// services.AddAutoMapper(typeof(Program));
        /// </summary>
        /// <param name="travelerRepository">Repository for traveler data operations</param>
        /// <param name="tripRepository">Repository for trip data operations</param>
        /// <param name="mapper">AutoMapper for DTO conversions</param>
        /// <param name="logger">Logger instance for this service</param>
        public TravelerService(
            ITravelerRepository travelerRepository,
            ITripRepository tripRepository,
            IMapper mapper,
            ILogger<TravelerService> logger)
        {
            _travelerRepository = travelerRepository ?? throw new ArgumentNullException(nameof(travelerRepository));
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new traveler for a trip owned by the authenticated user.
        /// </summary>
        public async Task<TravelerResponse> CreateTravelerAsync(
            Guid userId,
            Guid tripId,
            CreateTravelerRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Step 1: Verify trip exists
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                if (trip == null)
                {
                    _logger.LogWarning($"Trip {tripId} not found. CreateTraveler request from user {userId} failed.");
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found.");
                }

                // Step 2: Verify user owns the trip (CRITICAL SECURITY CHECK)
                if (trip.UserId != userId)
                {
                    _logger.LogWarning($"Unauthorized CreateTraveler attempt. User {userId} tried to add traveler to trip {tripId} owned by {trip.UserId}");
                    throw new ForbiddenException("You do not have permission to manage travelers on this trip.");
                }

                // Step 3: Check for duplicate email (if email is provided)
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    var emailExists = await _travelerRepository.TravelerExistsByEmailAsync(tripId, request.Email, null, cancellationToken);
                    if (emailExists)
                    {
                        _logger.LogWarning($"Duplicate email {request.Email} in trip {tripId}. CreateTraveler request from user {userId} failed.");
                        throw new ValidationException($"A traveler with email '{request.Email}' already exists in this trip.");
                    }
                }

                // Step 4: Map request to Traveler entity
                var traveler = _mapper.Map<Traveler>(request);
                traveler.TravelerId = Guid.NewGuid();
                traveler.TripId = tripId;

                // Step 5: Set audit fields
                traveler.CreatedAt = DateTime.UtcNow;
                traveler.UpdatedAt = DateTime.UtcNow;
                traveler.CreatedBy = userId;
                traveler.LastModifiedBy = userId;
                traveler.IsDeleted = false;

                // Step 6: Create traveler in database
                await _travelerRepository.CreateAsync(traveler, cancellationToken);
                await _travelerRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Traveler {traveler.TravelerId} created in trip {tripId} by user {userId}");

                // Step 7: Map to response and return
                return _mapper.Map<TravelerResponse>(traveler);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"EntityNotFoundException in CreateTravelerAsync: {ex.Message}");
                throw;
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError($"ForbiddenException in CreateTravelerAsync: {ex.Message}");
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"ValidationException in CreateTravelerAsync: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected exception in CreateTravelerAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves all travelers for a specific trip owned by the authenticated user.
        /// </summary>
        public async Task<List<TravelerResponse>> GetTravelersByTripAsync(
            Guid userId,
            Guid tripId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Step 1: Verify trip exists
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                if (trip == null)
                {
                    _logger.LogWarning($"Trip {tripId} not found. GetTravelersByTrip request from user {userId} failed.");
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found.");
                }

                // Step 2: Verify user owns the trip (CRITICAL SECURITY CHECK)
                if (trip.UserId != userId)
                {
                    _logger.LogWarning($"Unauthorized GetTravelersByTrip attempt. User {userId} tried to access travelers for trip {tripId} owned by {trip.UserId}");
                    throw new ForbiddenException("You do not have permission to view travelers on this trip.");
                }

                // Step 3: Fetch travelers for the trip (automatically excludes deleted travelers)
                var travelers = await _travelerRepository.GetTravelersByTripAsync(tripId, cancellationToken);

                _logger.LogInformation($"Retrieved {travelers.Count} travelers for trip {tripId} by user {userId}");

                // Step 4: Map to responses and return
                return _mapper.Map<List<TravelerResponse>>(travelers);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"EntityNotFoundException in GetTravelersByTripAsync: {ex.Message}");
                throw;
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError($"ForbiddenException in GetTravelersByTripAsync: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected exception in GetTravelersByTripAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a specific traveler by ID.
        /// </summary>
        public async Task<TravelerResponse> GetTravelerByIdAsync(
            Guid userId,
            Guid tripId,
            Guid travelerId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Step 1: Verify trip exists
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                if (trip == null)
                {
                    _logger.LogWarning($"Trip {tripId} not found. GetTravelerById request from user {userId} failed.");
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found.");
                }

                // Step 2: Verify user owns the trip (CRITICAL SECURITY CHECK)
                if (trip.UserId != userId)
                {
                    _logger.LogWarning($"Unauthorized GetTravelerById attempt. User {userId} tried to access traveler on trip {tripId} owned by {trip.UserId}");
                    throw new ForbiddenException("You do not have permission to view travelers on this trip.");
                }

                // Step 3: Fetch traveler from database
                var traveler = await _travelerRepository.GetByIdAsync(travelerId, cancellationToken);
                if (traveler == null || traveler.IsDeleted)
                {
                    _logger.LogWarning($"Traveler {travelerId} not found or deleted. GetTravelerById request from user {userId} failed.");
                    throw new EntityNotFoundException($"Traveler with ID {travelerId} not found.");
                }

                // Step 4: Verify traveler belongs to this trip
                if (traveler.TripId != tripId)
                {
                    _logger.LogWarning($"Traveler {travelerId} does not belong to trip {tripId}. Got different tripId: {traveler.TripId}");
                    throw new ForbiddenException("This traveler does not belong to the specified trip.");
                }

                _logger.LogInformation($"Retrieved traveler {travelerId} from trip {tripId} by user {userId}");

                // Step 5: Map to response and return
                return _mapper.Map<TravelerResponse>(traveler);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"EntityNotFoundException in GetTravelerByIdAsync: {ex.Message}");
                throw;
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError($"ForbiddenException in GetTravelerByIdAsync: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected exception in GetTravelerByIdAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing traveler.
        /// </summary>
        public async Task<TravelerResponse> UpdateTravelerAsync(
            Guid userId,
            Guid tripId,
            Guid travelerId,
            UpdateTravelerRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Step 1: Verify trip exists
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                if (trip == null)
                {
                    _logger.LogWarning($"Trip {tripId} not found. UpdateTraveler request from user {userId} failed.");
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found.");
                }

                // Step 2: Verify user owns the trip (CRITICAL SECURITY CHECK)
                if (trip.UserId != userId)
                {
                    _logger.LogWarning($"Unauthorized UpdateTraveler attempt. User {userId} tried to update traveler on trip {tripId} owned by {trip.UserId}");
                    throw new ForbiddenException("You do not have permission to manage travelers on this trip.");
                }

                // Step 3: Fetch traveler from database
                var traveler = await _travelerRepository.GetByIdAsync(travelerId, cancellationToken);
                if (traveler == null || traveler.IsDeleted)
                {
                    _logger.LogWarning($"Traveler {travelerId} not found or deleted. UpdateTraveler request from user {userId} failed.");
                    throw new EntityNotFoundException($"Traveler with ID {travelerId} not found.");
                }

                // Step 4: Verify traveler belongs to this trip
                if (traveler.TripId != tripId)
                {
                    _logger.LogWarning($"Traveler {travelerId} does not belong to trip {tripId}. Got different tripId: {traveler.TripId}");
                    throw new ForbiddenException("This traveler does not belong to the specified trip.");
                }

                // Step 5: Check for duplicate email (if email is being changed)
                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != traveler.Email)
                {
                    var emailExists = await _travelerRepository.TravelerExistsByEmailAsync(tripId, request.Email, travelerId, cancellationToken);
                    if (emailExists)
                    {
                        _logger.LogWarning($"Duplicate email {request.Email} in trip {tripId}. UpdateTraveler request from user {userId} failed.");
                        throw new ValidationException($"A traveler with email '{request.Email}' already exists in this trip.");
                    }
                }

                // Step 6: Apply updates from request (only non-null fields)
                _mapper.Map(request, traveler);

                // Step 7: Update audit fields
                traveler.UpdatedAt = DateTime.UtcNow;
                traveler.LastModifiedBy = userId;

                // Step 8: Update traveler in database
                await _travelerRepository.UpdateAsync(traveler, cancellationToken);
                await _travelerRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Traveler {travelerId} in trip {tripId} updated by user {userId}");

                // Step 9: Map to response and return
                return _mapper.Map<TravelerResponse>(traveler);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"EntityNotFoundException in UpdateTravelerAsync: {ex.Message}");
                throw;
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError($"ForbiddenException in UpdateTravelerAsync: {ex.Message}");
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"ValidationException in UpdateTravelerAsync: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected exception in UpdateTravelerAsync: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Soft deletes an existing traveler.
        /// </summary>
        public async Task DeleteTravelerAsync(
            Guid userId,
            Guid tripId,
            Guid travelerId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Step 1: Verify trip exists
                var trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
                if (trip == null)
                {
                    _logger.LogWarning($"Trip {tripId} not found. DeleteTraveler request from user {userId} failed.");
                    throw new EntityNotFoundException($"Trip with ID {tripId} not found.");
                }

                // Step 2: Verify user owns the trip (CRITICAL SECURITY CHECK)
                if (trip.UserId != userId)
                {
                    _logger.LogWarning($"Unauthorized DeleteTraveler attempt. User {userId} tried to delete traveler on trip {tripId} owned by {trip.UserId}");
                    throw new ForbiddenException("You do not have permission to manage travelers on this trip.");
                }

                // Step 3: Fetch traveler from database
                var traveler = await _travelerRepository.GetByIdAsync(travelerId, cancellationToken);
                if (traveler == null || traveler.IsDeleted)
                {
                    _logger.LogWarning($"Traveler {travelerId} not found or already deleted. DeleteTraveler request from user {userId} failed.");
                    throw new EntityNotFoundException($"Traveler with ID {travelerId} not found.");
                }

                // Step 4: Verify traveler belongs to this trip
                if (traveler.TripId != tripId)
                {
                    _logger.LogWarning($"Traveler {travelerId} does not belong to trip {tripId}. Got different tripId: {traveler.TripId}");
                    throw new ForbiddenException("This traveler does not belong to the specified trip.");
                }

                // Step 5: Soft delete the traveler
                traveler.IsDeleted = true;
                traveler.DeletedAt = DateTime.UtcNow;
                traveler.LastModifiedBy = userId;

                // Step 6: Update traveler in database
                await _travelerRepository.UpdateAsync(traveler, cancellationToken);
                await _travelerRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Traveler {travelerId} in trip {tripId} soft-deleted by user {userId}");
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError($"EntityNotFoundException in DeleteTravelerAsync: {ex.Message}");
                throw;
            }
            catch (ForbiddenException ex)
            {
                _logger.LogError($"ForbiddenException in DeleteTravelerAsync: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected exception in DeleteTravelerAsync: {ex.Message}");
                throw;
            }
        }
    }
}
