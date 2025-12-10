using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetPartnerTree.DTO;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.UseCases.GetPartnerTree;

public class GetPartnerTreeUseCase : IGetPartnerTreeUseCase
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;

    public GetPartnerTreeUseCase(
        IPartnerRepository partnerRepository,
        IVetorRepository vetorRepository,
        IUserRepository userRepository)
    {
        _partnerRepository = partnerRepository;
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
    }

    public async Task<GetPartnerTreeResult> GetTreeAsync(GetPartnerTreeRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar usuário atual
            var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
            if (currentUser == null || !currentUser.Active)
            {
                return GetPartnerTreeResult.Failure("Usuário atual não encontrado ou inativo.");
            }

            // Verificar permissões
            var hasPermission = currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal) ||
                               currentUser.Permission.HasFlag(PermissionEnum.AdminVetor) ||
                               currentUser.Permission.HasFlag(PermissionEnum.Operador);

            if (!hasPermission)
            {
                return GetPartnerTreeResult.Failure("Usuário não tem permissão para acessar dados de parceiros.");
            }

            // Determinar o vetor
            Guid targetVetorId;
            if (request.VetorId.HasValue)
            {
                targetVetorId = request.VetorId.Value;
                
                // Se não for AdminGlobal, verificar se pode acessar este vetor
                if (!currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal))
                {
                    var userVetores = currentUser.UserVetores;
                    if (!userVetores.Any(uv => uv.VetorId == targetVetorId))
                    {
                        return GetPartnerTreeResult.Failure("Usuário não tem permissão para acessar este vetor.");
                    }
                }
            }
            else
            {
                // Para AdminVetor e Operador, usar o próprio vetor
                if (currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal))
                {
                    return GetPartnerTreeResult.Failure("Admin Global deve especificar um vetor.");
                }
                
                var userVetores = currentUser.UserVetores;
                if (!userVetores.Any())
                {
                    return GetPartnerTreeResult.Failure("Usuário não está associado a nenhum vetor.");
                }
                
                targetVetorId = userVetores.First().VetorId;
            }

            // Buscar o vetor
            var vetor = await _vetorRepository.GetByIdAsync(targetVetorId, cancellationToken);
            if (vetor == null)
            {
                return GetPartnerTreeResult.Failure("Vetor não encontrado.");
            }

            // Buscar todos os parceiros do vetor
            var allPartners = await _partnerRepository.GetByVetorIdAsync(targetVetorId, cancellationToken);
            var partnersList = allPartners.ToList();

            // Filtrar por ativo se solicitado
            if (request.OnlyActive)
            {
                partnersList = partnersList.Where(p => p.Active).ToList();
            }

            // Se especificou um parceiro raiz, filtrar a árvore
            if (request.RootPartnerId.HasValue)
            {
                var rootPartner = partnersList.FirstOrDefault(p => p.Id == request.RootPartnerId.Value);
                if (rootPartner == null)
                {
                    return GetPartnerTreeResult.Failure("Parceiro raiz não encontrado.");
                }

                // Construir árvore a partir do parceiro raiz
                var subTree = await BuildSubTree(rootPartner, partnersList, 0, request.MaxDepth, cancellationToken);
                var tree = new PartnerTreeDto
                {
                    Vetor = VetorInfo.FromEntity(vetor),
                    RootPartners = new[] { subTree },
                    OrphanPartners = Enumerable.Empty<PartnerTreeNodeDto>()
                };

                var stats = CalculateTreeStatistics(new[] { subTree }, 0);
                return GetPartnerTreeResult.Success(tree, stats);
            }
            else
            {
                // Construir árvore completa do vetor
                var tree = await BuildCompleteTree(vetor, partnersList, request.MaxDepth, cancellationToken);
                var stats = CalculateTreeStatistics(tree.RootPartners.Concat(tree.OrphanPartners), tree.OrphanPartners.Count());
                return GetPartnerTreeResult.Success(tree, stats);
            }
        }
        catch (Exception ex)
        {
            return GetPartnerTreeResult.Failure($"Erro interno: {ex.Message}");
        }
    }

    private async Task<PartnerTreeDto> BuildCompleteTree(
        Vetor vetor, 
        List<Partner> allPartners, 
        int? maxDepth, 
        CancellationToken cancellationToken)
    {
        // Separar parceiros raiz (recomendados pelo vetor) e órfãos
        var rootPartners = allPartners.Where(p => !p.RecommenderId.HasValue || p.RecommenderId.Value == Guid.Empty).ToList();
        var partnersWithRecommender = allPartners.Where(p => p.RecommenderId.HasValue && p.RecommenderId.Value != Guid.Empty).ToList();
        
        // Identificar órfãos (têm recomendador que não existe na lista)
        var validRecommenderIds = allPartners.Select(p => p.Id).ToHashSet();
        validRecommenderIds.Add(vetor.Id); // Vetor também é recomendador válido
        
        var orphans = partnersWithRecommender.Where(p => !validRecommenderIds.Contains(p.RecommenderId!.Value)).ToList();
        var validPartners = partnersWithRecommender.Where(p => validRecommenderIds.Contains(p.RecommenderId!.Value)).ToList();

        // Construir árvore dos parceiros raiz
        var rootNodes = new List<PartnerTreeNodeDto>();
        foreach (var rootPartner in rootPartners)
        {
            var node = await BuildSubTree(rootPartner, allPartners, 0, maxDepth, cancellationToken);
            rootNodes.Add(node);
        }

        // Construir nós órfãos
        var orphanNodes = new List<PartnerTreeNodeDto>();
        foreach (var orphan in orphans)
        {
            var node = await BuildSubTree(orphan, allPartners, -1, maxDepth, cancellationToken); // -1 indica órfão
            orphanNodes.Add(node);
        }

        return new PartnerTreeDto
        {
            Vetor = VetorInfo.FromEntity(vetor),
            RootPartners = rootNodes,
            OrphanPartners = orphanNodes
        };
    }

    private async Task<PartnerTreeNodeDto> BuildSubTree(
        Partner partner, 
        List<Partner> allPartners, 
        int level, 
        int? maxDepth, 
        CancellationToken cancellationToken)
    {
        var children = new List<PartnerTreeNodeDto>();

        // Se não atingiu a profundidade máxima, buscar filhos
        if (!maxDepth.HasValue || level < maxDepth.Value)
        {
            var childPartners = allPartners.Where(p => p.RecommenderId == partner.Id).ToList();
            
            foreach (var child in childPartners)
            {
                var childNode = await BuildSubTree(child, allPartners, level + 1, maxDepth, cancellationToken);
                children.Add(childNode);
            }
        }

        // Buscar nome do recomendador
        string? recommenderName = null;
        if (partner.RecommenderId.HasValue && partner.RecommenderId.Value != Guid.Empty)
        {
            var recommender = allPartners.FirstOrDefault(p => p.Id == partner.RecommenderId.Value);
            recommenderName = recommender?.Name;
            
            // Se não encontrou nas partners, pode ser o vetor
            if (recommenderName == null)
            {
                var vetor = await _vetorRepository.GetByIdAsync(partner.RecommenderId.Value, cancellationToken);
                recommenderName = vetor?.Name;
            }
        }

        return PartnerTreeNodeDto.FromEntity(partner, level, recommenderName, children);
    }

    private TreeStatistics CalculateTreeStatistics(IEnumerable<PartnerTreeNodeDto> allNodes, int orphanCount)
    {
        var nodesList = FlattenTree(allNodes).ToList();
        
        var partnersByLevel = nodesList
            .Where(n => n.Level >= 0) // Excluir órfãos (-1)
            .GroupBy(n => n.Level)
            .ToDictionary(g => g.Key, g => g.Count());

        return new TreeStatistics
        {
            TotalPartners = nodesList.Count,
            ActivePartners = nodesList.Count(n => n.IsActive),
            InactivePartners = nodesList.Count(n => !n.IsActive),
            PartnersByLevel = partnersByLevel,
            OrphanPartners = orphanCount,
            MaxDepth = nodesList.Where(n => n.Level >= 0).DefaultIfEmpty().Max(n => n?.Level ?? 0)
        };
    }

    private IEnumerable<PartnerTreeNodeDto> FlattenTree(IEnumerable<PartnerTreeNodeDto> nodes)
    {
        foreach (var node in nodes)
        {
            yield return node;
            
            foreach (var child in FlattenTree(node.Children))
            {
                yield return child;
            }
        }
    }
}