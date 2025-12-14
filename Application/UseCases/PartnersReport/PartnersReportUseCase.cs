using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.PartnersReport.DTO;
using Domain.ValueObjects;

namespace Application.UseCases.PartnersReport;

public class PartnersReportUseCase : IPartnersReportUseCase
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IVetorRepository _vetorRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;

    public PartnersReportUseCase(
        IPartnerRepository partnerRepository,
        IVetorRepository vetorRepository,
        IBusinessRepository businessRepository,
        ICommissionRepository commissionRepository)
    {
        _partnerRepository = partnerRepository;
        _vetorRepository = vetorRepository;
        _businessRepository = businessRepository;
        _commissionRepository = commissionRepository;
    }

    public async Task<PartnersReportResult> ExecuteAsync(
        PartnersReportRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implementar validação de acesso por vetor quando necessário
            
            // Se não especificou vetor, pegar o primeiro disponível (para demo)
            Guid vetorId = request.VetorId ?? Guid.Empty;
            if (vetorId == Guid.Empty)
            {
                var vetores = await _vetorRepository.GetAllAsync(cancellationToken);
                var firstVetor = vetores.FirstOrDefault();
                if (firstVetor == null)
                {
                    return PartnersReportResult.Failure("Nenhum vetor encontrado.");
                }
                vetorId = firstVetor.Id;
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
                            if (payment.Status == ComissionPayment.Pago)
                            {
                                totalReceived += payment.Value;
                            }
                            else if (payment.Status == ComissionPayment.APagar)
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
            partnersTree = ApplySorting(partnersTree, request.SortBy, request.SortDirection);

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
                RecommenderId = parentId,
                RecommenderName = recommender?.Name ?? "",
                Children = BuildPartnersTree(partners, financialData, partner.Id, level + 1)
            };

            result.Add(node);
        }

        return result;
    }

    private List<PartnerTreeNodeDto> ApplySorting(List<PartnerTreeNodeDto> tree, string sortBy, string sortDirection)
    {
        bool ascending = sortDirection.ToLower() == "asc";

        var sorted = sortBy.ToLower() switch
        {
            "level" => ascending ? tree.OrderBy(p => p.Level).ToList() : tree.OrderByDescending(p => p.Level).ToList(),
            "totalreceived" => ascending ? tree.OrderBy(p => p.TotalReceived).ToList() : tree.OrderByDescending(p => p.TotalReceived).ToList(),
            "totalpending" => ascending ? tree.OrderBy(p => p.TotalPending).ToList() : tree.OrderByDescending(p => p.TotalPending).ToList(),
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