using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetPartnerById.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.GetPartnerById;

public class GetPartnerByIdUseCase : IGetPartnerByIdUseCase
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;

    public GetPartnerByIdUseCase(
        IPartnerRepository partnerRepository,
        IVetorRepository vetorRepository,
        IUserRepository userRepository)
    {
        _partnerRepository = partnerRepository;
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
    }

    public async Task<GetPartnerByIdResult> GetByIdAsync(Guid partnerId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar usuário atual
            var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
            if (currentUser == null || !currentUser.Active)
            {
                return GetPartnerByIdResult.Failure("Usuário atual não encontrado ou inativo.");
            }

            // Verificar permissões
            var hasPermission = currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal) ||
                               currentUser.Permission.HasFlag(PermissionEnum.AdminVetor) ||
                               currentUser.Permission.HasFlag(PermissionEnum.Operador);

            if (!hasPermission)
            {
                return GetPartnerByIdResult.Failure("Usuário não tem permissão para acessar dados de parceiros.");
            }

            // Buscar o parceiro
            var partner = await _partnerRepository.GetByIdAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return GetPartnerByIdResult.NotFound();
            }

            // Verificar se o usuário pode acessar este parceiro
            if (!currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal))
            {
                // Para AdminVetor e Operador, verificar se o parceiro pertence ao seu vetor
                var userVetores = currentUser.UserVetores;
                if (!userVetores.Any() || !userVetores.Any(uv => uv.VetorId == partner.VetorId))
                {
                    return GetPartnerByIdResult.Failure("Usuário não tem permissão para acessar este parceiro.");
                }
            }

            // Buscar informações do vetor
            var vetor = await _vetorRepository.GetByIdAsync(partner.VetorId, cancellationToken);
            if (vetor == null)
            {
                return GetPartnerByIdResult.Failure("Vetor do parceiro não encontrado.");
            }

            // Buscar informações do recomendador (se houver)
            Domain.Entities.Partner? recommender = null;
            if (partner.RecommenderId.HasValue)
            {
                recommender = await _partnerRepository.GetByIdAsync(partner.RecommenderId.Value, cancellationToken);
            }

            // Buscar parceiros recomendados por este parceiro
            var recommendedPartners = await _partnerRepository.GetRecommendedByPartnerAsync(partnerId, cancellationToken);

            // Calcular nível do parceiro
            int level = await CalculatePartnerLevelAsync(partnerId, cancellationToken);

            // Criar DTO detalhado
            var partnerDetail = PartnerDetailDto.FromEntity(partner, vetor, recommender, recommendedPartners, level);

            return GetPartnerByIdResult.Success(partnerDetail);
        }
        catch (Exception ex)
        {
            return GetPartnerByIdResult.Failure($"Erro interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Calcula o nível do parceiro baseado na cadeia de recomendação
    /// </summary>
    private async Task<int> CalculatePartnerLevelAsync(Guid partnerId, CancellationToken cancellationToken)
    {
        var partner = await _partnerRepository.GetByIdAsync(partnerId, cancellationToken);
        if (partner == null) return 0;

        // Se não tem recomendador, foi recomendado diretamente por um vetor = nível 1
        if (!partner.RecommenderId.HasValue)
        {
            return 1;
        }

        // Se tem recomendador, o nível é: nível do recomendador + 1
        var recommenderLevel = await CalculatePartnerLevelAsync(partner.RecommenderId.Value, cancellationToken);
        return recommenderLevel + 1;
    }
}