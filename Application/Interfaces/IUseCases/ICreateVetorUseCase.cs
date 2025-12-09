using Application.UseCases.CreateVetor.DTO;

namespace Application.Interfaces.IUseCases;

public interface ICreateVetorUseCase
{
    /// <summary>
    /// Cria um novo vetor no sistema. Apenas Admin Global pode realizar esta operação.
    /// </summary>
    /// <param name="request">Dados do vetor a ser criado</param>
    /// <param name="currentUserId">ID do usuário atual (deve ser Admin Global)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação com informações do vetor criado</returns>
    Task<CreateVetorResult> CreateAsync(CreateVetorRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}