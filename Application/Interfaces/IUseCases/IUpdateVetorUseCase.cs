using Application.UseCases.UpdateVetor.DTO;

namespace Application.Interfaces.IUseCases;

public interface IUpdateVetorUseCase
{
    /// <summary>
    /// Atualiza um vetor existente no sistema. Apenas Admin Global pode realizar esta operação.
    /// </summary>
    /// <param name="vetorId">ID do vetor a ser atualizado</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <param name="currentUserId">ID do usuário atual (deve ser Admin Global)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com informações do vetor atualizado</returns>
    Task<UpdateVetorResult> UpdateAsync(Guid vetorId, UpdateVetorRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}