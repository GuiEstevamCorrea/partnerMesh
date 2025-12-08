using Application.UseCases.ChangePassword.DTO;

namespace Application.Interfaces.IUseCases;

public interface IChangePasswordUseCase
{
    /// <summary>
    /// Permite ao usuário alterar sua própria senha
    /// </summary>
    Task<ChangePasswordResult> ChangeOwnPasswordAsync(Guid userId, ChangeOwnPasswordRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Permite ao administrador global resetar a senha de qualquer usuário
    /// </summary>
    Task<ChangePasswordResult> ResetPasswordAsync(Guid targetUserId, ResetPasswordRequest request, Guid adminUserId, CancellationToken cancellationToken = default);
}