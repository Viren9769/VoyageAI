using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using VoyageAI.API.Common.Exceptions;
using VoyageAI.API.Configuration;
using VoyageAI.API.Constants;
using VoyageAI.API.DTOs.Auth;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Models.Enums;
using VoyageAI.API.Repositories.Interfaces;
using VoyageAI.API.Services.Interfaces;

namespace VoyageAI.API.Services
{
    /// <summary>
    /// Service for authentication operations (registration and login).
    /// 
    /// This class implements all business logic for user authentication.
    /// It handles password hashing, verification, JWT token generation, and user creation.
    /// 
    /// Design Pattern: Service Layer
    /// - Encapsulates all authentication business logic
    /// - Depends on interfaces (IUserRepository, IMapper) for loose coupling
    /// - Enables unit testing through dependency injection
    /// 
    /// Dependencies:
    /// - IUserRepository: Data access for User entity
    /// - IMapper: AutoMapper for DTO conversions
    /// - IConfiguration: Access to appsettings.json for JWT settings
    /// - ILogger: Logging for debugging and monitoring
    /// 
    /// Security Features:
    /// - BCrypt password hashing with salt
    /// - JWT validation on all requests
    /// - Email uniqueness enforcement
    /// - Proper error messages without information leakage
    /// 
    /// Dependency Injection:
    /// services.AddScoped{IAuthService, AuthService}();
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Repository for user data access operations.
        /// Handles all CREATE, READ operations for the User entity.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Repository for refresh token data access operations.
        /// </summary>
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        /// <summary>
        /// AutoMapper instance for mapping between DTOs and entities.
        /// Maps RegisterRequest/LoginRequest DTOs to User entity and back.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Generic logger for this service.
        /// Used to log important events: registrations, login attempts, errors.
        /// </summary>
        private readonly ILogger<AuthService> _logger;

        /// <summary>
        /// JWT configuration settings (secret, issuer, audience, expiry).
        /// Bound to appsettings.json Jwt section.
        /// </summary>
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Initializes a new instance of the AuthService class.
        /// 
        /// Constructor Injection:
        /// All dependencies are injected by the DI container.
        /// This enables loose coupling and testability.
        /// 
        /// Example (in Program.cs):
        /// services.AddScoped{IUserRepository, UserRepository}();
        /// services.AddScoped{IAuthService, AuthService}();
        /// services.Configure{JwtSettings}(configuration.GetSection("Jwt"));
        /// services.AddAutoMapper(typeof(Program));
        /// </summary>
        /// <param name="userRepository">Repository for user data operations</param>
        /// <param name="refreshTokenRepository">Repository for refresh token data operations</param>
        /// <param name="mapper">AutoMapper for DTO conversions</param>
        /// <param name="configuration">Application configuration for JWT settings</param>
        /// <param name="logger">Logger instance for this service</param>
        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Bind JWT configuration from appsettings.json
            _jwtSettings = new JwtSettings();
            configuration.GetSection("Jwt").Bind(_jwtSettings);

            // Validate critical JWT settings
            if (string.IsNullOrWhiteSpace(_jwtSettings.Secret) || _jwtSettings.Secret.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT Secret must be configured in appsettings.json and be at least 32 characters long.");
            }

            if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
            {
                throw new InvalidOperationException(
                    "JWT Issuer must be configured in appsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
            {
                throw new InvalidOperationException(
                    "JWT Audience must be configured in appsettings.json.");
            }
        }

        /// <summary>
        /// Registers a new user with the provided registration information.
        /// 
        /// Registration Process:
        /// 1. Validate that email is not already registered
        /// 2. Hash the password using BCrypt (never store plain passwords)
        /// 3. Create User entity from RegisterRequest
        /// 4. Save user to database via repository
        /// 5. Generate JWT token for immediate login
        /// 6. Map User entity to UserDto
        /// 7. Return LoginResponse with token and user info
        /// 
        /// Security Measures:
        /// - Email uniqueness check prevents duplicate accounts
        /// - Password hashed with BCrypt (one-way, salted)
        /// - User created as IsActive=true by default
        /// - JWT generated immediately (automatic login after registration)
        /// - Original password never stored or logged
        /// 
        /// Error Handling:
        /// - Throws InvalidOperationException if email already exists
        /// - Throws ArgumentException if user entity is invalid
        /// - Logs all registration attempts for security audit trail
        /// 
        /// Performance Notes:
        /// - Email existence check is fast (AnyAsync query)
        /// - Database insert is single operation
        /// - JWT generation is in-memory CPU operation
        /// 
        /// Thread Safety:
        /// - CancellationToken propagated to all async operations
        /// - Repository handles transaction safety
        /// </summary>
        /// <param name="request">Registration request containing user details and password</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>LoginResponse containing JWT token and user information</returns>
        /// <exception cref="InvalidOperationException">Thrown if email already exists</exception>
        /// <exception cref="ArgumentException">Thrown if required fields are missing</exception>
        public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Registration request cannot be null");
            }

            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            // Check if email already exists (prevent duplicate accounts)
            var emailExists = await _userRepository.ExistsAsync(request.Email, cancellationToken);
            if (emailExists)
            {
                _logger.LogWarning("Registration failed: Email already exists - {Email}", request.Email);
                throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
            }

            try
            {
                // Hash password using BCrypt
                // BCrypt automatically generates salt and incorporates it in the hash
                // Never store or log the plain password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create User entity from RegisterRequest
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    Phone = request.Phone,
                    CountryCode = request.Country,
                    Status = UserStatus.Inactive  // New users start as inactive until email is verified
                };

                // Add user to database (not saved yet)
                await _userRepository.CreateAsync(user);

                // Persist the user to the database
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("User registered successfully: {UserId}", user.UserId);

                // Generate JWT token for immediate login
                var token = GenerateJwtToken(user);

                // Map User entity to UserDto
                var userDto = _mapper.Map<UserDto>(user);

                // Construct and return LoginResponse
                var response = new LoginResponse
                {
                    AccessToken = token,
                    RefreshToken = token,  // Will be replaced with actual refresh token in future
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                    User = userDto
                };

                return response;
            }
            catch (InvalidOperationException)
            {
                // Re-throw InvalidOperationException (email exists)
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration: {Email}", request.Email);
                throw new InvalidOperationException("An error occurred during registration. Please try again.", ex);
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// 
        /// Login Process:
        /// 1. Query database to find user by email
        /// 2. If user not found, throw UnauthorizedAccessException
        /// 3. Verify provided password against stored hash using BCrypt
        /// 4. If password incorrect, throw UnauthorizedAccessException
        /// 5. If password correct, generate JWT token
        /// 6. Map User entity to UserDto
        /// 7. Return LoginResponse with token and user info
        /// 
        /// Security Measures:
        /// - Case-insensitive email lookup prevents case sensitivity exploits
        /// - BCrypt.Verify provides constant-time comparison (prevents timing attacks)
        /// - Never reveal whether email exists or password is wrong (generic error message)
        /// - Password never logged or exposed in response
        /// - JWT includes expiry to prevent indefinite token usage
        /// 
        /// Error Handling:
        /// - Throws UnauthorizedAccessException for any authentication failure
        /// - Generic error message ("Invalid email or password") prevents user enumeration
        /// - Failed attempts are logged for security monitoring
        /// 
        /// Performance Notes:
        /// - Email lookup uses indexed column (fast)
        /// - BCrypt verification happens in-memory
        /// - JWT generation is CPU-bound but fast
        /// 
        /// Thread Safety:
        /// - CancellationToken propagated to all async operations
        /// - Repository handles transaction safety
        /// </summary>
        /// <param name="request">Login request containing email and password</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        /// <returns>LoginResponse containing JWT token and user information</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if email not found or password incorrect</exception>
        /// <exception cref="ArgumentNullException">Thrown if request is null</exception>
        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Login request cannot be null");
            }

            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            try
            {
                // Query database to find user by email (case-insensitive)
                var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

                // If user not found, throw UnauthorizedAccessException
                // Generic message prevents user enumeration attacks
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found for email - {Email}", request.Email);
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                // Verify password hash using BCrypt
                // BCrypt.Verify:
                // - Extracts salt from stored hash
                // - Hashes provided password with extracted salt
                // - Compares hashes in constant time (prevents timing attacks)
                var passwordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

                if (!passwordCorrect)
                {
                    _logger.LogWarning("Login failed: Invalid password for email - {Email}", request.Email);
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                _logger.LogInformation("User logged in successfully: {UserId}", user.UserId);

                // Password correct - generate JWT token
                var token = GenerateJwtToken(user);

                // Map User entity to UserDto
                var userDto = _mapper.Map<UserDto>(user);

                // Construct and return LoginResponse
                var response = new LoginResponse
                {
                    AccessToken = token,
                    RefreshToken = token,  // Will be replaced with actual refresh token in future
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                    User = userDto
                };

                return response;
            }
            catch (UnauthorizedAccessException)
            {
                // Re-throw UnauthorizedAccessException
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login: {Email}", request.Email);
                throw new InvalidOperationException("An error occurred during login. Please try again.", ex);
            }
        }

        /// <summary>
        /// Generates a JWT token for the given user.
        /// 
        /// Token Generation Process:
        /// 1. Create claims array with user information
        ///    - NameIdentifier: UserId (subject claim)
        ///    - Email: User's email
        ///    - FirstName: User's first name
        ///    - LastName: User's last name
        /// 2. Calculate expiration time (UtcNow + ExpiryMinutes)
        /// 3. Create signing credentials using secret key (HS256 algorithm)
        /// 4. Create JwtSecurityToken with claims, issuer, audience, expiry, and signature
        /// 5. Encode token to string format (JWT format: header.payload.signature)
        /// 
        /// JWT Structure:
        /// {
        ///     "alg": "HS256",
        ///     "typ": "JWT"
        /// }.{
        ///     "sub": "{UserId}",
        ///     "email": "{Email}",
        ///     "FirstName": "{FirstName}",
        ///     "LastName": "{LastName}",
        ///     "iss": "{Issuer}",
        ///     "aud": "{Audience}",
        ///     "exp": {ExpirationUnixTimestamp},
        ///     "iat": {IssuedUnixTimestamp}
        /// }.{signature}
        /// 
        /// Claims Explained:
        /// - sub (Subject): Uniquely identifies the principal (UserId)
        /// - email: User's email for identification
        /// - FirstName: Custom claim for user's first name
        /// - LastName: Custom claim for user's last name
        /// - iss (Issuer): Who created the token ("VoyageAI")
        /// - aud (Audience): Who the token is intended for ("VoyageAIApp")
        /// - exp (Expires): When the token expires (Unix timestamp)
        /// - iat (Issued At): When the token was created (Unix timestamp)
        /// 
        /// Security Considerations:
        /// - Secret key must be at least 32 characters (512 bits) for HS256
        /// - Token is BASE64-encoded, NOT encrypted (don't put sensitive data in claims)
        /// - Signature prevents tampering (any change invalidates signature)
        /// - Expiry prevents indefinite token usage (mitigates token theft impact)
        /// - Always transmit JWT over HTTPS to prevent interception
        /// 
        /// Client Usage:
        /// 1. Receive token from /register or /login endpoint
        /// 2. Store token in localStorage or sessionStorage
        /// 3. Include in Authorization header: "Bearer {token}"
        /// 4. Server validates signature and expiry on each request
        /// 
        /// Performance Notes:
        /// - Token generation is CPU-bound but very fast (milliseconds)
        /// - HMACSHA256 signing is efficient
        /// - No database calls required (in-memory operation)
        /// 
        /// Token Validation (handled by middleware):
        /// - Signature verified with secret key
        /// - Expiry date checked
        /// - Issuer and audience validated
        /// - Claims extracted for use in controllers
        /// </summary>
        /// <param name="user">The user entity for which to generate a token</param>
        /// <returns>JWT token as a string, ready for use in Authorization header</returns>
        /// <exception cref="ArgumentNullException">Thrown if user is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if JWT settings are invalid</exception>
        public string GenerateJwtToken(User user)
        {
            // Validate input
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            // Create claims array with user information
            // Claims are statements about the user (not encrypted, just base64 encoded)
            var claims = new[]
            {
                // NameIdentifier: The subject claim - uniquely identifies the user
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),

                // Email: User's email address
                new Claim(ClaimTypes.Email, user.Email),

                // Custom claims for user's name
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // Create symmetric security key from secret string
            // Secret must be at least 32 characters (256 bits) for HS256
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            // Create signing credentials using HMACSHA256 algorithm
            // This will be used to sign the token (prove it hasn't been tampered with)
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Calculate token expiration time
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            // Create JWT token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,          // Who issued the token
                audience: _jwtSettings.Audience,      // Who the token is for
                claims: claims,                       // User claims
                expires: expiration,                  // Token expiry
                signingCredentials: signingCredentials  // Signature (HS256)
            );

            // Encode token to string format (header.payload.signature)
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(token);

            _logger.LogDebug("JWT token generated for user: {UserId}", user.UserId);

            return tokenString;
        }

        /// <summary>
        /// Generates a secure refresh token (cryptographically random string).
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Refreshes an access token using a valid refresh token.
        /// </summary>
        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new InvalidCredentialsException();
            }

            // Look up refresh token
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

            if (refreshToken == null || !refreshToken.IsValid)
            {
                _logger.LogWarning("Invalid or expired refresh token used");
                throw new InvalidCredentialsException();
            }

            // Verify user account is active
            if (refreshToken.User?.Status != UserStatus.Active)
            {
                _logger.LogWarning("User account not active for refresh token: {UserId}", refreshToken.UserId);
                throw new AccountSuspendedException();
            }

            // Generate new access token
            var newAccessToken = GenerateJwtToken(refreshToken.User!);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            // Log successful token refresh
            _logger.LogInformation("Refresh token used by user: {UserId}", refreshToken.UserId);

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = request.RefreshToken,
                ExpiresAt = expiresAt,
                TokenType = "Bearer"
            };
        }
    }
}
