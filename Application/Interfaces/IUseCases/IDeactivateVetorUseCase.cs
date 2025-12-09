using Application.UseCases.DeactivateVetor.DTO;

namespace Application.Interfaces.IUseCases;

public interface IDeactivateVetorUseCase
{
    /// <summary>
    /// Inativa um vetor no sistema. Apenas Admin Global pode realizar esta operação.
    /// Valida se existe administrador ativo antes de permitir a desativação.
    /// </summary>
    /// <param name="vetorId">ID do vetor a ser inativado</param>
    /// <param name="currentUserId">ID do usuário atual (deve ser Admin Global)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com informações do vetor inativado</returns>
    Task<DeactivateVetorResult> DeactivateAsync(Guid vetorId, Guid currentUserId, CancellationToken cancellationToken = default);
}