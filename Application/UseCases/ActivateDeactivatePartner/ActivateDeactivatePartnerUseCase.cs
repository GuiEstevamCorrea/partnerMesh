using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ActivateDeactivatePartner.DTO;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.UseCases.ActivateDeactivatePartner;

public sealed class ActivateDeactivatePartnerUseCase : IActivateDeactivatePartnerUseCase
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVetorRepository _vetorRepository;

    public ActivateDeactivatePartnerUseCase(
        IPartnerRepository partnerRepository,
        IUserRepository userRepository,
        IVetorRepository vetorRepository)
    {
        _partnerRepository = partnerRepository;
        _userRepository = userRepository;
        _vetorRepository = vetorRepository;
    }

    public async Task<ActivateDeactivatePartnerResult> ActivateDeactivateAsync(Guid partnerId, ActivateDeactivatePartnerRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Validar entrada básica
        var basicValidationResult = ValidateBasicInput(partnerId, request);
        if (!basicValidationResult.IsValid)
        {
            return ActivateDeactivatePartnerResult.Failure(basicValidationResult.ErrorMessage);
        }

        // Buscar o parceiro existente
        var existingPartner = await _partnerRepository.GetByIdAsync(partnerId, cancellationToken);
        if (existingPartner is null)
        {
            return ActivateDeactivatePartnerResult.Failure("Parceiro não encontrado.");
        }

        // Verificar se já está no status solicitado
        if (existingPartner.Active == request.Active)
        {
            var statusText = request.Active ? "ativo" : "inativo";
            return ActivateDeactivatePartnerResult.Failure($"Parceiro já está {statusText}.");
        }

        // Buscar usuário atual e validar permissões
        var permissionValidationResult = await ValidateUserPermissionsAsync(currentUserId, existingPartner.VetorId, cancellationToken);
        if (!permissionValidationResult.IsValid)
        {
            return ActivateDeactivatePartnerResult.Failure(permissionValidationResult.ErrorMessage);
        }

        var currentUser = permissionValidationResult.User!;

        // Buscar vetor para obter informações
        var vetor = await _vetorRepository.GetByIdAsync(existingPartner.VetorId, cancellationToken);
        if (vetor is null)
        {
            return ActivateDeactivatePartnerResult.Failure("Vetor não encontrado.");
        }

        // Validações específicas para desativação
        if (!request.Active)
        {
            var deactivationValidation = await ValidateDeactivationAsync(partnerId, cancellationToken);
            if (!deactivationValidation.IsValid)
            {
                return ActivateDeactivatePartnerResult.Failure(deactivationValidation.ErrorMessage);
            }
        }

        // Aplicar mudança de status
        if (request.Active)
        {
            existingPartner.Activate();
        }
        else
        {
            existingPartner.Deactivate();
        }

        // Salvar no repositório
        await _partnerRepository.UpdateAsync(existingPartner, cancellationToken);

        // Buscar informações adicionais para o DTO
        var recommender = existingPartner.RecommenderId.HasValue 
            ? await _partnerRepository.GetByIdAsync(existingPartner.RecommenderId.Value, cancellationToken)
            : null;

        var recommendedPartners = await _partnerRepository.GetRecommendedByPartnerAsync(partnerId, cancellationToken);

        // Montar DTO de resposta
        var partnerDto = new PartnerDto
        {
            Id = existingPartner.Id,
            Name = existingPartner.Name,
            PhoneNumber = existingPartner.PhoneNumber,
            Email = existingPartner.Email,
            Active = existingPartner.Active,
            CreatedAt = existingPartner.CreatedAt,
            VetorId = existingPartner.VetorId,
            VetorName = vetor.Name,
            RecommenderId = existingPartner.RecommenderId,
            RecommenderName = recommender?.Name,
            RecommendedCount = recommendedPartners.Count()
        };

        var actionText = request.Active ? "ativado" : "desativado";
        var message = $"Parceiro {actionText} com sucesso.";

        return ActivateDeactivatePartnerResult.Success(partnerDto, message);
    }

    private static ValidationResult ValidateBasicInput(Guid partnerId, ActivateDeactivatePartnerRequest request)
    {
        if (partnerId == Guid.Empty)
        {
            return ValidationResult.Invalid("ID do parceiro é obrigatório.");
        }

        return ValidationResult.Valid();
    }

    private async Task<UserValidationResult> ValidateUserPermissionsAsync(Guid currentUserId, Guid vetorId, CancellationToken cancellationToken)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser is null)
        {
            return UserValidationResult.Invalid("Usuário atual não encontrado.");
        }

        if (!currentUser.Active)
        {
            return UserValidationResult.Invalid("Usuário atual está inativo.");
        }

        // Verificar se tem permissão para ativar/desativar parceiros
        if (currentUser.Permission != PermissionEnum.AdminGlobal &&
            currentUser.Permission != PermissionEnum.AdminVetor &&
            currentUser.Permission != PermissionEnum.Operador)
        {
            return UserValidationResult.Invalid("Você não tem permissão para ativar/desativar parceiros.");
        }

        // AdminGlobal pode ativar/desativar em qualquer vetor
        if (currentUser.Permission == PermissionEnum.AdminGlobal)
        {
            return UserValidationResult.Valid(currentUser);
        }

        // AdminVetor e Operador só podem ativar/desativar no próprio vetor
        var userVetores = currentUser.UserVetores?.Select(uv => uv.VetorId) ?? Enumerable.Empty<Guid>();
        if (!userVetores.Contains(vetorId))
        {
            return UserValidationResult.Invalid("Você só pode ativar/desativar parceiros do seu vetor.");
        }

        return UserValidationResult.Valid(currentUser);
    }

    private async Task<ValidationResult> ValidateDeactivationAsync(Guid partnerId, CancellationToken cancellationToken)
    {
        // Verificar se o parceiro tem recomendados ativos
        var recommendedPartners = await _partnerRepository.GetRecommendedByPartnerAsync(partnerId, cancellationToken);
        var activeRecommended = recommendedPartners.Where(p => p.Active).ToList();

        if (activeRecommended.Any())
        {
            var recommendedNames = string.Join(", ", activeRecommended.Take(3).Select(p => p.Name));
            var moreText = activeRecommended.Count > 3 ? $" e mais {activeRecommended.Count - 3}" : "";
            
            return ValidationResult.Invalid(
                $"Não é possível desativar este parceiro pois ele tem {activeRecommended.Count} parceiro(s) ativo(s) recomendado(s): {recommendedNames}{moreText}. " +
                "Desative ou mova os parceiros recomendados primeiro.");
        }

        return ValidationResult.Valid();
    }

    private sealed record ValidationResult(bool IsValid, string ErrorMessage)
    {
        public static ValidationResult Valid() => new(true, string.Empty);
        public static ValidationResult Invalid(string errorMessage) => new(false, errorMessage);
    }

    private sealed record UserValidationResult(bool IsValid, string ErrorMessage, User? User)
    {
        public static UserValidationResult Valid(User user) => new(true, string.Empty, user);
        public static UserValidationResult Invalid(string errorMessage) => new(false, errorMessage, null);
    }
}