using Application.UseCases.ActivateDeactivateUser.DTO;

namespace Application.Interfaces.IUseCases;

public interface IActivateDeactivateUserUseCase
{
    /// <summary>
    /// Ativa um usuário inativo
    /// </summary>
    Task<ActivateDeactivateUserResult> ActivateUserAsync(Guid targetUserId, Guid currentUserId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Desativa um usuário ativo (respeitando a regra: vetor deve ter ao menos 1 admin)
    /// </summary>
    Task<ActivateDeactivateUserResult> DeactivateUserAsync(Guid targetUserId, Guid currentUserId, CancellationToken cancellationToken = default);
}