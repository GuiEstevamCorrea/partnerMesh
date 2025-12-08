using Application.UseCases.RefreshToken.DTO;

namespace Application.Interfaces.IUseCases;

public interface IRefreshTokenUseCase
{
    Task<RefreshTokenResult> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}