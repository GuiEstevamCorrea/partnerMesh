using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ChangePassword.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.ChangePassword;

public sealed class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly IUserRepository _userRepository;

    public ChangePasswordUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ChangePasswordResult> ChangeOwnPasswordAsync(Guid userId, ChangeOwnPasswordRequest request, CancellationToken cancellationToken = default)
    {
        // Validar entrada
        var validationResult = ValidatePasswordChange(request.NewPassword);
        if (!validationResult.IsValid)
        {
            return ChangePasswordResult.Failure(validationResult.ErrorMessage);
        }

        // Buscar usuário
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ChangePasswordResult.Failure("Usuário não encontrado.");
        }

        // Verificar se o usuário está ativo
        if (!user.Active)
        {
            return ChangePasswordResult.Failure("Usuário inativo não pode alterar a senha.");
        }

        // Verificar senha atual
        if (!user.PasswordMatches(request.CurrentPassword))
        {
            return ChangePasswordResult.Failure("Senha atual incorreta.");
        }

        // Verificar se a nova senha é diferente da atual
        if (user.PasswordMatches(request.NewPassword))
        {
            return ChangePasswordResult.Failure("A nova senha deve ser diferente da senha atual.");
        }

        // Gerar hash da nova senha e atualizar
        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatePassword(newPasswordHash);

        // Salvar alterações
        await _userRepository.SaveAsync(user, cancellationToken);

        return ChangePasswordResult.Success();
    }

    public async Task<ChangePasswordResult> ResetPasswordAsync(Guid targetUserId, ResetPasswordRequest request, Guid adminUserId, CancellationToken cancellationToken = default)
    {
        // Validar entrada
        var validationResult = ValidatePasswordChange(request.NewPassword);
        if (!validationResult.IsValid)
        {
            return ChangePasswordResult.Failure(validationResult.ErrorMessage);
        }

        // Buscar usuário administrador
        var adminUser = await _userRepository.GetByIdAsync(adminUserId, cancellationToken);
        if (adminUser is null)
        {
            return ChangePasswordResult.Failure("Administrador não encontrado.");
        }

        // Verificar se o usuário é Admin Global
        if (adminUser.Permission != PermissionEnum.AdminGlobal)
        {
            return ChangePasswordResult.Failure("Apenas administradores globais podem resetar senhas.");
        }

        // Verificar se o admin está ativo
        if (!adminUser.Active)
        {
            return ChangePasswordResult.Failure("Administrador inativo não pode resetar senhas.");
        }

        // Buscar usuário alvo
        var targetUser = await _userRepository.GetByIdAsync(targetUserId, cancellationToken);
        if (targetUser is null)
        {
            return ChangePasswordResult.Failure("Usuário alvo não encontrado.");
        }

        // Verificar se não está tentando resetar a própria senha (deve usar o método próprio)
        if (adminUserId == targetUserId)
        {
            return ChangePasswordResult.Failure("Para alterar sua própria senha, use o endpoint específico.");
        }

        // Verificar se a nova senha é diferente da atual (opcional, mas boa prática)
        if (targetUser.PasswordMatches(request.NewPassword))
        {
            return ChangePasswordResult.Failure("A nova senha deve ser diferente da senha atual do usuário.");
        }

        // Gerar hash da nova senha e atualizar
        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        targetUser.UpdatePassword(newPasswordHash);

        // Salvar alterações
        await _userRepository.SaveAsync(targetUser, cancellationToken);

        return ChangePasswordResult.Success();
    }

    private static ValidationResult ValidatePasswordChange(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            return ValidationResult.Invalid("Nova senha é obrigatória.");
        }

        if (newPassword.Length < 6)
        {
            return ValidationResult.Invalid("Nova senha deve ter pelo menos 6 caracteres.");
        }

        if (newPassword.Length > 100)
        {
            return ValidationResult.Invalid("Nova senha deve ter no máximo 100 caracteres.");
        }

        // Validação básica de complexidade
        var hasUpperCase = newPassword.Any(char.IsUpper);
        var hasLowerCase = newPassword.Any(char.IsLower);
        var hasDigit = newPassword.Any(char.IsDigit);

        if (!hasUpperCase || !hasLowerCase || !hasDigit)
        {
            return ValidationResult.Invalid("Nova senha deve conter pelo menos uma letra maiúscula, uma minúscula e um número.");
        }

        return ValidationResult.Valid();
    }

    private sealed record ValidationResult(bool IsValid, string ErrorMessage = "")
    {
        public static ValidationResult Valid() => new(true);
        public static ValidationResult Invalid(string message) => new(false, message);
    }
}