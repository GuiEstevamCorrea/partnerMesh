using Application.UseCases.ListUsers.DTO;

namespace Application.Interfaces.IUseCases;

public interface IListUsersUseCase
{
    /// <summary>
    /// Lista usuários com filtros e paginação, respeitando as permissões do usuário atual
    /// </summary>
    Task<ListUsersResult> ListAsync(ListUsersRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}