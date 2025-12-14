using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infraestructure.Repositories;

/// <summary>
/// Implementação temporária em memória do repositório de refresh tokens.
/// Esta implementação será substituída por Entity Framework + PostgreSQL.
/// </summary>
public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private static readonly List<RefreshToken> _refreshTokens = new();

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
        return Task.FromResult(refreshToken);
    }

    public Task SaveAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        var existingToken = _refreshTokens.FirstOrDefault(rt => rt.Id == refreshToken.Id);
        if (existingToken != null)
        {
            _refreshTokens.Remove(existingToken);
        }
        
        _refreshTokens.Add(refreshToken);
        return Task.CompletedTask;
    }

    public Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userTokens = _refreshTokens.Where(rt => rt.UserId == userId).ToList();
        foreach (var token in userTokens)
        {
            token.Revoke();
        }
        return Task.CompletedTask;
    }
}