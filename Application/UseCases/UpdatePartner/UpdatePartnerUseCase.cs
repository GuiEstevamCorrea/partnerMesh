using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.UpdatePartner.DTO;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.UseCases.UpdatePartner;

public sealed class UpdatePartnerUseCase : IUpdatePartnerUseCase
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVetorRepository _vetorRepository;

    public UpdatePartnerUseCase(
        IPartnerRepository partnerRepository,
        IUserRepository userRepository,
        IVetorRepository vetorRepository)
    {
        _partnerRepository = partnerRepository;
        _userRepository = userRepository;
        _vetorRepository = vetorRepository;
    }

    public async Task<UpdatePartnerResult> UpdateAsync(Guid partnerId, UpdatePartnerRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Validar entrada básica
        var basicValidationResult = ValidateBasicInput(partnerId, request);
        if (!basicValidationResult.IsValid)
        {
            return UpdatePartnerResult.Failure(basicValidationResult.ErrorMessage);
        }

        // Buscar o parceiro existente
        var existingPartner = await _partnerRepository.GetByIdAsync(partnerId, cancellationToken);
        if (existingPartner is null)
        {
            return UpdatePartnerResult.Failure("Parceiro não encontrado.");
        }

        // Buscar usuário atual e validar permissões
        var permissionValidationResult = await ValidateUserPermissionsAsync(currentUserId, existingPartner.VetorId, cancellationToken);
        if (!permissionValidationResult.IsValid)
        {
            return UpdatePartnerResult.Failure(permissionValidationResult.ErrorMessage);
        }

        var currentUser = permissionValidationResult.User!;

        // Buscar vetor para obter informações
        var vetor = await _vetorRepository.GetByIdAsync(existingPartner.VetorId, cancellationToken);
        if (vetor is null)
        {
            return UpdatePartnerResult.Failure("Vetor não encontrado.");
        }

        // Validar email único (excluindo o próprio parceiro)
        var emailExists = await _partnerRepository.EmailExistsAsync(request.Email, partnerId, cancellationToken);
        if (emailExists)
        {
            return UpdatePartnerResult.Failure("Já existe outro parceiro com este email.");
        }

        // Validar recomendador se especificado
        Partner? recommender = null;
        if (request.RecommenderId.HasValue)
        {
            var recommenderValidationResult = await ValidateRecommenderAsync(request.RecommenderId.Value, existingPartner.VetorId, partnerId, cancellationToken);
            if (!recommenderValidationResult.IsValid)
            {
                return UpdatePartnerResult.Failure(recommenderValidationResult.ErrorMessage);
            }
            recommender = recommenderValidationResult.Recommender!;
        }

        // Atualizar informações do parceiro
        existingPartner.UpdateInfo(
            name: request.Name.Trim(),
            phoneNumber: request.PhoneNumber.Trim(),
            email: request.Email.Trim().ToLowerInvariant()
        );

        // Atualizar recomendador se necessário
        existingPartner.UpdateRecommender(request.RecommenderId);

        // Salvar no repositório
        await _partnerRepository.UpdateAsync(existingPartner, cancellationToken);

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
            RecommenderName = recommender?.Name
        };

        return UpdatePartnerResult.Success(partnerDto);
    }

    private static ValidationResult ValidateBasicInput(Guid partnerId, UpdatePartnerRequest request)
    {
        if (partnerId == Guid.Empty)
        {
            return ValidationResult.Invalid("ID do parceiro é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ValidationResult.Invalid("Nome é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return ValidationResult.Invalid("Telefone é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return ValidationResult.Invalid("Email é obrigatório.");
        }

        if (!IsValidEmail(request.Email))
        {
            return ValidationResult.Invalid("Email deve ter um formato válido.");
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

        // Verificar se tem permissão para atualizar parceiros
        if (currentUser.Permission != PermissionEnum.AdminGlobal &&
            currentUser.Permission != PermissionEnum.AdminVetor &&
            currentUser.Permission != PermissionEnum.Operador)
        {
            return UserValidationResult.Invalid("Você não tem permissão para atualizar parceiros.");
        }

        // AdminGlobal pode atualizar em qualquer vetor
        if (currentUser.Permission == PermissionEnum.AdminGlobal)
        {
            return UserValidationResult.Valid(currentUser);
        }

        // AdminVetor e Operador só podem atualizar no próprio vetor
        var userVetores = currentUser.UserVetores?.Select(uv => uv.VetorId) ?? Enumerable.Empty<Guid>();
        if (!userVetores.Contains(vetorId))
        {
            return UserValidationResult.Invalid("Você só pode atualizar parceiros do seu vetor.");
        }

        return UserValidationResult.Valid(currentUser);
    }

    private async Task<RecommenderValidationResult> ValidateRecommenderAsync(Guid recommenderId, Guid vetorId, Guid partnerId, CancellationToken cancellationToken)
    {
        var recommender = await _partnerRepository.GetByIdAsync(recommenderId, cancellationToken);
        if (recommender is null)
        {
            return RecommenderValidationResult.Invalid("Recomendador não encontrado.");
        }

        // Verificar se pertence ao mesmo vetor
        if (recommender.VetorId != vetorId)
        {
            return RecommenderValidationResult.Invalid("Recomendador deve pertencer ao mesmo vetor.");
        }

        // Verificar se está ativo
        if (!recommender.CanRecommend())
        {
            return RecommenderValidationResult.Invalid("Recomendador deve estar ativo para poder recomendar.");
        }

        // Verificar se não está tentando se recomendar
        if (recommender.Id == partnerId)
        {
            return RecommenderValidationResult.Invalid("Um parceiro não pode ser recomendador de si mesmo.");
        }

        // Verificar se não criaria um ciclo
        var wouldCreateCycle = await _partnerRepository.WouldCreateCycleAsync(partnerId, recommenderId, cancellationToken);
        if (wouldCreateCycle)
        {
            return RecommenderValidationResult.Invalid("Esta alteração criaria um ciclo na cadeia de recomendação.");
        }

        return RecommenderValidationResult.Valid(recommender);
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

    private sealed record RecommenderValidationResult(bool IsValid, string ErrorMessage, Partner? Recommender)
    {
        public static RecommenderValidationResult Valid(Partner recommender) => new(true, string.Empty, recommender);
        public static RecommenderValidationResult Invalid(string errorMessage) => new(false, errorMessage, null);
    }
}