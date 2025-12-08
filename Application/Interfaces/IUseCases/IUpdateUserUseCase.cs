using Application.UseCases.UpdateUser.DTO;

namespace Application.Interfaces.IUseCases;

public interface IUpdateUserUseCase
{
    Task<UpdateUserResult> UpdateAsync(Guid userId, UpdateUserRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}