using Application.UseCases.CreateUser.DTO;

namespace Application.Interfaces.IUseCases;

public interface ICreateUserUseCase
{
    Task<CreateUserResult> CreateAsync(CreateUserRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}