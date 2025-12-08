using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.Logout.DTO;

namespace Application.UseCases.Logout;

public sealed class LogoutUseCase : ILogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutUseCase(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<LogoutResult> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return LogoutResult.Failure("Refresh token é obrigatório.");
        }

        // Buscar refresh token
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (refreshToken is null)
        {
            // Se o token não existe, consideramos como logout bem-sucedido
            // pois o objetivo (invalidar a sessão) já foi alcançado
            return LogoutResult.Success();
        }

        // Revogar o refresh token
        refreshToken.Revoke();
        await _refreshTokenRepository.SaveAsync(refreshToken, cancellationToken);

        // Opcionalmente, revogar todos os tokens do usuário para maior segurança
        await _refreshTokenRepository.RevokeAllByUserIdAsync(refreshToken.UserId, cancellationToken);

        return LogoutResult.Success();
    }
}