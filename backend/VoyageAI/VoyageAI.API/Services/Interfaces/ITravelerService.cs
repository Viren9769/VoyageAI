using VoyageAI.API.DTOs.Travelers;

namespace VoyageAI.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for traveler operations.
    /// 
    /// This interface defines the contract for all traveler business logic.
    /// All complex business rules, validations, and security checks are handled by the service layer.
    /// 
    /// Separation of Concerns:
    /// - Controller: HTTP routing only, calls service
    /// - Service: Business logic, validation, authorization, orchestration
    /// - Repository: Data access only
    /// - Entity: Database schema only
    /// 
    /// Design Patterns:
    /// - Service Pattern: Encapsulates business logic
    /// - Dependency Inversion: Controllers depend on interface, not concrete class
    /// - Async Operations: All methods are async for scalability
    /// 
    /// Dependency Injection:
    /// Injected as: services.AddScoped{ITravelerService, TravelerService}()
    /// 
    /// Critical Security Rules:
    /// 1. Every endpoint must verify: User exists, Trip exists, Trip belongs to user, Traveler belongs to trip
    /// 2. UserId is extracted from JWT token (never from request body)
    /// 3. TripId is provided as route parameter (never from request body)
    /// 4. Users can ONLY manage travelers on trips they own
    /// 5. Soft delete pattern: Never physically delete travelers, set IsDeleted = true
    /// 
    /// Authorization Verification Order:
    /// 1. Get trip from database
    /// 2. Verify trip.UserId == authenticated userId (ownership check)
    /// 3. Get traveler from database
    /// 4. Verify traveler.TripId == tripId (traveler belongs to trip)
    /// 5. Proceed with operation
    /// 
    /// If any verification fails:
    /// - 401 Unauthorized: User not logged in (handled by [Authorize] attribute)
    /// - 404 Not Found: Trip or Traveler doesn't exist
    /// - 403 Forbidden: User doesn't own the trip
    /// </summary>
    public interface ITravelerService
    {
        /// <summary>
        /// Creates a new traveler for a trip owned by the authenticated user.
        /// 
        /// Business Logic:
        /// 1. Extract UserId from JWT token (authenticated user)
        /// 2. Query database for trip with matching TripId
        /// 3. Verify trip exists, else throw EntityNotFoundException → 404
        /// 4. Verify trip.UserId == authenticated UserId (ownership), else throw ForbiddenException → 403
        /// 5. Validate request (DataAnnotations + FluentValidation)
        /// 6. Check if traveler with same email already exists in this trip
        /// 7. Create Traveler entity from CreateTravelerRequest
        /// 8. Associate traveler with trip (TravelerId = Guid.NewGuid(), TripId = tripId)
        /// 9. Set audit fields (CreatedAt, CreatedBy, UpdatedAt, LastModifiedBy)
        /// 10. Persist traveler to database via repository
        /// 11. Map Traveler to TravelerResponse
        /// 12. Return response with created traveler
        /// 
        /// Error Handling:
        /// - Trip not found → 404 Not Found (EntityNotFoundException)
        /// - User is not trip owner → 403 Forbidden (ForbiddenException)
        /// - Email already exists in trip → 409 Conflict (ValidationException)
        /// - Invalid input → 400 Bad Request (validation layer)
        /// - Database error → 500 Internal Server Error
        /// 
        /// Security Considerations:
        /// - UserId is extracted from JWT token (not from request body)
        /// - TripId comes from route parameter (not from request body)
        /// - Trip ownership is verified before creating traveler
        /// - Duplicate email check prevents adding same person twice to same trip
        /// - No way for user to create travelers on trips they don't own
        /// 
        /// Response on Success (201 Created):
        /// {
        ///     "travelerId": "550e8400-e29b-41d4-a716-446655440050",
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "firstName": "John",
        ///     "lastName": "Doe",
        ///     "email": "john@example.com",
        ///     "dateOfBirth": "1985-05-15T00:00:00Z",
        ///     "passportNumber": "12345678",
        ///     "passportExpiry": "2030-12-31T00:00:00Z",
        ///     "isPrimaryTraveler": false,
        ///     "createdAt": "2025-01-06T14:30:00Z",
        ///     "updatedAt": "2025-01-06T14:30:00Z",
        ///     ...
        /// }
        /// 
        /// Example Usage:
        /// var request = new CreateTravelerRequest
        /// {
        ///     FirstName = "John",
        ///     LastName = "Doe",
        ///     Email = "john@example.com",
        ///     DateOfBirth = new DateTime(1985, 5, 15),
        ///     PassportNumber = "12345678",
        ///     PassportExpiry = new DateTime(2030, 12, 31)
        /// };
        /// var userId = Guid.Parse(User.FindFirst("sub").Value);  // From JWT
        /// var response = await _travelerService.CreateTravelerAsync(userId, tripId, request, ct);
        /// </summary>
        Task<TravelerResponse> CreateTravelerAsync(
            Guid userId,
            Guid tripId,
            CreateTravelerRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all travelers for a specific trip owned by the authenticated user.
        /// 
        /// Business Logic:
        /// 1. Extract UserId from JWT token
        /// 2. Query database for trip with matching TripId
        /// 3. Verify trip exists, else throw EntityNotFoundException → 404
        /// 4. Verify trip.UserId == authenticated UserId (ownership), else throw ForbiddenException → 403
        /// 5. Query database for all non-deleted travelers in this trip
        /// 6. Order travelers by IsPrimaryTraveler (true first), then by CreatedAt
        /// 7. Map each Traveler entity to TravelerResponse
        /// 8. Return list of responses
        /// 
        /// Authorization:
        /// - Users can only see travelers on trips they own
        /// - Database query automatically excludes deleted travelers
        /// 
        /// Error Handling:
        /// - Trip not found → 404 Not Found
        /// - User is not trip owner → 403 Forbidden
        /// 
        /// Returns:
        /// - List of TravelerResponse (can be empty if trip has no travelers)
        /// - Never null
        /// 
        /// Response Format:
        /// [
        ///     {
        ///         "travelerId": "...",
        ///         "firstName": "John",
        ///         "lastName": "Doe",
        ///         "isPrimaryTraveler": true,
        ///         ...
        ///     },
        ///     {
        ///         "travelerId": "...",
        ///         "firstName": "Jane",
        ///         "lastName": "Doe",
        ///         "isPrimaryTraveler": false,
        ///         ...
        ///     }
        /// ]
        /// 
        /// Example Usage:
        /// var userId = Guid.Parse(User.FindFirst("sub").Value);  // From JWT
        /// var travelers = await _travelerService.GetTravelersByTripAsync(userId, tripId, ct);
        /// return Ok(travelers);  // 200 OK with array of travelers
        /// </summary>
        Task<List<TravelerResponse>> GetTravelersByTripAsync(
            Guid userId,
            Guid tripId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a specific traveler by ID.
        /// 
        /// Business Logic:
        /// 1. Extract UserId from JWT token
        /// 2. Query database for trip with matching TripId
        /// 3. Verify trip exists, else throw EntityNotFoundException → 404
        /// 4. Verify trip.UserId == authenticated UserId (ownership), else throw ForbiddenException → 403
        /// 5. Query database for traveler with matching TravelerId
        /// 6. Verify traveler exists, else throw EntityNotFoundException → 404
        /// 7. Verify traveler.TripId == tripId (traveler belongs to this trip), else throw ForbiddenException → 403
        /// 8. Verify traveler is not deleted (IsDeleted == false)
        /// 9. Map Traveler entity to TravelerResponse
        /// 10. Return response
        /// 
        /// Authorization:
        /// - Users can only access travelers on trips they own
        /// - Owner verification prevents unauthorized access
        /// 
        /// Error Handling:
        /// - Trip not found → 404 Not Found
        /// - User is not trip owner → 403 Forbidden
        /// - Traveler not found or deleted → 404 Not Found
        /// - Traveler doesn't belong to trip → 403 Forbidden
        /// 
        /// Response on Success (200 OK):
        /// {
        ///     "travelerId": "550e8400-e29b-41d4-a716-446655440050",
        ///     "tripId": "550e8400-e29b-41d4-a716-446655440000",
        ///     "firstName": "John",
        ///     "lastName": "Doe",
        ///     "email": "john@example.com",
        ///     ...
        /// }
        /// 
        /// Example Usage:
        /// var userId = Guid.Parse(User.FindFirst("sub").Value);  // From JWT
        /// var traveler = await _travelerService.GetTravelerByIdAsync(userId, tripId, travelerId, ct);
        /// return Ok(traveler);  // 200 OK with traveler details
        /// </summary>
        Task<TravelerResponse> GetTravelerByIdAsync(
            Guid userId,
            Guid tripId,
            Guid travelerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing traveler.
        /// 
        /// Business Logic:
        /// 1. Extract UserId from JWT token
        /// 2. Query database for trip with matching TripId
        /// 3. Verify trip exists, else throw EntityNotFoundException → 404
        /// 4. Verify trip.UserId == authenticated UserId (ownership), else throw ForbiddenException → 403
        /// 5. Query database for traveler with matching TravelerId
        /// 6. Verify traveler exists, else throw EntityNotFoundException → 404
        /// 7. Verify traveler.TripId == tripId (traveler belongs to this trip)
        /// 8. Validate request (DataAnnotations + FluentValidation)
        /// 9. Check if new email (if provided) already exists in this trip
        /// 10. Apply updates from UpdateTravelerRequest (only specified fields)
        /// 11. Validate updated values (age, passport expiry, etc.)
        /// 12. Update UpdatedAt timestamp (DateTime.UtcNow)
        /// 13. Update LastModifiedBy with current UserId
        /// 14. Persist changes via repository
        /// 15. Map updated Traveler to TravelerResponse
        /// 16. Return response
        /// 
        /// Partial Updates:
        /// - Request properties can be null (only non-null values are updated)
        /// - null values in request are ignored (field not updated)
        /// - Allows clients to update only the fields they need
        /// 
        /// Authorization:
        /// - Users can only update travelers on trips they own
        /// 
        /// Error Handling:
        /// - Trip not found → 404 Not Found
        /// - User is not trip owner → 403 Forbidden
        /// - Traveler not found → 404 Not Found
        /// - Email already exists in trip → 409 Conflict
        /// - Invalid data → 400 Bad Request (validation)
        /// 
        /// Response on Success (200 OK):
        /// {
        ///     "travelerId": "550e8400-e29b-41d4-a716-446655440050",
        ///     "firstName": "Jane",  // Updated
        ///     "lastName": "Doe",
        ///     "email": "jane@example.com",  // Updated
        ///     "updatedAt": "2025-01-06T15:30:00Z",  // New timestamp
        ///     ...
        /// }
        /// 
        /// Example Usage:
        /// var request = new UpdateTravelerRequest
        /// {
        ///     Email = "newemail@example.com",
        ///     DateOfBirth = new DateTime(1985, 5, 16)
        /// };
        /// var userId = Guid.Parse(User.FindFirst("sub").Value);  // From JWT
        /// var response = await _travelerService.UpdateTravelerAsync(userId, tripId, travelerId, request, ct);
        /// return Ok(response);  // 200 OK with updated traveler
        /// </summary>
        Task<TravelerResponse> UpdateTravelerAsync(
            Guid userId,
            Guid tripId,
            Guid travelerId,
            UpdateTravelerRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Soft deletes an existing traveler.
        /// 
        /// Business Logic:
        /// 1. Extract UserId from JWT token
        /// 2. Query database for trip with matching TripId
        /// 3. Verify trip exists, else throw EntityNotFoundException → 404
        /// 4. Verify trip.UserId == authenticated UserId (ownership), else throw ForbiddenException → 403
        /// 5. Query database for traveler with matching TravelerId
        /// 6. Verify traveler exists, else throw EntityNotFoundException → 404
        /// 7. Verify traveler.TripId == tripId (traveler belongs to this trip)
        /// 8. Mark traveler as deleted (IsDeleted = true, DeletedAt = DateTime.UtcNow)
        /// 9. Update LastModifiedBy with current UserId
        /// 10. Persist changes via repository
        /// 11. Return no content (204 No Content)
        /// 
        /// Soft Delete Pattern:
        /// - IsDeleted flag is set to true
        /// - DeletedAt timestamp records when deletion occurred
        /// - Record remains in database but is excluded from queries
        /// - Allows recovery if needed
        /// - Maintains referential integrity and audit trail
        /// 
        /// Authorization:
        /// - Users can only delete travelers on trips they own
        /// 
        /// Error Handling:
        /// - Trip not found → 404 Not Found
        /// - User is not trip owner → 403 Forbidden
        /// - Traveler not found → 404 Not Found
        /// - Traveler already deleted → 404 Not Found (treated as not found)
        /// 
        /// Response on Success (204 No Content):
        /// (empty response body)
        /// 
        /// Example Usage:
        /// var userId = Guid.Parse(User.FindFirst("sub").Value);  // From JWT
        /// await _travelerService.DeleteTravelerAsync(userId, tripId, travelerId, ct);
        /// return NoContent();  // 204 No Content
        /// </summary>
        Task DeleteTravelerAsync(
            Guid userId,
            Guid tripId,
            Guid travelerId,
            CancellationToken cancellationToken = default);
    }
}
