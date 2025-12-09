using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.CreateVetor.DTO;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.UseCases.CreateVetor;

public sealed class CreateVetorUseCase : ICreateVetorUseCase
{
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;

    public CreateVetorUseCase(
        IVetorRepository vetorRepository,
        IUserRepository userRepository)
    {
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
    }

    public async Task<CreateVetorResult> CreateAsync(CreateVetorRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Validações de entrada
        var validationResult = await ValidateRequestAsync(request, currentUserId, cancellationToken);
        if (!validationResult.IsValid)
        {
            return CreateVetorResult.Failure(validationResult.ErrorMessage);
        }

        // Verificar se nome já existe
        if (await _vetorRepository.NameExistsAsync(request.Name, cancellationToken))
        {
            return CreateVetorResult.Failure("Nome do vetor já está em uso.");
        }

        // Verificar se email já existe
        if (await _vetorRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return CreateVetorResult.Failure("Email do vetor já está em uso.");
        }

        // Criar o vetor
        var vetor = new Vetor(request.Name, request.Email);

        // Salvar o vetor
        await _vetorRepository.SaveAsync(vetor, cancellationToken);

        // Retornar resultado de sucesso
        var vetorInfo = new VetorInfo(
            vetor.Id,
            vetor.Name,
            vetor.Email,
            vetor.Active,
            vetor.CreatedAt);

        return CreateVetorResult.Success(vetorInfo);
    }

    private async Task<ValidationResult> ValidateRequestAsync(CreateVetorRequest request, Guid currentUserId, CancellationToken cancellationToken)
    {
        // Validações básicas
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return ValidationResult.Invalid("Nome do vetor é obrigatório.");
        }

        if (request.Name.Length < 2)
        {
            return ValidationResult.Invalid("Nome do vetor deve ter pelo menos 2 caracteres.");
        }

        if (request.Name.Length > 100)
        {
            return ValidationResult.Invalid("Nome do vetor deve ter no máximo 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return ValidationResult.Invalid("Email do vetor é obrigatório.");
        }

        if (!IsValidEmail(request.Email))
        {
            return ValidationResult.Invalid("Email do vetor inválido.");
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
            return ValidationResult.Invalid("Apenas administradores globais podem criar vetores.");
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