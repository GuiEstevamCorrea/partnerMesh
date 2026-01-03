using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.UpdateUser.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.UpdateUser;

public sealed class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IVetorRepository _vetorRepository;

    public UpdateUserUseCase(
        IUserRepository userRepository,
        IVetorRepository vetorRepository)
    {
        _userRepository = userRepository;
        _vetorRepository = vetorRepository;
    }

    public async Task<UpdateUserResult> UpdateAsync(Guid userId, UpdateUserRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Buscar usuário a ser atualizado
        var userToUpdate = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (userToUpdate is null)
        {
            return UpdateUserResult.Failure("Usuário não encontrado.");
        }

        // Buscar usuário atual para verificar permissões
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser is null)
        {
            return UpdateUserResult.Failure("Usuário atual não encontrado.");
        }

        // Verificar se o usuário atual tem permissão para atualizar o usuário
        if (!CanUpdateUser(currentUser, userToUpdate))
        {
            return UpdateUserResult.Failure("Você não tem permissão para atualizar este usuário.");
        }

        // Validar e aplicar atualizações
        var validationResult = await ValidateAndApplyUpdates(userToUpdate, request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return UpdateUserResult.Failure(validationResult.ErrorMessage);
        }

        // Salvar alterações
        await _userRepository.SaveAsync(userToUpdate, cancellationToken);

        // Retornar resultado de sucesso
        var userInfo = new UserInfo(
            userToUpdate.Id,
            userToUpdate.Name,
            userToUpdate.Email,
            userToUpdate.Permission.ToString(),
            userToUpdate.Active,
            userToUpdate.CreatedAt,
            userToUpdate.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId));

        return UpdateUserResult.Success(userInfo);
    }

    private bool CanUpdateUser(Domain.Entities.User currentUser, Domain.Entities.User userToUpdate)
    {
        // Admin Global pode atualizar qualquer usuário
        if (currentUser.Permission == PermissionEnum.AdminGlobal)
        {
            return true;
        }

        // Admin de Vetor só pode atualizar usuários do mesmo vetor
        if (currentUser.Permission == PermissionEnum.AdminVetor)
        {
            var currentUserVetorIds = currentUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId);
            var userToUpdateVetorIds = userToUpdate.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId);
            
            return currentUserVetorIds.Any(currentVetorId => userToUpdateVetorIds.Contains(currentVetorId));
        }

        return false;
    }

    private async Task<ValidationResult> ValidateAndApplyUpdates(Domain.Entities.User user, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        // Validar email se fornecido
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            if (!IsValidEmail(request.Email))
            {
                return ValidationResult.Invalid("Email inválido.");
            }

            var emailExists = await _userRepository.EmailExistsExcludingUserAsync(request.Email, user.Id, cancellationToken);
            if (emailExists)
            {
                return ValidationResult.Invalid("Email já está em uso por outro usuário.");
            }

            user.UpdateEmail(request.Email);
        }

        // Validar e atualizar nome se fornecido
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            if (request.Name.Length < 2)
            {
                return ValidationResult.Invalid("Nome deve ter pelo menos 2 caracteres.");
            }

            user.UpdateName(request.Name);
        }

        // Validar e atualizar senha se fornecida
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            if (request.Password.Length < 6)
            {
                return ValidationResult.Invalid("Senha deve ter pelo menos 6 caracteres.");
            }

            // Hash da nova senha
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.UpdatePassword(passwordHash);
        }

        // Validar e atualizar permissão se fornecida
        if (request.Permission.HasValue)
        {
            var permissionValidation = await ValidatePermissionUpdate(user, request.Permission.Value, request.VetorId, cancellationToken);
            if (!permissionValidation.IsValid)
            {
                return permissionValidation;
            }

            user.UpdatePermission(request.Permission.Value);
        }

        // Validar e atualizar vetor se fornecido
        if (request.VetorId.HasValue)
        {
            if (user.Permission == PermissionEnum.AdminGlobal)
            {
                return ValidationResult.Invalid("Admin Global não pode ter vetor associado.");
            }

            var vetor = await _vetorRepository.GetByIdAsync(request.VetorId.Value, cancellationToken);
            if (vetor is null)
            {
                return ValidationResult.Invalid("Vetor não encontrado.");
            }

            user.UpdateVetor(request.VetorId.Value);
        }
        else if (request.VetorId == null && request.Permission.HasValue && request.Permission.Value != PermissionEnum.AdminGlobal)
        {
            // Se VetorId é null explicitamente e não é Admin Global, remove vetores
            user.ClearVetores();
        }

        return ValidationResult.Valid();
    }

    private async Task<ValidationResult> ValidatePermissionUpdate(Domain.Entities.User user, PermissionEnum newPermission, Guid? vetorId, CancellationToken cancellationToken)
    {
        // Regras para Admin Global
        if (newPermission == PermissionEnum.AdminGlobal)
        {
            if (vetorId.HasValue)
            {
                return ValidationResult.Invalid("Admin Global não pode ter vetor associado.");
            }
            return ValidationResult.Valid();
        }

        // Regras para outras permissões
        if (newPermission == PermissionEnum.AdminVetor || newPermission == PermissionEnum.Operador)
        {
            // Se não foi fornecido VetorId, usar o vetor atual do usuário
            var targetVetorId = vetorId ?? user.UserVetores.FirstOrDefault(uv => uv.Active)?.VetorId;
            
            if (!targetVetorId.HasValue)
            {
                return ValidationResult.Invalid($"Usuário com permissão {newPermission} deve ter um vetor associado.");
            }

            var vetor = await _vetorRepository.GetByIdAsync(targetVetorId.Value, cancellationToken);
            if (vetor is null)
            {
                return ValidationResult.Invalid("Vetor não encontrado.");
            }
        }

        return ValidationResult.Valid();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private sealed record ValidationResult(bool IsValid, string ErrorMessage = "")
    {
        public static ValidationResult Valid() => new(true);
        public static ValidationResult Invalid(string message) => new(false, message);
    }
}