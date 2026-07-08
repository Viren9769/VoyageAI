namespace VoyageAI.API.DTOs.Auth
{
    /// <summary>
    /// Response after refreshing an access token.
    /// Similar to LoginResponse but includes both new access and refresh tokens.
    /// </summary>
    public class RefreshTokenResponse
    {
        /// <summary>
        /// New access token (JWT).
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// New or existing refresh token.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// When the access token expires.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Token type (always "Bearer").
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Seconds until access token expires.
        /// </summary>
        public int ExpiresIn => (int)(ExpiresAt - DateTime.UtcNow).TotalSeconds;
    }
}
