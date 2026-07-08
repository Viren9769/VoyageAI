using Microsoft.EntityFrameworkCore;
using VoyageAI.API.Data;
using VoyageAI.API.Models.Entities;
using VoyageAI.API.Repositories.Interfaces;

namespace VoyageAI.API.Repositories
{
    /// <summary>
    /// EF Core implementation of IRefreshTokenRepository.
    /// </summary>
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly VoyageDbContext _context;

        public RefreshTokenRepository(VoyageDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, ct);
        }

        public async Task<IEnumerable<RefreshToken>> GetValidTokensByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId &&
                            !rt.IsRevoked &&
                            rt.ExpiresAt > now)
                .ToListAsync(ct);
        }

        public async Task CreateAsync(RefreshToken token, CancellationToken ct = default)
        {
            await _context.RefreshTokens.AddAsync(token, ct);
        }

        public async Task UpdateAsync(RefreshToken token, CancellationToken ct = default)
        {
            _context.RefreshTokens.Update(token);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid refreshTokenId, CancellationToken ct = default)
        {
            var token = await _context.RefreshTokens.FindAsync(new object[] { refreshTokenId }, cancellationToken: ct);
            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
            }
        }

        public async Task RevokeAllUserTokensAsync(Guid userId, string? reason = null, CancellationToken ct = default)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(ct);

            foreach (var token in tokens)
            {
                token.Revoke(reason);
            }

            _context.RefreshTokens.UpdateRange(tokens);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}
