using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetVetorById.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.GetVetorById;

public sealed class GetVetorByIdUseCase : IGetVetorByIdUseCase
{
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessRepository _businessRepository;

    public GetVetorByIdUseCase(
        IVetorRepository vetorRepository,
        IUserRepository userRepository,
        IPartnerRepository partnerRepository,
        IBusinessRepository businessRepository)
    {
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
        _partnerRepository = partnerRepository;
        _businessRepository = businessRepository;
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
        // Buscar usuários do vetor
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var vetorUsers = users.Where(u => u.UserVetores?.Any(uv => uv.VetorId == vetorId && uv.Active) == true).ToList();

        // Buscar parceiros do vetor
        var allPartners = await _partnerRepository.GetByVetorIdAsync(vetorId, cancellationToken);
        var partnersList = allPartners.ToList();
        var activePartners = partnersList.Count(p => p.Active);
        var inactivePartners = partnersList.Count(p => !p.Active);

        // Buscar negócios do vetor através dos parceiros
        var allBusinesses = await _businessRepository.GetByVetorIdAsync(vetorId, cancellationToken);
        var businessesList = allBusinesses.ToList();
        var totalBusinessValue = businessesList.Where(b => b.Status != Domain.ValueTypes.BusinessStatus.Cancelado).Sum(b => b.Value);

        // Calcular comissões (10% do valor dos negócios ativos)
        var totalCommissions = totalBusinessValue * 0.10m;

        return new VetorStatisticsDto
        {
            TotalUsers = vetorUsers.Count,
            TotalPartners = partnersList.Count,
            ActivePartners = activePartners,
            InactivePartners = inactivePartners,
            TotalBusinesses = businessesList.Count,
            TotalBusinessValue = totalBusinessValue,
            TotalCommissions = totalCommissions,
            PaidCommissions = 0m, // Requer integração com CommissionRepository
            PendingCommissions = totalCommissions // Por enquanto, assume tudo pendente
        };
    }
}