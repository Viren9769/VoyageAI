using Microsoft.AspNetCore.Mvc;
using VoyageAI.API.DTOs.Auth;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Controllers
{
    /// <summary>
    /// API controller for user authentication operations.
    /// 
    /// This controller handles HTTP requests for user registration and login.
    /// It is intentionally thin - all business logic resides in the service layer.
    /// 
    /// Architecture:
    /// HTTP Request → Validation (DataAnnotations + FluentValidation)
    ///     → AuthController (routes to service)
    ///     → IAuthService (business logic)
    ///     → IUserRepository (data access)
    ///     → Database
    /// 
    /// HTTP Endpoints:
    /// - POST /api/auth/register - Register a new user
    /// - POST /api/auth/login - Authenticate user and get JWT token
    /// 
    /// Design Principles:
    /// - No business logic in controller (belongs in service)
    /// - No database access in controller (belongs in repository)
    /// - Minimal error handling (complex cases handled by middleware)
    /// - Clean separation of concerns
    /// - Service calls are wrapped in try-catch for specific business exceptions
    /// 
    /// Dependencies:
    /// - IAuthService: Injected via constructor dependency injection
    /// 
    /// Validation:
    /// - Input validation: DataAnnotations on DTOs
    /// - Complex validation: FluentValidation (IValidation{T})
    /// - Validation failures: 400 Bad Request (handled by middleware)
    /// 
    /// Error Handling:
    /// - InvalidOperationException → 409 Conflict (email already exists)
    /// - UnauthorizedAccessException → 401 Unauthorized (invalid credentials)
    /// - Other exceptions → 500 Internal Server Error (handled by middleware)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// The authentication service.
        /// Handles all business logic for registration and login.
        /// Injected via constructor dependency injection.
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Logger for this controller.
        /// Used to log HTTP requests and errors.
        /// </summary>
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the AuthController class.
        /// 
        /// Constructor Injection:
        /// - IAuthService: Provides registration and login business logic
        /// - ILogger: Provides logging capabilities
        /// 
        /// Both are injected by the ASP.NET Core dependency injection container
        /// configured in Program.cs.
        /// </summary>
        /// <param name="authService">The authentication service</param>
        /// <param name="logger">Logger instance for this controller</param>
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registers a new user account.
        /// 
        /// Endpoint: POST /api/auth/register
        /// 
        /// Request Body:
        /// {
        ///     "firstName": "John",
        ///     "lastName": "Doe",
        ///     "email": "john@example.com",
        ///     "password": "SecurePass@123",
        ///     "phone": "1234567890",
        ///     "country": "USA"
        /// }
        /// 
        /// Request Validation:
        /// 1. DataAnnotations validation (required fields, email format, length)
        /// 2. FluentValidation (password complexity, email uniqueness)
        /// 3. Invalid input → 400 Bad Request (handled by validation middleware)
        /// 
        /// Registration Process:
        /// 1. Service checks email uniqueness
        /// 2. Service hashes password with BCrypt
        /// 3. Service creates User entity
        /// 4. Service saves user to database
        /// 5. Service generates JWT token
        /// 6. Service maps User to UserDto
        /// 7. Controller returns LoginResponse
        /// 
        /// Success Response (201 Created):
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
        /// Error Responses:
        /// - 400 Bad Request: Validation error (missing fields, invalid format, etc.)
        ///   Response: { "errors": { "Email": ["Email is required"], ... } }
        /// 
        /// - 409 Conflict: Email already registered
        ///   Response: { "message": "Email 'john@example.com' is already registered." }
        /// 
        /// - 500 Internal Server Error: Unexpected server error
        ///   Response: { "message": "An error occurred during registration. Please try again." }
        /// 
        /// Security Considerations:
        /// - Password is hashed with BCrypt before database storage
        /// - Email must be unique (prevents duplicate accounts)
        /// - User starts as active (IsActive = true)
        /// - JWT token generated for immediate login
        /// - Password never logged or exposed in response
        /// 
        /// HTTP Status Codes:
        /// - 201 Created: User successfully registered
        /// - 400 Bad Request: Input validation failed
        /// - 409 Conflict: Email already exists
        /// - 500 Internal Server Error: Unexpected error
        /// </summary>
        /// <param name="request">The registration request containing user details</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>
        /// 201 Created with LoginResponse containing JWT token and user info.
        /// Caller can use token in Authorization header: "Bearer {token}"
        /// </returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponse>> Register(
            [FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            // Validate input
            if (request == null)
            {
                return BadRequest(new { message = "Request body cannot be empty" });
            }

            _logger.LogInformation("Register request received for email: {Email}", request.Email);

            try
            {
                // Call service to handle registration business logic
                // Service handles:
                // - Email uniqueness check
                // - Password hashing (BCrypt)
                // - User entity creation
                // - Database persistence
                // - JWT token generation
                // - DTO mapping
                var response = await _authService.RegisterAsync(request, cancellationToken);

                _logger.LogInformation("User registered successfully for email: {Email}", request.Email);

                // Return 201 Created with Location header
                // Client receives token and user information
                // Token should be stored and sent in Authorization header for future requests
                return CreatedAtAction(nameof(Register), response);
            }
            catch (InvalidOperationException ex)
            {
                // Email already exists
                _logger.LogWarning("Registration failed: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });  // 409 Conflict
            }
            catch (ArgumentException ex)
            {
                // Invalid input in request body
                _logger.LogWarning("Registration failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });  // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Unexpected error
                _logger.LogError(ex, "Unexpected error during registration for email: {Email}", request.Email);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred during registration. Please try again." });  // 500
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// 
        /// Endpoint: POST /api/auth/login
        /// 
        /// Request Body:
        /// {
        ///     "email": "john@example.com",
        ///     "password": "SecurePass@123"
        /// }
        /// 
        /// Request Validation:
        /// 1. DataAnnotations validation (required fields, email format)
        /// 2. FluentValidation (additional rules if needed)
        /// 3. Invalid input → 400 Bad Request (handled by validation middleware)
        /// 
        /// Login Process:
        /// 1. Service queries database for user by email
        /// 2. If user not found → throw UnauthorizedAccessException (401)
        /// 3. Service verifies password hash using BCrypt
        /// 4. If password incorrect → throw UnauthorizedAccessException (401)
        /// 5. If password correct, service generates JWT token
        /// 6. Service maps User to UserDto
        /// 7. Controller returns LoginResponse with token
        /// 
        /// Success Response (200 OK):
        /// {
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "expires": "2025-01-06T14:30:00Z",
        ///     "user": {
        ///         "userId": "550e8400-e29b-41d4-a716-446655440000",
        ///         "firstName": "John",
        ///         "lastName": "Doe",
        ///         "email": "john@example.com",
        ///         "profileImageUrl": "https://..."
        ///     }
        /// }
        /// 
        /// Error Responses:
        /// - 400 Bad Request: Validation error (missing fields, invalid email format)
        ///   Response: { "errors": { "Email": ["Email is required"], ... } }
        /// 
        /// - 401 Unauthorized: Invalid email or password
        ///   Response: { "message": "Invalid email or password." }
        ///   Note: Generic message prevents user enumeration attacks
        /// 
        /// - 500 Internal Server Error: Unexpected server error
        ///   Response: { "message": "An error occurred during login. Please try again." }
        /// 
        /// Using the Token:
        /// 1. Client stores token from response (localStorage/sessionStorage)
        /// 2. Include in all subsequent authenticated requests:
        ///    Authorization: Bearer {token}
        /// 3. Server validates token signature and expiry
        /// 4. If valid, request proceeds. If invalid/expired, return 401 Unauthorized
        /// 5. When token expires, user must login again
        /// 
        /// Security Considerations:
        /// - Password verified using BCrypt.Verify (constant-time comparison)
        /// - Prevents timing attacks
        /// - Generic error message for all failures (prevents user enumeration)
        /// - Login attempts can be monitored/rate-limited by infrastructure
        /// - Token should only be sent over HTTPS
        /// - Never expose password in logs or responses
        /// 
        /// JWT Token Claims:
        /// - sub: UserId
        /// - email: User's email
        /// - firstName: User's first name
        /// - lastName: User's last name
        /// - iss: Issuer (VoyageAI)
        /// - aud: Audience (VoyageAIApp)
        /// - exp: Expiration time
        /// - iat: Issued at time
        /// 
        /// HTTP Status Codes:
        /// - 200 OK: Authentication successful, token returned
        /// - 400 Bad Request: Validation error
        /// - 401 Unauthorized: Invalid email or password
        /// - 500 Internal Server Error: Unexpected error
        /// </summary>
        /// <param name="request">The login request containing email and password</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>
        /// 200 OK with LoginResponse containing JWT token and user info.
        /// Token should be included in Authorization header for authenticated requests: "Bearer {token}"
        /// </returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            // Validate input
            if (request == null)
            {
                return BadRequest(new { message = "Request body cannot be empty" });
            }

            _logger.LogInformation("Login request received for email: {Email}", request.Email);

            try
            {
                // Call service to handle login business logic
                // Service handles:
                // - User lookup by email
                // - Password verification (BCrypt)
                // - JWT token generation
                // - DTO mapping
                var response = await _authService.LoginAsync(request, cancellationToken);

                _logger.LogInformation("User logged in successfully for email: {Email}", request.Email);

                // Return 200 OK with LoginResponse
                // Client receives token and user information
                // Token should be stored and sent in Authorization header for future requests
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Invalid email or password
                _logger.LogWarning("Login failed: Invalid credentials for email: {Email}", request.Email);
                return Unauthorized(new { message = ex.Message });  // 401 Unauthorized
            }
            catch (ArgumentException ex)
            {
                // Invalid input in request body
                _logger.LogWarning("Login failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });  // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Unexpected error
                _logger.LogError(ex, "Unexpected error during login for email: {Email}", request.Email);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred during login. Please try again." });  // 500
            }
        }

        /// <summary>
        /// Refreshes an access token using a valid refresh token.
        /// 
        /// Endpoint: POST /api/auth/refresh-token
        /// 
        /// Request Body:
        /// {
        ///     "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// }
        /// 
        /// Success Response (200 OK):
        /// {
        ///     "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "refreshToken": "existing-or-new-refresh-token",
        ///     "expiresAt": "2025-01-06T15:30:00Z",
        ///     "tokenType": "Bearer",
        ///     "expiresIn": 3600
        /// }
        /// 
        /// Error Response (401 Unauthorized):
        /// {
        ///     "success": false,
        ///     "message": "Invalid email or password.",
        ///     "errors": [
        ///         {
        ///             "propertyName": null,
        ///             "errorMessage": "Invalid or expired token",
        ///             "errorCode": "INVALID_CREDENTIALS"
        ///         }
        ///     ],
        ///     "timestamp": "2025-01-06T14:00:00Z"
        /// }
        /// 
        /// Validation:
        /// - Refresh token must be provided and non-empty
        /// - Token must exist in database and not be revoked
        /// - Token must not be expired
        /// - Associated user account must be active
        /// 
        /// Client Usage:
        /// 1. Store refresh token from login response
        /// 2. Before access token expires, call this endpoint with refresh token
        /// 3. Receive new access token
        /// 4. Continue using new access token for API requests
        /// 5. If refresh token is invalid/expired, user must login again
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(
            RefreshTokenRequest request,
            CancellationToken ct)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request, ct);
                return Ok(response);  // 200
            }
            catch (ArgumentException ex)
            {
                // Validation error
                _logger.LogWarning(ex, "Validation error during token refresh");
                return BadRequest(new { message = ex.Message });  // 400
            }
            catch (UnauthorizedAccessException ex)
            {
                // Invalid refresh token
                _logger.LogWarning(ex, "Invalid refresh token used");
                return Unauthorized(new { message = ex.Message });  // 401
            }
            catch (InvalidOperationException ex)
            {
                // Account suspended or other business rule violation
                _logger.LogWarning(ex, "Business rule violation during token refresh");
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { message = ex.Message });  // 403
            }
            catch (Exception ex)
            {
                // Unexpected error
                _logger.LogError(ex, "Unexpected error during token refresh");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred. Please try again." });  // 500
            }
        }
    }
}
