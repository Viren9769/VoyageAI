namespace VoyageAI.API.Models.Entities
{
    /// <summary>
    /// Represents a refresh token for JWT token refresh flow.
    /// Refresh tokens allow users to obtain new access tokens without re-entering credentials.
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Unique identifier for this refresh token.
        /// </summary>
        public Guid RefreshTokenId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Foreign key to the User who owns this token.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Navigation property to the User entity.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// The hashed refresh token value.
        /// NEVER store plain tokens in database (security best practice).
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// When this refresh token expires and is no longer valid.
        /// Default: 7 days from creation.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// When this refresh token was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether this token has been revoked (user logout, password change, etc).
        /// Revoked tokens cannot be used to refresh access tokens.
        /// </summary>
        public bool IsRevoked { get; set; } = false;

        /// <summary>
        /// When this token was revoked (null if not revoked).
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// Reason for revocation (e.g., "User logout", "Password changed", "Suspicious activity").
        /// </summary>
        public string? RevocationReason { get; set; }

        /// <summary>
        /// Checks if this token is valid for use.
        /// Valid if: not expired, not revoked, and belongs to an active user.
        /// </summary>
        public bool IsValid =>
            DateTime.UtcNow < ExpiresAt &&
            !IsRevoked &&
            User?.Status == Enums.UserStatus.Active;

        /// <summary>
        /// Revokes this token (logs user out, prevents token reuse).
        /// </summary>
        public void Revoke(string? reason = null)
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            RevocationReason = reason;
        }
    }
}
