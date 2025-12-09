using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.UpdateVetor.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.UpdateVetor;

public sealed class UpdateVetorUseCase : IUpdateVetorUseCase
{
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;

    public UpdateVetorUseCase(
        IVetorRepository vetorRepository,
        IUserRepository userRepository)
    {
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
    }

    public async Task<UpdateVetorResult> UpdateAsync(Guid vetorId, UpdateVetorRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Validações de entrada e permissões
        var validationResult = await ValidateRequestAsync(vetorId, request, currentUserId, cancellationToken);
        if (!validationResult.IsValid)
        {
            return UpdateVetorResult.Failure(validationResult.ErrorMessage);
        }

        var vetor = validationResult.Vetor!;

        // Aplicar atualizações específicas
        var updateResult = await ValidateAndApplyUpdates(vetor, request, cancellationToken);
        if (!updateResult.IsValid)
        {
            return UpdateVetorResult.Failure(updateResult.ErrorMessage);
        }

        // Salvar alterações
        await _vetorRepository.SaveAsync(vetor, cancellationToken);

        // Retornar resultado de sucesso
        var vetorInfo = new VetorInfo(
            vetor.Id,
            vetor.Name,
            vetor.Email,
            vetor.Active,
            vetor.CreatedAt);

        return UpdateVetorResult.Success(vetorInfo);
    }

    private async Task<ValidationResult> ValidateRequestAsync(Guid vetorId, UpdateVetorRequest request, Guid currentUserId, CancellationToken cancellationToken)
    {
        // Verificar se há pelo menos um campo para atualizar
        if (string.IsNullOrWhiteSpace(request.Name) && string.IsNullOrWhiteSpace(request.Email) && !request.Active.HasValue)
        {
            return ValidationResult.Invalid("Pelo menos um campo deve ser fornecido para atualização.");
        }

        // Obter usuário atual para verificar permissões
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser == null)
        {
            return ValidationResult.Invalid("Usuário atual não encontrado.");
        }

        // Verificar se o usuário está ativo
        if (!currentUser.Active)
        {
            return ValidationResult.Invalid("Usuário atual está inativo.");
        }

        // Verificar se o usuário é Admin Global
        if (currentUser.Permission != PermissionEnum.AdminGlobal)
        {
            return ValidationResult.Invalid("Apenas administradores globais podem atualizar vetores.");
        }

        // Buscar vetor a ser atualizado
        var vetor = await _vetorRepository.GetByIdAsync(vetorId, cancellationToken);
        if (vetor == null)
        {
            return ValidationResult.Invalid("Vetor não encontrado.");
        }

        return ValidationResult.Valid(vetor);
    }

    private async Task<ValidationResult> ValidateAndApplyUpdates(Domain.Entities.Vetor vetor, UpdateVetorRequest request, CancellationToken cancellationToken)
    {
        // Validar e atualizar nome se fornecido
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            if (request.Name.Length < 2)
            {
                return ValidationResult.Invalid("Nome do vetor deve ter pelo menos 2 caracteres.");
            }

            if (request.Name.Length > 100)
            {
                return ValidationResult.Invalid("Nome do vetor deve ter no máximo 100 caracteres.");
            }

            // Verificar se nome já existe (excluindo o vetor atual)
            var nameExists = await _vetorRepository.NameExistsExcludingVetorAsync(request.Name, vetor.Id, cancellationToken);
            if (nameExists)
            {
                return ValidationResult.Invalid("Nome do vetor já está em uso por outro vetor.");
            }

            vetor.UpdateName(request.Name);
        }

        // Validar e atualizar email se fornecido
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            if (!IsValidEmail(request.Email))
            {
                return ValidationResult.Invalid("Email do vetor inválido.");
            }

            // Verificar se email já existe (excluindo o vetor atual)
            var emailExists = await _vetorRepository.EmailExistsExcludingVetorAsync(request.Email, vetor.Id, cancellationToken);
            if (emailExists)
            {
                return ValidationResult.Invalid("Email do vetor já está em uso por outro vetor.");
            }

            vetor.UpdateEmail(request.Email);
        }

        // Atualizar status se fornecido
        if (request.Active.HasValue)
        {
            if (request.Active.Value)
            {
                vetor.Activate();
            }
            else
            {
                // Validação crítica: verificar se existem usuários ativos associados ao vetor antes de desativar
                var canDeactivate = await CanDeactivateVetor(vetor, cancellationToken);
                if (!canDeactivate.IsValid)
                {
                    return canDeactivate;
                }

                vetor.Deactivate();
            }
        }

        return ValidationResult.Valid();
    }

    private async Task<ValidationResult> CanDeactivateVetor(Domain.Entities.Vetor vetor, CancellationToken cancellationToken)
    {
        // Buscar todos os usuários
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        
        // Verificar se há usuários ativos associados ao vetor
        var activeUsersInVetor = allUsers
            .Where(u => u.Active && 
                       u.UserVetores.Any(uv => uv.Active && uv.VetorId == vetor.Id))
            .Count();

        if (activeUsersInVetor > 0)
        {
            return ValidationResult.Invalid($"Não é possível desativar este vetor pois existem {activeUsersInVetor} usuário(s) ativo(s) associado(s). Desative ou mova os usuários primeiro.");
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

    private sealed record ValidationResult(bool IsValid, string ErrorMessage = "", Domain.Entities.Vetor? Vetor = null)
    {
        public static ValidationResult Valid(Domain.Entities.Vetor? vetor = null) => new(true, "", vetor);
        public static ValidationResult Invalid(string message) => new(false, message);
    }
}