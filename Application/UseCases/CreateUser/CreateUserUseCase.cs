using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.AuthenticateUser.DTO;
using Application.UseCases.CreateUser.DTO;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.UseCases.CreateUser;

public sealed class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IVetorRepository _vetorRepository;

    public CreateUserUseCase(
        IUserRepository userRepository,
        IVetorRepository vetorRepository)
    {
        _userRepository = userRepository;
        _vetorRepository = vetorRepository;
    }

    public async Task<CreateUserResult> CreateAsync(CreateUserRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Validações de entrada
        var validationResult = await ValidateRequestAsync(request, currentUserId, cancellationToken);
        if (!validationResult.IsValid)
        {
            return CreateUserResult.Failure(validationResult.ErrorMessage);
        }

        // Verificar se email já existe
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return CreateUserResult.Failure("Email já está em uso.");
        }

        // Criar usuário
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Name, request.Email, passwordHash, request.Permission);

        // Adicionar vetor se especificado
        if (request.VetorId.HasValue)
        {
            user.AddVetor(request.VetorId.Value);
        }

        // Salvar usuário
        await _userRepository.SaveAsync(user, cancellationToken);

        // Retornar resultado
        var userInfo = new UserInfo(
            user.Id,
            user.Name,
            user.Email,
            user.Permission,
            request.VetorId.HasValue ? new List<Guid> { request.VetorId.Value } : new List<Guid>());

        return CreateUserResult.Success(userInfo);
    }

    private async Task<ValidationResult> ValidateRequestAsync(CreateUserRequest request, Guid currentUserId, CancellationToken cancellationToken)
    {
        // Validações básicas
        if (string.IsNullOrWhiteSpace(request.Name))
            return ValidationResult.Invalid("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return ValidationResult.Invalid("Email é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Password))
            return ValidationResult.Invalid("Senha é obrigatória.");

        if (request.Password.Length < 6)
            return ValidationResult.Invalid("Senha deve ter pelo menos 6 caracteres.");

        // Obter usuário atual para verificar permissões
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser == null)
            return ValidationResult.Invalid("Usuário atual não encontrado.");

        // Validações de permissão
        if (currentUser.Permission != PermissionEnum.AdminGlobal && currentUser.Permission != PermissionEnum.AdminVetor)
            return ValidationResult.Invalid("Apenas Admin Global ou Admin de Vetor podem criar usuários.");

        // Validações específicas por tipo de usuário a ser criado
        if (request.Permission == PermissionEnum.AdminGlobal)
        {
            // Apenas Admin Global pode criar outro Admin Global
            if (currentUser.Permission != PermissionEnum.AdminGlobal)
                return ValidationResult.Invalid("Apenas Admin Global pode criar outro Admin Global.");

            // Admin Global não pode ter vetor
            if (request.VetorId.HasValue)
                return ValidationResult.Invalid("Admin Global não pode ter vetor associado.");
        }
        else
        {
            // Outros tipos de usuário devem ter vetor
            if (!request.VetorId.HasValue)
                return ValidationResult.Invalid("Usuário de vetor deve ter vetor associado.");

            // Verificar se o vetor existe
            var vetor = await _vetorRepository.GetByIdAsync(request.VetorId.Value, cancellationToken);
            if (vetor == null)
                return ValidationResult.Invalid("Vetor não encontrado.");

            // Se o usuário atual é Admin de Vetor, só pode criar usuários para seu próprio vetor
            if (currentUser.Permission == PermissionEnum.AdminVetor)
            {
                var currentUserVetorIds = currentUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId).ToList();
                if (!currentUserVetorIds.Contains(request.VetorId.Value))
                    return ValidationResult.Invalid("Admin de Vetor só pode criar usuários para seu próprio vetor.");
            }
        }

        return ValidationResult.Valid();
    }

    private record ValidationResult(bool IsValid, string ErrorMessage = "")
    {
        public static ValidationResult Valid() => new(true);
        public static ValidationResult Invalid(string message) => new(false, message);
    }
}