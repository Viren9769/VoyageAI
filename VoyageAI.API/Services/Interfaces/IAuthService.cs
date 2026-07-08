using VoyageAI.API.DTOs.Auth;
using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for authentication operations.
    /// 
    /// This interface defines the contract for all authentication business logic.
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
    /// Injected as: services.AddScoped{IAuthService, AuthService}()
    /// 
    /// Usage in Controller:
    /// [ApiController]
    /// [Route("api/[controller]")]
    /// public class AuthController : ControllerBase
    /// {
    ///     private readonly IAuthService _authService;
    ///     
    ///     public AuthController(IAuthService authService)
    ///     {
    ///         _authService = authService;
    ///     }
    ///     
    ///     [HttpPost("register")]
    ///     public async Task{ActionResult{LoginResponse}} Register(RegisterRequest request, CancellationToken ct)
    ///     {
    ///         var response = await _authService.RegisterAsync(request, ct);
    ///         return Created("", response);
    ///     }
    /// }
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user with the provided information.
        /// 
        /// Registration Flow:
        /// 1. Validate input (DataAnnotations + FluentValidation)
        /// 2. Check if email already exists (prevent duplicates)
        /// 3. If email exists → throw ConflictException (409 response)
        /// 4. Validate password complexity (uppercase, lowercase, number, special char)
        /// 5. Hash password using BCrypt.Net
        /// 6. Create User entity from RegisterRequest
        /// 7. Persist user to database via repository
        /// 8. Generate JWT token for immediate login
        /// 9. Map User to UserDto
        /// 10. Return LoginResponse with token and user
        /// 
        /// Business Logic:
        /// - Email uniqueness validation (case-insensitive)
        /// - Password hashing using BCrypt (never store plain passwords)
        /// - User entity creation with defaults (UserId, CreatedAt, UpdatedAt, IsActive)
        /// - JWT token generation with user claims
        /// 
        /// Error Handling:
        /// - Invalid input → 400 Bad Request (validation layer)
        /// - Email already exists → 409 Conflict
        /// - Database error → 500 Internal Server Error
        /// 
        /// Security Considerations:
        /// - Password must be hashed using BCrypt (never plain text)
        /// - Email is case-insensitive for comparison
        /// - IsActive defaults to true on registration (user starts as active)
        /// - JWT token issued immediately after successful registration
        /// 
        /// Response on Success (201 Created):
        /// {
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "expires": "2025-01-06T14:30:00Z",
        ///     "user": {
        ///         "userId": "550e8400-e29b-41d4-a716-446655440000",
        ///         "firstName": "John",
        ///         "lastName": "Doe",
        ///         "email": "john@example.com",
        ///         "profileImageUrl": null
        ///     }
        /// }
        /// 
        /// Example Usage:
        /// var request = new RegisterRequest
        /// {
        ///     FirstName = "John",
        ///     LastName = "Doe",
        ///     Email = "john@example.com",
        ///     Password = "SecurePass@123",
        ///     Phone = "1234567890",
        ///     Country = "USA"
        /// };
        /// 
        /// try
        /// {
        ///     var response = await _authService.RegisterAsync(request, cancellationToken);
        ///     return Created("", response);
        /// }
        /// catch (InvalidOperationException ex)
        /// {
        ///     return Conflict(new { message = ex.Message });  // Email already exists
        /// }
        /// </summary>
        /// <param name="request">User registration data (validated by FluentValidation)</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>LoginResponse containing JWT token and user information</returns>
        /// <exception cref="InvalidOperationException">Thrown if email already exists (409 Conflict)</exception>
        /// <exception cref="ArgumentNullException">Thrown if request is null</exception>
        /// <exception cref="ArgumentException">Thrown if request properties are invalid</exception>
        Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// 
        /// Login Flow:
        /// 1. Validate input (email and password required)
        /// 2. Query database to find user by email
        /// 3. If user not found → throw UnauthorizedException (401 response)
        /// 4. If user found, verify password using BCrypt.Net
        /// 5. If password incorrect → throw UnauthorizedException (401 response)
        /// 6. If password correct, generate JWT token
        /// 7. Map User to UserDto
        /// 8. Return LoginResponse with token and user
        /// 
        /// Business Logic:
        /// - Case-insensitive email lookup
        /// - Secure password verification using BCrypt (constant-time comparison)
        /// - JWT token generation with user claims
        /// - Token includes expiry, issuer, and audience
        /// 
        /// Error Handling:
        /// - Invalid input → 400 Bad Request (validation layer)
        /// - Email not found → 401 Unauthorized
        /// - Password incorrect → 401 Unauthorized
        /// - Database error → 500 Internal Server Error
        /// 
        /// Security Considerations:
        /// - Passwords are NEVER returned in response
        /// - BCrypt provides constant-time comparison to prevent timing attacks
        /// - Failed authentication returns generic "401 Unauthorized" (no email validation leakage)
        /// - Token expiry is checked on each request
        /// - Token is signed with secret key (cannot be forged)
        /// 
        /// Response on Success (200 OK):
        /// {
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "expires": "2025-01-06T14:30:00Z",
        ///     "user": {
        ///         "userId": "550e8400-e29b-41d4-a716-446655440000",
        ///         "firstName": "John",
        ///         "lastName": "Doe",
        ///         "email": "john@example.com",
        ///         "profileImageUrl": "https://cdn.example.com/avatar.jpg"
        ///     }
        /// }
        /// 
        /// Example Usage:
        /// var request = new LoginRequest
        /// {
        ///     Email = "john@example.com",
        ///     Password = "SecurePass@123"
        /// };
        /// 
        /// try
        /// {
        ///     var response = await _authService.LoginAsync(request, cancellationToken);
        ///     return Ok(response);
        /// }
        /// catch (UnauthorizedAccessException)
        /// {
        ///     return Unauthorized(new { message = "Invalid email or password" });
        /// }
        /// </summary>
        /// <param name="request">User login credentials (validated by FluentValidation)</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>LoginResponse containing JWT token and user information</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if email not found or password incorrect (401 Unauthorized)</exception>
        /// <exception cref="ArgumentNullException">Thrown if request is null</exception>
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a JWT token for the given user.
        /// 
        /// Token Generation:
        /// 1. Create JWT claims with user information
        /// 2. Calculate expiry time (UtcNow + ExpiryMinutes from JwtSettings)
        /// 3. Sign token with secret key using HS256 algorithm
        /// 4. Serialize token to string
        /// 
        /// JWT Structure:
        /// Header: { "alg": "HS256", "typ": "JWT" }
        /// Payload: {
        ///     "sub": "{UserId}",
        ///     "email": "{Email}",
        ///     "firstName": "{FirstName}",
        ///     "lastName": "{LastName}",
        ///     "iss": "VoyageAI",
        ///     "aud": "VoyageAIApp",
        ///     "exp": {expiration_unix_timestamp},
        ///     "iat": {issued_unix_timestamp}
        /// }
        /// Signature: HMACSHA256(header.payload, secret)
        /// 
        /// Claims Explained:
        /// - sub (subject): UserId - identifies the user
        /// - email: User's email for identification
        /// - firstName: User's first name for personalization
        /// - lastName: User's last name for personalization
        /// - iss (issuer): "VoyageAI" - who created the token
        /// - aud (audience): "VoyageAIApp" - who the token is for
        /// - exp (expiration): When token expires (required for token validity)
        /// - iat (issued at): When token was created
        /// 
        /// JWT Bearer Usage:
        /// Client includes token in Authorization header:
        /// Authorization: Bearer {token}
        /// 
        /// Server validates:
        /// 1. Signature is valid (not tampered)
        /// 2. Expiration has not passed
        /// 3. Issuer matches expected value
        /// 4. Audience matches expected value
        /// 
        /// Token Expiry:
        /// - Configured in appsettings.json: Jwt.ExpiryMinutes
        /// - Default: 60 minutes
        /// - Prevents indefinite token usage
        /// - Requires re-login after expiry (or use refresh tokens in future)
        /// 
        /// Security Considerations:
        /// - Secret key must be at least 32 characters for HS256 (configured in appsettings)
        /// - Token should be sent over HTTPS only (enforced by infrastructure)
        /// - Token expiry prevents indefinite access after account compromise
        /// - Claims are public (base64 encoded, not encrypted), don't include sensitive data
        /// 
        /// Example Usage:
        /// var user = await _userRepository.GetByEmailAsync("john@example.com", ct);
        /// string token = _authService.GenerateJwtToken(user);
        /// // token is now valid for API requests with Authorization: Bearer {token}
        /// </summary>
        /// <param name="user">The user entity for which to generate a token</param>
        /// <returns>JWT token as a string, ready for use in Authorization header</returns>
        /// <exception cref="ArgumentNullException">Thrown if user is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if JWT settings are not configured</exception>
        string GenerateJwtToken(User user);

        /// <summary>
        /// Refreshes an access token using a valid refresh token.
        /// 
        /// Refresh Token Flow:
        /// 1. Validate refresh token is not empty
        /// 2. Look up refresh token in database
        /// 3. Check if token exists, is not revoked, and not expired
        /// 4. Load associated user
        /// 5. Verify user account is active (not suspended/deleted)
        /// 6. Generate new access token for user
        /// 7. Optionally rotate refresh token (issue new refresh token)
        /// 8. Return RefreshTokenResponse with new tokens
        /// 
        /// Business Logic:
        /// - Refresh tokens provide seamless re-authentication
        /// - Prevent users from losing access on token expiry
        /// - Enable password-less continuation if credentials still valid
        /// - Can implement rotation for enhanced security
        /// 
        /// Error Handling:
        /// - Invalid/not found token → 401 Unauthorized
        /// - Token expired → 401 Unauthorized
        /// - Token revoked → 401 Unauthorized
        /// - User account suspended/deleted → 403 Forbidden
        /// - Database error → 500 Internal Server Error
        /// 
        /// Security Considerations:
        /// - Refresh tokens stored hashed in database (like passwords)
        /// - Tokens can be revoked on logout, password change, etc.
        /// - Token rotation prevents token reuse attacks
        /// - Short-lived access token + long-lived refresh token pattern
        /// 
        /// Response on Success (200 OK):
        /// {
        ///     "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "refreshToken": "new-refresh-token-or-same-existing-token",
        ///     "expiresAt": "2025-01-06T15:30:00Z",
        ///     "tokenType": "Bearer",
        ///     "expiresIn": 3600
        /// }
        /// </summary>
        /// <param name="request">RefreshTokenRequest containing the refresh token</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>RefreshTokenResponse with new access token and refresh token</returns>
        /// <exception cref="InvalidCredentialsException">Thrown if refresh token is invalid or expired</exception>
        /// <exception cref="AccountSuspendedException">Thrown if user account is suspended</exception>
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
    }
}
