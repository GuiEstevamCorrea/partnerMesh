using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.CreatePartner.DTO;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.UseCases.CreatePartner;

public sealed class CreatePartnerUseCase : ICreatePartnerUseCase
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVetorRepository _vetorRepository;

    public CreatePartnerUseCase(
        IPartnerRepository partnerRepository,
        IUserRepository userRepository,
        IVetorRepository vetorRepository)
    {
        _partnerRepository = partnerRepository;
        _userRepository = userRepository;
        _vetorRepository = vetorRepository;
    }

    public async Task<CreatePartnerResult> CreateAsync(CreatePartnerRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Validar entrada básica
        var basicValidationResult = ValidateBasicInput(request);
        if (!basicValidationResult.IsValid)
        {
            return CreatePartnerResult.Failure(basicValidationResult.ErrorMessage);
        }

        // Buscar usuário atual e validar permissões
        var permissionValidationResult = await ValidateUserPermissionsAsync(currentUserId, request.VetorId, cancellationToken);
        if (!permissionValidationResult.IsValid)
        {
            return CreatePartnerResult.Failure(permissionValidationResult.ErrorMessage);
        }

        var currentUser = permissionValidationResult.User!;

        // Validar se o vetor existe e está ativo
        var vetor = await _vetorRepository.GetByIdAsync(request.VetorId, cancellationToken);
        if (vetor is null)
        {
            return CreatePartnerResult.Failure("Vetor não encontrado.");
        }

        if (!vetor.Active)
        {
            return CreatePartnerResult.Failure("Vetor está inativo.");
        }

        // Validar email único
        var emailExists = await _partnerRepository.EmailExistsAsync(request.Email, cancellationToken);
        if (emailExists)
        {
            return CreatePartnerResult.Failure("Já existe um parceiro com este email.");
        }

        // Validar recomendador se especificado
        Partner? recommender = null;
        if (request.RecommenderId.HasValue)
        {
            var recommenderValidationResult = await ValidateRecommenderAsync(request.RecommenderId.Value, request.VetorId, cancellationToken);
            if (!recommenderValidationResult.IsValid)
            {
                return CreatePartnerResult.Failure(recommenderValidationResult.ErrorMessage);
            }
            recommender = recommenderValidationResult.Recommender!;
        }

        // Criar novo parceiro
        var partner = new Partner(
            name: request.Name.Trim(),
            phoneNumber: request.PhoneNumber.Trim(),
            email: request.Email.Trim().ToLowerInvariant(),
            vetorId: request.VetorId,
            recommenderId: request.RecommenderId
        );

        // Salvar no repositório
        await _partnerRepository.AddAsync(partner, cancellationToken);

        // Montar DTO de resposta
        var partnerDto = new PartnerDto
        {
            Id = partner.Id,
            Name = partner.Name,
            PhoneNumber = partner.PhoneNumber,
            Email = partner.Email,
            Active = partner.Active,
            CreatedAt = partner.CreatedAt,
            VetorId = partner.VetorId,
            VetorName = vetor.Name,
            RecommenderId = partner.RecommenderId,
            RecommenderName = recommender?.Name
        };

        return CreatePartnerResult.Success(partnerDto);
    }

    private static ValidationResult ValidateBasicInput(CreatePartnerRequest request)
    {
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

        if (request.VetorId == Guid.Empty)
        {
            return ValidationResult.Invalid("Vetor é obrigatório.");
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

        // Verificar se tem permissão para criar parceiros
        if (currentUser.Permission != PermissionEnum.AdminGlobal &&
            currentUser.Permission != PermissionEnum.AdminVetor &&
            currentUser.Permission != PermissionEnum.Operador)
        {
            return UserValidationResult.Invalid("Você não tem permissão para criar parceiros.");
        }

        // AdminGlobal pode criar em qualquer vetor
        if (currentUser.Permission == PermissionEnum.AdminGlobal)
        {
            return UserValidationResult.Valid(currentUser);
        }

        // AdminVetor e Operador só podem criar no próprio vetor
        var userVetores = currentUser.UserVetores?.Select(uv => uv.VetorId) ?? Enumerable.Empty<Guid>();
        if (!userVetores.Contains(vetorId))
        {
            return UserValidationResult.Invalid("Você só pode criar parceiros no seu vetor.");
        }

        return UserValidationResult.Valid(currentUser);
    }

    private async Task<RecommenderValidationResult> ValidateRecommenderAsync(Guid recommenderId, Guid vetorId, CancellationToken cancellationToken)
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