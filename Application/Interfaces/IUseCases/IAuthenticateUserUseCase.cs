using Application.UseCases.AuthenticateUser.DTO;

namespace Application.Interfaces.IUseCases;

public interface IAuthenticateUserUseCase
{
    Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken = default);
}