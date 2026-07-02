namespace VoyageAI.API.DTOs.Auth
{
    /// <summary>
    /// Request to refresh an access token using a refresh token.
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// The refresh token received during login.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }
}
