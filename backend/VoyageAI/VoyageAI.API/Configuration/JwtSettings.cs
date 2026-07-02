namespace VoyageAI.API.Configuration
{
    /// <summary>
    /// JWT configuration settings for token generation and validation.
    /// Binds to appsettings.json Jwt section.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the secret key used for signing JWT tokens.
        /// Must be at least 32 characters for HS256 algorithm.
        /// </summary>
        public string Secret { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the issuer of the JWT token.
        /// Identifies the principal that created the token.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the audience of the JWT token.
        /// Identifies the recipients that the JWT is intended for.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token expiry time in minutes.
        /// Default is 60 minutes.
        /// </summary>
        public int ExpiryMinutes { get; set; } = 60;
    }
}
