using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.FinancialReport.DTO;
using Domain.ValueObjects;

namespace Application.UseCases.FinancialReport;

public class FinancialReportUseCase : IFinancialReportUseCase
{
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;

    public FinancialReportUseCase(
        IVetorRepository vetorRepository,
        IUserRepository userRepository,
        ICommissionRepository commissionRepository,
        IPartnerRepository partnerRepository,
        IBusinessRepository businessRepository,
        IBusinessTypeRepository businessTypeRepository)
    {
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
        _commissionRepository = commissionRepository;
        _partnerRepository = partnerRepository;
        _businessRepository = businessRepository;
        _businessTypeRepository = businessTypeRepository;
    }

    public async Task<FinancialReportResult> ExecuteAsync(
        FinancialReportRequest request, 
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Validar usuário atual
            var currentUser = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (currentUser == null)
            {
                return FinancialReportResult.Failure("Usuário não encontrado.");
            }

            // 2. Determinar vetor
            Guid vetorId;
            if (request.VetorId.HasValue)
            {
                // Verificar se o usuário tem acesso a este vetor
                if (!HasAccessToVetor(currentUser, request.VetorId.Value))
                {
                    return FinancialReportResult.Failure("Acesso negado ao vetor solicitado.");
                }
                vetorId = request.VetorId.Value;
            }
            else
            {
                // Usar primeiro vetor do usuário atual (se for admin/operador de vetor)
                var userVetor = currentUser.UserVetores.FirstOrDefault(uv => uv.Active);
                if (userVetor == null)
                {
                    return FinancialReportResult.Failure("Usuário não está associado a nenhum vetor e nenhum vetor foi especificado.");
                }
                vetorId = userVetor.VetorId;
            }

            // 3. Validar vetor
            var vetor = await _vetorRepository.GetByIdAsync(vetorId, cancellationToken);
            if (vetor == null)
            {
                return FinancialReportResult.Failure("Vetor não encontrado.");
            }

            // 4. Obter todas as comissões do vetor
            var allCommissions = await _commissionRepository.GetAllAsync(cancellationToken);
            var vetorCommissions = allCommissions.Where(c => c.Bussiness?.Partner?.VetorId == vetorId).ToList();

            // 5. Aplicar filtros
            var filteredPayments = ApplyFilters(vetorCommissions, request);

            // 6. Obter dados relacionados
            var partners = await _partnerRepository.GetAllAsync(cancellationToken);
            var businesses = await _businessRepository.GetAllAsync(cancellationToken);
            var businessTypes = await _businessTypeRepository.GetAllAsync(cancellationToken);

            var vetorPartners = partners.Where(p => p.VetorId == vetorId).ToList();
            var vetorBusinesses = businesses.Where(b => b.Partner?.VetorId == vetorId).ToList();

            // 7. Construir dados do relatório
            var reportData = await BuildReportData(
                vetorId, 
                vetor.Name, 
                request, 
                filteredPayments, 
                vetorPartners, 
                vetorBusinesses, 
                businessTypes.ToList());

            return FinancialReportResult.Success(reportData);
        }
        catch (Exception ex)
        {
            return FinancialReportResult.Failure($"Erro interno: {ex.Message}");
        }
    }

    private static bool HasAccessToVetor(Domain.Entities.User user, Guid vetorId)
    {
        // Admin global pode acessar qualquer vetor
        if (user.Permission == Domain.ValueTypes.PermissionEnum.AdminGlobal)
            return true;

        // Usuários de vetor só podem acessar seus próprios vetores
        return user.UserVetores.Any(uv => uv.VetorId == vetorId && uv.Active);
    }

    private static List<Domain.ValueObjects.ComissionPayment> ApplyFilters(
        List<Domain.Entities.Comission> commissions, 
        FinancialReportRequest request)
    {
        // Extrair todos os ComissionPayments
        var allPayments = commissions.SelectMany(c => c.Pagamentos).ToList();
        var filtered = allPayments.AsQueryable();

        // Filtro por período (usando data da comissão)
        if (request.StartDate.HasValue)
        {
            var commissionIds = commissions
                .Where(c => c.CreatedAt >= request.StartDate.Value)
                .Select(c => c.Id)
                .ToList();
            filtered = filtered.Where(p => commissionIds.Contains(p.ComissionId));
        }

        if (request.EndDate.HasValue)
        {
            var commissionIds = commissions
                .Where(c => c.CreatedAt <= request.EndDate.Value)
                .Select(c => c.Id)
                .ToList();
            filtered = filtered.Where(p => commissionIds.Contains(p.ComissionId));
        }

        // Filtro por status de pagamento
        if (request.IsPaid.HasValue)
        {
            var targetStatus = request.IsPaid.Value ? Domain.ValueObjects.ComissionPayment.Pago : Domain.ValueObjects.ComissionPayment.APagar;
            filtered = filtered.Where(p => p.Status == targetStatus);
        }

        // Filtro por parceiro específico
        if (request.PartnerId.HasValue)
        {
            filtered = filtered.Where(p => p.PartnerId == request.PartnerId.Value);
        }

        // Filtro por tipo de negócio
        if (request.BusinessTypeId.HasValue)
        {
            var commissionIds = commissions
                .Where(c => c.Bussiness?.BussinessTypeId == request.BusinessTypeId.Value)
                .Select(c => c.Id)
                .ToList();
            filtered = filtered.Where(p => commissionIds.Contains(p.ComissionId));
        }

        // Filtro por nível (usando TipoPagamento como proxy para nível)
        if (request.Level.HasValue)
        {
            string targetType = request.Level.Value switch
            {
                1 => Domain.ValueObjects.ComissionPayment.RecomendadorPagamento,
                2 => Domain.ValueObjects.ComissionPayment.IntermediarioPagamento,
                3 => Domain.ValueObjects.ComissionPayment.ParticipantePagamento,
                _ => ""
            };
            
            if (!string.IsNullOrEmpty(targetType))
            {
                filtered = filtered.Where(p => p.TipoPagamento == targetType);
            }
        }

        return filtered.ToList();
    }

    private Task<FinancialReportData> BuildReportData(
        Guid vetorId,
        string vetorName,
        FinancialReportRequest request,
        List<Domain.ValueObjects.ComissionPayment> payments,
        List<Domain.Entities.Partner> partners,
        List<Domain.Entities.Bussiness> businesses,
        List<Domain.Entities.BusinessType> businessTypes)
    {
        // Construir informações do período
        var period = new PeriodSummaryDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = BuildPeriodDescription(request.StartDate, request.EndDate)
        };

        // Calcular totais gerais
        var totals = CalculateGeneralTotals(payments, businesses);

        // Calcular resumo por níveis (tipos de pagamento)
        var levelsSummary = CalculateLevelsSummary(payments, partners);

        // Calcular resumo por tipo de negócio
        var businessTypesSummary = CalculateBusinessTypesSummary(payments, businesses, businessTypes);

        // Calcular top parceiros
        var topPartners = CalculateTopPartners(payments, partners);

        // Para relatório de um vetor específico, VetorsSummary será vazio
        // Poderia ser implementado para comparar múltiplos vetores no futuro
        var vetorsSummary = Enumerable.Empty<VetorFinancialSummaryDto>();

        var reportData = new FinancialReportData
        {
            VetorId = vetorId,
            VetorName = vetorName,
            Period = period,
            Totals = totals,
            LevelsSummary = levelsSummary.OrderBy(l => l.Level),
            VetorsSummary = vetorsSummary,
            BusinessTypesSummary = businessTypesSummary.OrderByDescending(b => b.TotalGeneral),
            TopPartners = topPartners.OrderByDescending(p => p.TotalGeneral).Take(10)
        };

        return Task.FromResult(reportData);
    }

    private static string BuildPeriodDescription(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue)
        {
            return $"{startDate.Value:dd/MM/yyyy} a {endDate.Value:dd/MM/yyyy}";
        }

        if (startDate.HasValue)
        {
            return $"A partir de {startDate.Value:dd/MM/yyyy}";
        }

        if (endDate.HasValue)
        {
            return $"Até {endDate.Value:dd/MM/yyyy}";
        }

        return "Todos os períodos";
    }

    private static FinancialTotalsDto CalculateGeneralTotals(
        List<Domain.ValueObjects.ComissionPayment> payments,
        List<Domain.Entities.Bussiness> businesses)
    {
        var paidPayments = payments.Where(p => p.Status == Domain.ValueObjects.ComissionPayment.Pago).ToList();
        var pendingPayments = payments.Where(p => p.Status == Domain.ValueObjects.ComissionPayment.APagar).ToList();

        // Contar negócios únicos que geraram esses pagamentos
        var commissionIds = payments.Select(p => p.ComissionId).Distinct().ToList();
        var relatedBusinesses = businesses.Where(b => 
            b.Comissao != null && commissionIds.Contains(b.Comissao.Id)).ToList();

        return new FinancialTotalsDto
        {
            TotalPaid = paidPayments.Sum(p => p.Value),
            TotalPending = pendingPayments.Sum(p => p.Value),
            PaidCount = paidPayments.Count,
            PendingCount = pendingPayments.Count,
            BusinessCount = relatedBusinesses.Count
        };
    }

    private static List<LevelFinancialSummaryDto> CalculateLevelsSummary(
        List<Domain.ValueObjects.ComissionPayment> payments,
        List<Domain.Entities.Partner> partners)
    {
        var summary = new Dictionary<int, LevelFinancialSummaryDto>();

        // Mapear tipos de pagamento para níveis
        var typeToLevelMap = new Dictionary<string, int>
        {
            { Domain.ValueObjects.ComissionPayment.RecomendadorPagamento, 1 },
            { Domain.ValueObjects.ComissionPayment.IntermediarioPagamento, 2 },
            { Domain.ValueObjects.ComissionPayment.ParticipantePagamento, 3 },
            { Domain.ValueObjects.ComissionPayment.VetorPagamento, 0 }
        };

        // Agrupar por tipo de pagamento (nível)
        var levelGroups = payments.GroupBy(p => typeToLevelMap.GetValueOrDefault(p.TipoPagamento, 0));

        foreach (var levelGroup in levelGroups)
        {
            var level = levelGroup.Key;
            var levelPayments = levelGroup.ToList();

            var paidPayments = levelPayments.Where(p => p.Status == Domain.ValueObjects.ComissionPayment.Pago).ToList();
            var pendingPayments = levelPayments.Where(p => p.Status == Domain.ValueObjects.ComissionPayment.APagar).ToList();

            // Contar parceiros únicos neste nível
            var partnerIds = levelPayments.Select(p => p.PartnerId).Distinct().ToList();
            var partnersCount = partners.Count(p => partnerIds.Contains(p.Id));

            summary[level] = new LevelFinancialSummaryDto
            {
                Level = level,
                TotalPaid = paidPayments.Sum(p => p.Value),
                TotalPending = pendingPayments.Sum(p => p.Value),
                PaidCount = paidPayments.Count,
                PendingCount = pendingPayments.Count,
                PartnersCount = partnersCount
            };
        }

        return summary.Values.ToList();
    }

    private static List<BusinessTypeFinancialSummaryDto> CalculateBusinessTypesSummary(
        List<Domain.ValueObjects.ComissionPayment> payments,
        List<Domain.Entities.Bussiness> businesses,
        List<Domain.Entities.BusinessType> businessTypes)
    {
        var summary = new List<BusinessTypeFinancialSummaryDto>();

        foreach (var businessType in businessTypes)
        {
            // Para simplificar, vou calcular por tipo de negócio sem relações complexas
            // Em uma implementação real, seria necessário carregar as comissões com seus relacionamentos
            
            var paidPayments = payments.Where(p => p.Status == Domain.ValueObjects.ComissionPayment.Pago).ToList();
            var pendingPayments = payments.Where(p => p.Status == Domain.ValueObjects.ComissionPayment.APagar).ToList();

            // Simplified calculation - assumindo distribuição proporcional
            var typeBusinessCount = businesses.Count(b => b.BussinessTypeId == businessType.Id);
            if (typeBusinessCount == 0) continue;

            var totalBusinesses = businesses.Count;
            var proportion = totalBusinesses > 0 ? (decimal)typeBusinessCount / totalBusinesses : 0;

            summary.Add(new BusinessTypeFinancialSummaryDto
            {
                BusinessTypeId = businessType.Id,
                BusinessTypeName = businessType.Name,
                TotalPaid = paidPayments.Sum(p => p.Value) * proportion,
                TotalPending = pendingPayments.Sum(p => p.Value) * proportion,
                PaidCount = (int)(paidPayments.Count * proportion),
                PendingCount = (int)(pendingPayments.Count * proportion),
                BusinessCount = typeBusinessCount
            });
        }

        return summary;
    }

    private static List<PartnerFinancialSummaryDto> CalculateTopPartners(
        List<Domain.ValueObjects.ComissionPayment> payments,
        List<Domain.Entities.Partner> partners)
    {
        var summary = new List<PartnerFinancialSummaryDto>();

        // Agrupar pagamentos por parceiro
        var partnerGroups = payments.GroupBy(p => p.PartnerId);

        foreach (var partnerGroup in partnerGroups)
        {
            var partnerId = partnerGroup.Key;
            var partner = partners.FirstOrDefault(p => p.Id == partnerId);

            if (partner == null) continue;

            var partnerPayments = partnerGroup.ToList();
            var paidPayments = partnerPayments.Where(p => p.Status == Domain.ValueObjects.ComissionPayment.Pago).ToList();
            var pendingPayments = partnerPayments.Where(p => p.Status == Domain.ValueObjects.ComissionPayment.APagar).ToList();

            // Mapear tipo de pagamento mais comum para nível
            var typeToLevelMap = new Dictionary<string, int>
            {
                { Domain.ValueObjects.ComissionPayment.RecomendadorPagamento, 1 },
                { Domain.ValueObjects.ComissionPayment.IntermediarioPagamento, 2 },
                { Domain.ValueObjects.ComissionPayment.ParticipantePagamento, 3 },
                { Domain.ValueObjects.ComissionPayment.VetorPagamento, 0 }
            };

            var mostCommonLevel = partnerPayments.GroupBy(p => typeToLevelMap.GetValueOrDefault(p.TipoPagamento, 1))
                                                .OrderByDescending(g => g.Count())
                                                .FirstOrDefault()?.Key ?? 1;

            // Contar comissões únicas (assumindo que cada payment vem de uma comissão diferente)
            var businessCount = partnerPayments.Select(p => p.ComissionId).Distinct().Count();

            // Data do último pagamento
            var lastBusinessDate = partnerPayments.Any() ? DateTime.UtcNow : DateTime.MinValue; // Simplified

            summary.Add(new PartnerFinancialSummaryDto
            {
                PartnerId = partner.Id,
                PartnerName = partner.Name,
                PartnerEmail = partner.Email,
                Level = mostCommonLevel,
                TotalPaid = paidPayments.Sum(p => p.Value),
                TotalPending = pendingPayments.Sum(p => p.Value),
                PaidCount = paidPayments.Count,
                PendingCount = pendingPayments.Count,
                BusinessCount = businessCount,
                IsActive = partner.Active,
                LastBusinessDate = lastBusinessDate
            });
        }

        return summary;
    }
}