using Application.UseCases.ListVetores.DTO;

namespace Application.Interfaces.IUseCases;

public interface IListVetoresUseCase
{
    /// <summary>
    /// Lista vetores com filtros e paginação, respeitando as permissões do usuário atual.
    /// Admin Global vê todos os vetores. Admin de Vetor vê apenas seus próprios vetores.
    /// </summary>
    /// <param name="request">Filtros e parâmetros de paginação</param>
    /// <param name="currentUserId">ID do usuário atual</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de vetores com informações resumidas</returns>
    Task<ListVetoresResult> ListAsync(ListVetoresRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}