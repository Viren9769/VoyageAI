namespace VoyageAI.API.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object for successful login response.
    /// 
    /// This DTO is returned to the client after successful authentication.
    /// It contains the JWT token, token expiry information, and essential user details.
    /// 
    /// Response Structure:
    /// {
    ///     "token": "eyJhbGciOiJIUzI1NiIs...",
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
    /// Client Usage:
    /// 1. Client receives response
    /// 2. Stores token in localStorage or sessionStorage
    /// 3. Uses token in Authorization header: "Authorization: Bearer {token}"
    /// 4. Displays user information in UI
    /// 5. Should refresh token before expiry or re-authenticate
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the JWT authentication token.
        /// 
        /// The token is a signed JSON Web Token (JWT) that contains:
        /// - UserId
        /// - Email
        /// - FirstName
        /// - LastName
        /// - Expiration time (iat + ExpiryMinutes)
        /// 
        /// Client Usage:
        /// - Store in localStorage or sessionStorage
        /// - Include in Authorization header as: "Bearer {token}"
        /// - Use for all authenticated API requests
        /// 
        /// Token Structure:
        /// {
        ///     "header": { "alg": "HS256", "typ": "JWT" },
        ///     "payload": {
        ///         "sub": "{UserId}",
        ///         "email": "{Email}",
        ///         "firstName": "{FirstName}",
        ///         "lastName": "{LastName}",
        ///         "iss": "VoyageAI",
        ///         "aud": "VoyageAIApp",
        ///         "exp": {expiration_time},
        ///         "iat": {issued_time}
        ///     }
        /// }
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the refresh token string.
        /// 
        /// Refresh tokens are long-lived tokens that can be exchanged for new access tokens
        /// without requiring the user to provide credentials again.
        /// 
        /// Client Usage:
        /// - Store securely in localStorage or secure httpOnly cookie
        /// - Use to call /api/auth/refresh-token endpoint before access token expires
        /// - When refreshed, receive new access and refresh tokens
        /// - Provides seamless experience without user re-authentication
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token expiration date in UTC.
        /// 
        /// This is a DateTime in UTC format indicating when the JWT token will expire.
        /// Format example: "2025-01-06T14:30:00Z"
        /// 
        /// Client Usage:
        /// - Compare current time to this expiry
        /// - If current time > expiry, token is invalid
        /// - Before expiry approaches, refresh token or prompt user to re-login
        /// - Prevents using expired tokens for API requests
        /// 
        /// Security Notes:
        /// - Use this to determine when to refresh or re-authenticate
        /// - Don't trust client-side expiry from decoded token (can be tampered)
        /// - Server validates token expiry on each request
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Gets or sets the token type. Always "Bearer" for JWT.
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Gets or sets the authenticated user's information.
        /// 
        /// Contains only safe, non-sensitive user fields that the client needs.
        /// Does NOT include PasswordHash, IsActive, or server-side audit fields.
        /// 
        /// This information can be used to:
        /// - Display user profile in UI
        /// - Populate user menu/header
        /// - Show personal information
        /// - Cache user data on client-side
        /// </summary>
        public UserDto User { get; set; } = null!;
    }

    /// <summary>
    /// Data Transfer Object for user information in login response.</summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the user's unique identifier (UserId).
        /// 
        /// This is a GUID that uniquely identifies the user in the system.
        /// Used for:
        /// - Identifying which user is making API requests
        /// - Creating relationships (trips, activities, etc.)
        /// - Audit trails and logging
        /// 
        /// Format example: "550e8400-e29b-41d4-a716-446655440000"
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// 
        /// Used for:
        /// - Personalizing UI messages ("Hello, John!")
        /// - Displaying in user profile
        /// - Sharing with other users in collaborative features
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's last name.
        /// 
        /// Used for:
        /// - Personalizing UI messages
        /// - Displaying in user profile
        /// - Sharing with other users in collaborative features
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's email address.
        /// 
        /// This is the unique login identifier for the user.
        /// Used for:
        /// - Displaying contact information
        /// - Email-based notifications
        /// - Account recovery
        /// - User identification in collaborative features
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL to the user's profile image.
        /// 
        /// Can be null if user hasn't uploaded a profile picture.
        /// Used for:
        /// - Displaying user avatar in UI
        /// - User profile picture on profile page
        /// - Showing user identity in collaborative features (comments, activities, etc.)
        /// 
        /// Example: "https://cdn.voyageai.com/profiles/user-id-123/avatar.jpg"
        /// </summary>
        public string? ProfileImageUrl { get; set; }
    }
}
