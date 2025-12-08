using Application.UseCases.Logout.DTO;

namespace Application.Interfaces.IUseCases;

public interface ILogoutUseCase
{
    Task<LogoutResult> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);
}