using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetVetorById.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.GetVetorById;

public sealed class GetVetorByIdUseCase : IGetVetorByIdUseCase
{
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;

    public GetVetorByIdUseCase(
        IVetorRepository vetorRepository,
        IUserRepository userRepository)
    {
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
    }

    public async Task<GetVetorByIdResult> GetByIdAsync(GetVetorByIdRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Validar entrada
        if (request.VetorId == Guid.Empty)
        {
            return GetVetorByIdResult.Failure("ID do vetor é obrigatório.");
        }

        // Buscar usuário atual para verificar permissões
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser is null)
        {
            return GetVetorByIdResult.Failure("Usuário atual não encontrado.");
        }

        // Verificar se o usuário atual está ativo
        if (!currentUser.Active)
        {
            return GetVetorByIdResult.Failure("Usuário atual está inativo.");
        }

        // Verificar permissões básicas
        if (currentUser.Permission == PermissionEnum.Operador)
        {
            return GetVetorByIdResult.Failure("Operadores não têm permissão para visualizar detalhes de vetores.");
        }

        // Buscar o vetor
        var vetor = await _vetorRepository.GetByIdAsync(request.VetorId, cancellationToken);
        if (vetor is null)
        {
            return GetVetorByIdResult.Failure("Vetor não encontrado.");
        }

        // Verificar permissões de acesso ao vetor específico
        var hasAccess = VerifyVetorAccess(currentUser, vetor.Id);
        if (!hasAccess)
        {
            return GetVetorByIdResult.Failure("Você não tem permissão para acessar este vetor.");
        }

        // Calcular estatísticas do vetor
        var statistics = await CalculateVetorStatistics(vetor.Id, cancellationToken);

        // Montar DTO de resposta
        var vetorDetailDto = new VetorDetailDto
        {
            Id = vetor.Id,
            Name = vetor.Name,
            Email = vetor.Email,
            Active = vetor.Active,
            CreatedAt = vetor.CreatedAt,
            Statistics = statistics
        };

        return GetVetorByIdResult.Success(vetorDetailDto);
    }

    private bool VerifyVetorAccess(Domain.Entities.User currentUser, Guid vetorId)
    {
        // AdminGlobal pode acessar qualquer vetor
        if (currentUser.Permission == PermissionEnum.AdminGlobal)
        {
            return true;
        }

        // AdminVetor só pode acessar vetores onde é administrador
        if (currentUser.Permission == PermissionEnum.AdminVetor)
        {
            var userVetores = currentUser.UserVetores?.Select(uv => uv.VetorId) ?? Enumerable.Empty<Guid>();
            return userVetores.Contains(vetorId);
        }

        return false;
    }

    private async Task<VetorStatisticsDto> CalculateVetorStatistics(Guid vetorId, CancellationToken cancellationToken)
    {
        // Por enquanto, retornamos estatísticas básicas
        // No futuro, quando tivermos mais entidades (Partner, Business, etc.), calcularemos valores reais
        
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var vetorUsers = users.Where(u => u.UserVetores?.Any(uv => uv.VetorId == vetorId) == true).ToList();

        return new VetorStatisticsDto
        {
            TotalUsers = vetorUsers.Count,
            TotalPartners = 0, // TODO: Implementar quando tivermos Partner repository
            ActivePartners = 0,
            InactivePartners = 0,
            TotalBusinesses = 0, // TODO: Implementar quando tivermos Business repository
            TotalBusinessValue = 0m,
            TotalCommissions = 0m,
            PaidCommissions = 0m,
            PendingCommissions = 0m
        };
    }
}