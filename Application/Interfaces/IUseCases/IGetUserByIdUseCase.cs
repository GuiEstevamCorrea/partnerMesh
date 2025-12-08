using Application.UseCases.GetUserById.DTO;

namespace Application.Interfaces.IUseCases;

public interface IGetUserByIdUseCase
{
    /// <summary>
    /// Obtém dados detalhados de um usuário por ID, respeitando as permissões do usuário atual
    /// </summary>
    Task<GetUserByIdResult> GetAsync(Guid userId, Guid currentUserId, CancellationToken cancellationToken = default);
}