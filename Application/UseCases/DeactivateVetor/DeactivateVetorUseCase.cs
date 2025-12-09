using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.DeactivateVetor.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.DeactivateVetor;

public sealed class DeactivateVetorUseCase : IDeactivateVetorUseCase
{
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;

    public DeactivateVetorUseCase(
        IVetorRepository vetorRepository,
        IUserRepository userRepository)
    {
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
    }

    public async Task<DeactivateVetorResult> DeactivateAsync(Guid vetorId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Validações de entrada e permissões
        var validationResult = await ValidateRequestAsync(vetorId, currentUserId, cancellationToken);
        if (!validationResult.IsValid)
        {
            return DeactivateVetorResult.Failure(validationResult.ErrorMessage);
        }

        var vetor = validationResult.Vetor!;

        // Verificar se o vetor já está inativo
        if (!vetor.Active)
        {
            return DeactivateVetorResult.Failure("Vetor já está inativo.");
        }

        // Validação crítica específica do UC-22: verificar se existem administradores ativos
        var adminValidation = await ValidateActiveAdministrators(vetor, cancellationToken);
        if (!adminValidation.IsValid)
        {
            return DeactivateVetorResult.Failure(adminValidation.ErrorMessage);
        }

        // Inativar o vetor
        vetor.Deactivate();

        // Salvar alterações
        await _vetorRepository.SaveAsync(vetor, cancellationToken);

        // Retornar resultado de sucesso
        var vetorInfo = new VetorInfo(
            vetor.Id,
            vetor.Name,
            vetor.Email,
            vetor.Active,
            vetor.CreatedAt);

        return DeactivateVetorResult.Success(vetorInfo);
    }

    private async Task<ValidationResult> ValidateRequestAsync(Guid vetorId, Guid currentUserId, CancellationToken cancellationToken)
    {
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
            return ValidationResult.Invalid("Apenas administradores globais podem inativar vetores.");
        }

        // Buscar vetor a ser inativado
        var vetor = await _vetorRepository.GetByIdAsync(vetorId, cancellationToken);
        if (vetor == null)
        {
            return ValidationResult.Invalid("Vetor não encontrado.");
        }

        return ValidationResult.Valid(vetor);
    }

    private async Task<ValidationResult> ValidateActiveAdministrators(Domain.Entities.Vetor vetor, CancellationToken cancellationToken)
    {
        // Buscar todos os usuários
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        
        // Verificar se há administradores ativos associados ao vetor
        var activeAdminsInVetor = allUsers
            .Where(u => u.Active && 
                       (u.Permission == PermissionEnum.AdminVetor || u.Permission == PermissionEnum.Operador) &&
                       u.UserVetores.Any(uv => uv.Active && uv.VetorId == vetor.Id))
            .ToList();

        if (activeAdminsInVetor.Count > 0)
        {
            var adminNames = string.Join(", ", activeAdminsInVetor.Select(u => u.Name));
            return ValidationResult.Invalid($"Não é possível inativar este vetor pois existem {activeAdminsInVetor.Count} usuário(s) ativo(s) associado(s): {adminNames}. Desative ou mova os usuários primeiro.");
        }

        // Verificar se há parceiros associados (regra adicional de proteção)
        if (vetor.Partners.Any())
        {
            var activeParceiros = vetor.Partners.Count(p => p.Active);
            if (activeParceiros > 0)
            {
                return ValidationResult.Invalid($"Não é possível inativar este vetor pois existem {activeParceiros} parceiro(s) ativo(s) associado(s). Desative ou mova os parceiros primeiro.");
            }
        }

        return ValidationResult.Valid();
    }

    private sealed record ValidationResult(bool IsValid, string ErrorMessage = "", Domain.Entities.Vetor? Vetor = null)
    {
        public static ValidationResult Valid(Domain.Entities.Vetor? vetor = null) => new(true, "", vetor);
        public static ValidationResult Invalid(string message) => new(false, message);
    }
}