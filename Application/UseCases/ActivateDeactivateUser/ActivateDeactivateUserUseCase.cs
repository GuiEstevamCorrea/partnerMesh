using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ActivateDeactivateUser.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.ActivateDeactivateUser;

public sealed class ActivateDeactivateUserUseCase : IActivateDeactivateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public ActivateDeactivateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ActivateDeactivateUserResult> ActivateUserAsync(Guid targetUserId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Buscar usuário atual para verificar permissões
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser is null)
        {
            return ActivateDeactivateUserResult.Failure("Usuário atual não encontrado.");
        }

        // Verificar permissões
        var permissionCheck = await ValidatePermissions(currentUser, targetUserId, cancellationToken);
        if (!permissionCheck.IsValid)
        {
            return ActivateDeactivateUserResult.Failure(permissionCheck.ErrorMessage);
        }

        var targetUser = permissionCheck.TargetUser!;

        // Verificar se o usuário já está ativo
        if (targetUser.Active)
        {
            return ActivateDeactivateUserResult.Failure("Usuário já está ativo.");
        }

        // Ativar o usuário
        targetUser.Activate();

        // Salvar alterações
        await _userRepository.SaveAsync(targetUser, cancellationToken);

        // Retornar resultado
        var userInfo = CreateUserStatusInfo(targetUser);
        return ActivateDeactivateUserResult.Success(userInfo, "ativado");
    }

    public async Task<ActivateDeactivateUserResult> DeactivateUserAsync(Guid targetUserId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Buscar usuário atual para verificar permissões
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser is null)
        {
            return ActivateDeactivateUserResult.Failure("Usuário atual não encontrado.");
        }

        // Verificar permissões
        var permissionCheck = await ValidatePermissions(currentUser, targetUserId, cancellationToken);
        if (!permissionCheck.IsValid)
        {
            return ActivateDeactivateUserResult.Failure(permissionCheck.ErrorMessage);
        }

        var targetUser = permissionCheck.TargetUser!;

        // Verificar se o usuário já está inativo
        if (!targetUser.Active)
        {
            return ActivateDeactivateUserResult.Failure("Usuário já está inativo.");
        }

        // Validação crítica: se é Admin de Vetor, verificar se é o único admin ativo do vetor
        if (targetUser.Permission == PermissionEnum.AdminVetor)
        {
            var canDeactivate = await CanDeactivateVetorAdmin(targetUser, cancellationToken);
            if (!canDeactivate.IsValid)
            {
                return ActivateDeactivateUserResult.Failure(canDeactivate.ErrorMessage);
            }
        }

        // Desativar o usuário
        targetUser.Deactivate();

        // Salvar alterações
        await _userRepository.SaveAsync(targetUser, cancellationToken);

        // Retornar resultado
        var userInfo = CreateUserStatusInfo(targetUser);
        return ActivateDeactivateUserResult.Success(userInfo, "desativado");
    }

    private async Task<ValidationResult> ValidatePermissions(Domain.Entities.User currentUser, Guid targetUserId, CancellationToken cancellationToken)
    {
        // Verificar se o usuário atual está ativo
        if (!currentUser.Active)
        {
            return ValidationResult.Invalid("Usuário atual está inativo.");
        }

        // Buscar usuário alvo
        var targetUser = await _userRepository.GetByIdAsync(targetUserId, cancellationToken);
        if (targetUser is null)
        {
            return ValidationResult.Invalid("Usuário alvo não encontrado.");
        }

        // Admin Global pode ativar/desativar qualquer usuário
        if (currentUser.Permission == PermissionEnum.AdminGlobal)
        {
            return ValidationResult.Valid(targetUser);
        }

        // Admin de Vetor só pode ativar/desativar usuários do mesmo vetor
        if (currentUser.Permission == PermissionEnum.AdminVetor)
        {
            var currentUserVetorIds = currentUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId);
            var targetUserVetorIds = targetUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId);
            
            if (!currentUserVetorIds.Any(currentVetorId => targetUserVetorIds.Contains(currentVetorId)))
            {
                return ValidationResult.Invalid("Você só pode ativar/desativar usuários do mesmo vetor.");
            }

            return ValidationResult.Valid(targetUser);
        }

        return ValidationResult.Invalid("Você não tem permissão para ativar/desativar usuários.");
    }

    private async Task<ValidationResult> CanDeactivateVetorAdmin(Domain.Entities.User adminUser, CancellationToken cancellationToken)
    {
        // Buscar todos os usuários
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        
        // Para cada vetor do admin que está sendo desativado
        foreach (var userVetor in adminUser.UserVetores.Where(uv => uv.Active))
        {
            // Contar quantos admins ativos existem nesse vetor (excluindo o que está sendo desativado)
            var activeAdminsInVetor = allUsers
                .Where(u => u.Id != adminUser.Id && // Excluir o usuário que está sendo desativado
                           u.Active && // Deve estar ativo
                           u.Permission == PermissionEnum.AdminVetor && // Deve ser Admin de Vetor
                           u.UserVetores.Any(uv => uv.Active && uv.VetorId == userVetor.VetorId)) // Deve pertencer ao mesmo vetor
                .Count();

            // Se não há outros admins ativos neste vetor, não pode desativar
            if (activeAdminsInVetor == 0)
            {
                return ValidationResult.Invalid($"Não é possível desativar este usuário pois ele é o único administrador ativo do vetor. Cada vetor deve ter pelo menos um administrador ativo.");
            }
        }

        return ValidationResult.Valid();
    }

    private static UserStatusInfo CreateUserStatusInfo(Domain.Entities.User user)
    {
        return new UserStatusInfo(
            user.Id,
            user.Name,
            user.Email,
            user.Active,
            user.Permission.ToString(),
            user.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId));
    }

    private sealed record ValidationResult(bool IsValid, string ErrorMessage = "", Domain.Entities.User? TargetUser = null)
    {
        public static ValidationResult Valid(Domain.Entities.User? targetUser = null) => new(true, "", targetUser);
        public static ValidationResult Invalid(string message) => new(false, message);
    }
}