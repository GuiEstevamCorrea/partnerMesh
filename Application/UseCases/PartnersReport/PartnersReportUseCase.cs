using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.PartnersReport.DTO;
using Domain.ValueObjects;
using Domain.ValueTypes;
using Domain.Extensions;

namespace Application.UseCases.PartnersReport;

public class PartnersReportUseCase : IPartnersReportUseCase
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IVetorRepository _vetorRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUserRepository _userRepository;

    public PartnersReportUseCase(
        IPartnerRepository partnerRepository,
        IVetorRepository vetorRepository,
        IBusinessRepository businessRepository,
        ICommissionRepository commissionRepository,
        IUserRepository userRepository)
    {
        _partnerRepository = partnerRepository;
        _vetorRepository = vetorRepository;
        _businessRepository = businessRepository;
        _commissionRepository = commissionRepository;
        _userRepository = userRepository;
    }

    public async Task<PartnersReportResult> ExecuteAsync(
        PartnersReportRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Buscar o usuário para validação de acesso
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null || !user.Active)
            {
                return PartnersReportResult.Failure("Usuário não encontrado ou inativo.");
            }

            // Validar acesso ao vetor
            Guid vetorId = request.VetorId ?? Guid.Empty;
            if (vetorId == Guid.Empty)
            {
                // Se não especificou vetor, pegar o primeiro disponível
                var vetores = await _vetorRepository.GetAllAsync(cancellationToken);
                var firstVetor = vetores.FirstOrDefault();
                if (firstVetor == null)
                {
                    return PartnersReportResult.Failure("Nenhum vetor encontrado.");
                }
                vetorId = firstVetor.Id;
            }

            // Verificar se o usuário tem acesso ao vetor solicitado
            // AdminGlobal tem acesso a todos os vetores
            var hasGlobalAccess = user.Permission.HasFlag(Domain.ValueTypes.PermissionEnum.AdminGlobal);
            if (!hasGlobalAccess)
            {
                // Para outros usuários, verificar se estão vinculados ao vetor
                var hasVetorAccess = user.UserVetores.Any(uv => uv.VetorId == vetorId && uv.Active);
                if (!hasVetorAccess)
                {
                    return PartnersReportResult.Failure("Usuário não tem acesso ao vetor solicitado.");
                }
            }

            // Buscar dados do vetor
            var vetor = await _vetorRepository.GetByIdAsync(vetorId, cancellationToken);
            if (vetor == null)
            {
                return PartnersReportResult.Failure("Vetor não encontrado.");
            }

            // Buscar todos os parceiros do vetor
            var allPartners = await _partnerRepository.GetByVetorIdAsync(vetorId, cancellationToken);
            var partnersList = allPartners.ToList();

            if (!partnersList.Any())
            {
                // Retornar relatório vazio
                var emptyReport = new PartnersReportData
                {
                    VetorId = vetorId,
                    VetorName = vetor.Name,
                    PartnersTree = Enumerable.Empty<PartnerTreeNodeDto>(),
                    LevelsSummary = Enumerable.Empty<LevelSummaryDto>(),
                    Totals = new ReportTotalsDto()
                };
                return PartnersReportResult.Success(emptyReport);
            }

            // Buscar dados financeiros para cada parceiro
            var partnerFinancialData = new Dictionary<Guid, (decimal received, decimal pending, int businessCount)>();
            
            foreach (var partner in partnersList)
            {
                var businesses = await _businessRepository.GetByPartnerIdAsync(partner.Id, cancellationToken);
                var businessList = businesses.ToList();
                
                decimal totalReceived = 0;
                decimal totalPending = 0;
                
                foreach (var business in businessList)
                {
                    var commission = await _commissionRepository.GetByBusinessIdAsync(business.Id, cancellationToken);
                    if (commission != null)
                    {
                        foreach (var payment in commission.Pagamentos.Where(p => p.PartnerId == partner.Id))
                        {
                            if (payment.Status == Domain.ValueTypes.PaymentStatus.Pago)
                            {
                                totalReceived += payment.Value;
                            }
                            else if (payment.Status == Domain.ValueTypes.PaymentStatus.APagar)
                            {
                                totalPending += payment.Value;
                            }
                        }
                    }
                }
                
                partnerFinancialData[partner.Id] = (totalReceived, totalPending, businessList.Count);
            }

            // Aplicar filtros
            var filteredPartners = partnersList.AsQueryable();
            if (request.ActiveOnly.HasValue)
            {
                filteredPartners = filteredPartners.Where(p => p.Active == request.ActiveOnly.Value);
            }

            // Construir árvore hierárquica
            var partnersTree = BuildPartnersTree(filteredPartners.ToList(), partnerFinancialData, null, 0);

            // Aplicar ordenação
            // Converter strings de ordenação para enums
            var sortField = PartnerReportSortField.Name;
            var sortDirection = SortDirection.Ascending;
            
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                PartnerReportSortFieldExtensions.TryParse(request.SortBy, out sortField);
            }
            
            if (!string.IsNullOrWhiteSpace(request.SortDirection))
            {
                SortDirectionExtensions.TryParse(request.SortDirection, out sortDirection);
            }
            
            partnersTree = ApplySorting(partnersTree, sortField, sortDirection);

            // Calcular resumo por níveis
            var levelsSummary = CalculateLevelsSummary(partnersList, partnerFinancialData);

            // Calcular totais gerais
            var totals = CalculateTotals(partnersList, partnerFinancialData);

            var reportData = new PartnersReportData
            {
                VetorId = vetorId,
                VetorName = vetor.Name,
                PartnersTree = partnersTree,
                LevelsSummary = levelsSummary.OrderBy(l => l.Level),
                Totals = totals
            };

            return PartnersReportResult.Success(reportData);
        }
        catch (Exception ex)
        {
            return PartnersReportResult.Failure($"Erro interno: {ex.Message}");
        }
    }

    private List<PartnerTreeNodeDto> BuildPartnersTree(
        List<Domain.Entities.Partner> partners,
        Dictionary<Guid, (decimal received, decimal pending, int businessCount)> financialData,
        Guid? parentId,
        int level)
    {
        var children = partners.Where(p => p.RecommenderId == parentId).ToList();
        var result = new List<PartnerTreeNodeDto>();

        foreach (var partner in children)
        {
            var financial = financialData.GetValueOrDefault(partner.Id, (0, 0, 0));
            var recommender = parentId.HasValue ? partners.FirstOrDefault(p => p.Id == parentId.Value) : null;

            var node = new PartnerTreeNodeDto
            {
                Id = partner.Id,
                Name = partner.Name,
                Email = partner.Email,
                PhoneNumber = partner.PhoneNumber,
                IsActive = partner.Active,
                CreatedAt = partner.CreatedAt,
                Level = level,
                TotalReceived = financial.received,
                TotalPending = financial.pending,
                BusinessCount = financial.businessCount,
                Children = new List<PartnerTreeNodeDto>()
            };

            result.Add(node);
        }

        return result;
    }

    private List<PartnerTreeNodeDto> ApplySorting(List<PartnerTreeNodeDto> tree, PartnerReportSortField sortBy, SortDirection sortDirection)
    {
        bool ascending = sortDirection == SortDirection.Ascending;

        var sorted = sortBy switch
        {
            PartnerReportSortField.Level => ascending ? tree.OrderBy(p => p.Level).ToList() : tree.OrderByDescending(p => p.Level).ToList(),
            PartnerReportSortField.TotalReceived => ascending ? tree.OrderBy(p => p.TotalReceived).ToList() : tree.OrderByDescending(p => p.TotalReceived).ToList(),
            PartnerReportSortField.TotalPending => ascending ? tree.OrderBy(p => p.TotalPending).ToList() : tree.OrderByDescending(p => p.TotalPending).ToList(),
            _ => ascending ? tree.OrderBy(p => p.Name).ToList() : tree.OrderByDescending(p => p.Name).ToList()
        };

        // Aplicar ordenação recursivamente nos filhos
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].Children.Any())
            {
                var sortedChildren = ApplySorting(sorted[i].Children.ToList(), sortBy, sortDirection);
                sorted[i] = sorted[i] with { Children = sortedChildren };
            }
        }

        return sorted;
    }

    private List<LevelSummaryDto> CalculateLevelsSummary(
        List<Domain.Entities.Partner> partners,
        Dictionary<Guid, (decimal received, decimal pending, int businessCount)> financialData)
    {
        var summary = new Dictionary<int, LevelSummaryDto>();
        
        // Calcular níveis baseado na árvore
        var levelMap = CalculatePartnerLevels(partners);
        
        foreach (var partner in partners)
        {
            var level = levelMap.GetValueOrDefault(partner.Id, 0);
            var financial = financialData.GetValueOrDefault(partner.Id, (0, 0, 0));
            
            if (!summary.ContainsKey(level))
            {
                summary[level] = new LevelSummaryDto
                {
                    Level = level,
                    ActivePartnersCount = 0,
                    InactivePartnersCount = 0,
                    TotalReceived = 0,
                    TotalPending = 0,
                    TotalBusinessCount = 0
                };
            }
            
            var current = summary[level];
            summary[level] = current with
            {
                ActivePartnersCount = current.ActivePartnersCount + (partner.Active ? 1 : 0),
                InactivePartnersCount = current.InactivePartnersCount + (partner.Active ? 0 : 1),
                TotalReceived = current.TotalReceived + financial.received,
                TotalPending = current.TotalPending + financial.pending,
                TotalBusinessCount = current.TotalBusinessCount + financial.businessCount
            };
        }
        
        return summary.Values.ToList();
    }

    private Dictionary<Guid, int> CalculatePartnerLevels(List<Domain.Entities.Partner> partners)
    {
        var levels = new Dictionary<Guid, int>();
        
        // Primeira passagem: encontrar parceiros raiz (sem recomendador)
        var rootPartners = partners.Where(p => !p.RecommenderId.HasValue).ToList();
        foreach (var root in rootPartners)
        {
            CalculateLevelRecursive(root.Id, 0, partners, levels);
        }
        
        return levels;
    }

    private void CalculateLevelRecursive(Guid partnerId, int level, List<Domain.Entities.Partner> allPartners, Dictionary<Guid, int> levels)
    {
        levels[partnerId] = level;
        
        var children = allPartners.Where(p => p.RecommenderId == partnerId).ToList();
        foreach (var child in children)
        {
            CalculateLevelRecursive(child.Id, level + 1, allPartners, levels);
        }
    }

    private ReportTotalsDto CalculateTotals(
        List<Domain.Entities.Partner> partners,
        Dictionary<Guid, (decimal received, decimal pending, int businessCount)> financialData)
    {
        var totalReceived = financialData.Values.Sum(f => f.received);
        var totalPending = financialData.Values.Sum(f => f.pending);
        var totalBusinessCount = financialData.Values.Sum(f => f.businessCount);
        var activeCount = partners.Count(p => p.Active);
        var inactiveCount = partners.Count(p => !p.Active);
        
        // Calcular profundidade máxima
        var levelMap = CalculatePartnerLevels(partners);
        var maxDepth = levelMap.Values.DefaultIfEmpty(0).Max();
        
        return new ReportTotalsDto
        {
            TotalActivePartners = activeCount,
            TotalInactivePartners = inactiveCount,
            TotalReceived = totalReceived,
            TotalPending = totalPending,
            TotalBusinessCount = totalBusinessCount,
            MaxDepth = maxDepth + 1 // +1 porque os níveis começam em 0
        };
    }
}