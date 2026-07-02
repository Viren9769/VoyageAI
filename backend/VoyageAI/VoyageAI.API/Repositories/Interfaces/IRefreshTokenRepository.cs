using VoyageAI.API.Models.Entities;

namespace VoyageAI.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository abstraction for refresh token data access.
    /// </summary>
    public interface IRefreshTokenRepository
    {
        /// <summary>
        /// Gets a refresh token by its token string (hashed value).
        /// </summary>
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);

        /// <summary>
        /// Gets all valid (non-revoked, non-expired) refresh tokens for a user.
        /// </summary>
        Task<IEnumerable<RefreshToken>> GetValidTokensByUserIdAsync(Guid userId, CancellationToken ct = default);

        /// <summary>
        /// Creates a new refresh token for a user.
        /// </summary>
        Task CreateAsync(RefreshToken token, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing refresh token (e.g., to revoke it).
        /// </summary>
        Task UpdateAsync(RefreshToken token, CancellationToken ct = default);

        /// <summary>
        /// Deletes a refresh token by ID.
        /// </summary>
        Task DeleteAsync(Guid refreshTokenId, CancellationToken ct = default);

        /// <summary>
        /// Revokes all tokens for a user (e.g., on logout or password change).
        /// </summary>
        Task RevokeAllUserTokensAsync(Guid userId, string? reason = null, CancellationToken ct = default);

        /// <summary>
        /// Persists changes to database.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
