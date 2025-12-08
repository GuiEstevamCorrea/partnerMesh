using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task SaveAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}