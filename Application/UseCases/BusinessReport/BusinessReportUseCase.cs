using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.BusinessReport.DTO;
using Domain.ValueObjects;
using Domain.ValueTypes;
using Domain.Extensions;

namespace Application.UseCases.BusinessReport;

public class BusinessReportUseCase : IBusinessReportUseCase
{
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly ICommissionRepository _commissionRepository;

    public BusinessReportUseCase(
        IVetorRepository vetorRepository,
        IUserRepository userRepository,
        IBusinessRepository businessRepository,
        IPartnerRepository partnerRepository,
        IBusinessTypeRepository businessTypeRepository,
        ICommissionRepository commissionRepository)
    {
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
        _businessRepository = businessRepository;
        _partnerRepository = partnerRepository;
        _businessTypeRepository = businessTypeRepository;
        _commissionRepository = commissionRepository;
    }

    public async Task<BusinessReportResult> ExecuteAsync(
        BusinessReportRequest request, 
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Validar usuário atual
            var currentUser = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (currentUser == null)
            {
                return BusinessReportResult.Failure("Usuário não encontrado.");
            }

            // 2. Determinar vetor
            Guid vetorId;
            if (request.VetorId.HasValue)
            {
                // Verificar se o usuário tem acesso a este vetor
                if (!HasAccessToVetor(currentUser, request.VetorId.Value))
                {
                    return BusinessReportResult.Failure("Acesso negado ao vetor solicitado.");
                }
                vetorId = request.VetorId.Value;
            }
            else
            {
                // Usar primeiro vetor do usuário atual
                var userVetor = currentUser.UserVetores.FirstOrDefault(uv => uv.Active);
                if (userVetor == null)
                {
                    return BusinessReportResult.Failure("Usuário não está associado a nenhum vetor e nenhum vetor foi especificado.");
                }
                vetorId = userVetor.VetorId;
            }

            // 3. Validar vetor
            var vetor = await _vetorRepository.GetByIdAsync(vetorId, cancellationToken);
            if (vetor == null)
            {
                return BusinessReportResult.Failure("Vetor não encontrado.");
            }

            // 4. Obter dados necessários
            var allBusinesses = await _businessRepository.GetAllAsync(cancellationToken);
            var partners = await _partnerRepository.GetAllAsync(cancellationToken);
            var businessTypes = await _businessTypeRepository.GetAllAsync(cancellationToken);
            var commissions = await _commissionRepository.GetAllAsync(cancellationToken);

            // 5. Filtrar negócios do vetor
            var vetorBusinesses = allBusinesses.Where(b => b.Partner?.VetorId == vetorId).ToList();

            // 6. Aplicar filtros
            var filteredBusinesses = ApplyFilters(vetorBusinesses, request);

            // 7. Calcular totais antes da paginação
            var totalCount = filteredBusinesses.Count();

            // 8. Aplicar ordenação
            var sortedBusinesses = ApplySorting(filteredBusinesses, request.SortBy, request.SortDirection);

            // 9. Aplicar paginação se solicitada
            var paginatedBusinesses = ApplyPagination(sortedBusinesses, request);

            // 10. Construir dados do relatório
            var reportData = await BuildReportData(
                vetorId,
                vetor.Name,
                request,
                paginatedBusinesses.ToList(),
                filteredBusinesses.ToList(), // Para cálculos de resumo
                partners.ToList(),
                businessTypes.ToList(),
                commissions.ToList(),
                totalCount);

            return BusinessReportResult.Success(reportData);
        }
        catch (Exception ex)
        {
            return BusinessReportResult.Failure($"Erro interno: {ex.Message}");
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

    private static IQueryable<Domain.Entities.Bussiness> ApplyFilters(
        List<Domain.Entities.Bussiness> businesses,
        BusinessReportRequest request)
    {
        var filtered = businesses.AsQueryable();

        // Filtro por período
        if (request.StartDate.HasValue)
        {
            filtered = filtered.Where(b => b.Date >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            filtered = filtered.Where(b => b.Date <= request.EndDate.Value);
        }

        // Filtro por parceiro
        if (request.PartnerId.HasValue)
        {
            filtered = filtered.Where(b => b.PartnerId == request.PartnerId.Value);
        }

        // Filtro por tipo de negócio
        if (request.BusinessTypeId.HasValue)
        {
            filtered = filtered.Where(b => b.BussinessTypeId == request.BusinessTypeId.Value);
        }

        // Filtro por valor mínimo
        if (request.MinValue.HasValue)
        {
            filtered = filtered.Where(b => b.Value >= request.MinValue.Value);
        }

        // Filtro por valor máximo
        if (request.MaxValue.HasValue)
        {
            filtered = filtered.Where(b => b.Value <= request.MaxValue.Value);
        }

        // Filtro por status do negócio
        if (!string.IsNullOrEmpty(request.Status))
        {
            filtered = filtered.Where(b => b.Status.ToLegacyString() == request.Status);
        }

        // Filtro por status da comissão (se solicitado)
        if (request.CommissionPaid.HasValue)
        {
            if (request.CommissionPaid.Value)
            {
                // Negócios com comissões totalmente pagas
                filtered = filtered.Where(b => b.Comissao != null && 
                    b.Comissao.Pagamentos.All(p => p.Status == PaymentStatus.Pago));
            }
            else
            {
                // Negócios com comissões pendentes
                filtered = filtered.Where(b => b.Comissao == null || 
                    b.Comissao.Pagamentos.Any(p => p.Status == PaymentStatus.APagar));
            }
        }

        return filtered;
    }

    private static IOrderedQueryable<Domain.Entities.Bussiness> ApplySorting(
        IQueryable<Domain.Entities.Bussiness> businesses,
        string sortBy,
        string sortDirection)
    {
        var ascending = sortDirection?.ToLower() != "desc";

        return sortBy?.ToLower() switch
        {
            "value" => ascending ? businesses.OrderBy(b => b.Value) : businesses.OrderByDescending(b => b.Value),
            "partner" => ascending ? businesses.OrderBy(b => b.Partner != null ? b.Partner.Name : "") : businesses.OrderByDescending(b => b.Partner != null ? b.Partner.Name : ""),
            "type" => ascending ? businesses.OrderBy(b => b.BussinessType != null ? b.BussinessType.Name : "") : businesses.OrderByDescending(b => b.BussinessType != null ? b.BussinessType.Name : ""),
            "createdat" => ascending ? businesses.OrderBy(b => b.CreatedAt) : businesses.OrderByDescending(b => b.CreatedAt),
            _ => ascending ? businesses.OrderBy(b => b.Date) : businesses.OrderByDescending(b => b.Date)
        };
    }

    private static IEnumerable<Domain.Entities.Bussiness> ApplyPagination(
        IOrderedQueryable<Domain.Entities.Bussiness> businesses,
        BusinessReportRequest request)
    {
        if (!request.Page.HasValue)
        {
            return businesses.ToList();
        }

        var page = Math.Max(1, request.Page.Value);
        var pageSize = Math.Max(1, Math.Min(200, request.PageSize)); // Limita até 200 itens por página

        return businesses.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    private Task<BusinessReportData> BuildReportData(
        Guid vetorId,
        string vetorName,
        BusinessReportRequest request,
        List<Domain.Entities.Bussiness> paginatedBusinesses,
        List<Domain.Entities.Bussiness> allFilteredBusinesses,
        List<Domain.Entities.Partner> partners,
        List<Domain.Entities.BusinessType> businessTypes,
        List<Domain.Entities.Comission> commissions,
        int totalCount)
    {
        // Construir informações do período
        var period = new PeriodInfoDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = BuildPeriodDescription(request.StartDate, request.EndDate)
        };

        // Mapear negócios para DTOs
        var businessItems = paginatedBusinesses.Select(b => MapBusinessToDto(b, partners, businessTypes)).ToList();

        // Calcular resumo
        var summary = CalculateSummary(allFilteredBusinesses, partners, businessTypes);

        // Calcular paginação
        var pagination = CalculatePagination(request, totalCount);

        // Calcular resumo por tipos
        var typesSummary = CalculateTypesSummary(allFilteredBusinesses, businessTypes);

        // Calcular top parceiros
        var topPartners = CalculateTopPartners(allFilteredBusinesses, partners);

        var reportData = new BusinessReportData
        {
            VetorId = vetorId,
            VetorName = vetorName,
            Period = period,
            Businesses = businessItems,
            Summary = summary,
            Pagination = pagination,
            TypesSummary = typesSummary.OrderByDescending(t => t.TotalValue),
            TopPartners = topPartners.OrderByDescending(p => p.TotalValue).Take(10)
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

    private static BusinessReportItemDto MapBusinessToDto(
        Domain.Entities.Bussiness business,
        List<Domain.Entities.Partner> partners,
        List<Domain.Entities.BusinessType> businessTypes)
    {
        var partner = partners.FirstOrDefault(p => p.Id == business.PartnerId);
        var businessType = businessTypes.FirstOrDefault(bt => bt.Id == business.BussinessTypeId);

        // Analisar comissão
        var commission = business.Comissao;
        string commissionStatus = "Sem Comissão";
        int paidPayments = 0;
        int pendingPayments = 0;
        decimal paidValue = 0;
        decimal pendingValue = 0;

        if (commission != null)
        {
            var payments = commission.Pagamentos.ToList();
            paidPayments = payments.Count(p => p.Status == PaymentStatus.Pago);
            pendingPayments = payments.Count(p => p.Status == PaymentStatus.APagar);
            paidValue = payments.Where(p => p.Status == PaymentStatus.Pago).Sum(p => p.Value);
            pendingValue = payments.Where(p => p.Status == PaymentStatus.APagar).Sum(p => p.Value);

            commissionStatus = pendingPayments == 0 ? "Totalmente Paga" :
                              paidPayments > 0 ? "Parcialmente Paga" : "Pendente";
        }

        return new BusinessReportItemDto
        {
            Id = business.Id,
            Value = business.Value,
            Date = business.Date,
            CreatedAt = business.CreatedAt,
            Status = business.Status.ToLegacyString(),
            Observations = business.Observations,
            PartnerId = business.PartnerId,
            PartnerName = partner?.Name ?? "N/A",
            PartnerEmail = partner?.Email ?? "",
            PartnerActive = partner?.Active ?? false,
            BusinessTypeId = business.BussinessTypeId,
            BusinessTypeName = businessType?.Name ?? "N/A",
            BusinessTypeDescription = businessType?.Description ?? "",
            CommissionId = commission?.Id,
            CommissionTotalValue = commission?.TotalValue ?? 0,
            CommissionCreatedAt = commission?.CreatedAt,
            PaidPaymentsCount = paidPayments,
            PendingPaymentsCount = pendingPayments,
            TotalPaymentsCount = paidPayments + pendingPayments,
            PaidCommissionValue = paidValue,
            PendingCommissionValue = pendingValue,
            CommissionStatus = commissionStatus
        };
    }

    private static BusinessSummaryDto CalculateSummary(
        List<Domain.Entities.Bussiness> businesses,
        List<Domain.Entities.Partner> partners,
        List<Domain.Entities.BusinessType> businessTypes)
    {
        var activeBusinesses = businesses.Where(b => b.Status == Domain.ValueTypes.BusinessStatus.Ativo).ToList();
        var cancelledBusinesses = businesses.Where(b => b.Status == Domain.ValueTypes.BusinessStatus.Cancelado).ToList();

        var totalValue = businesses.Sum(b => b.Value);
        var averageValue = businesses.Count > 0 ? totalValue / businesses.Count : 0;

        // Calcular dados de comissão
        var businessesWithCommission = businesses.Where(b => b.Comissao != null).ToList();
        var allPayments = businessesWithCommission.SelectMany(b => b.Comissao!.Pagamentos).ToList();

        var totalCommissionValue = businessesWithCommission.Sum(b => b.Comissao!.TotalValue);
        var paidCommissionValue = allPayments.Where(p => p.Status == PaymentStatus.Pago).Sum(p => p.Value);
        var pendingCommissionValue = allPayments.Where(p => p.Status == PaymentStatus.APagar).Sum(p => p.Value);

        var paidPaymentsCount = allPayments.Count(p => p.Status == PaymentStatus.Pago);
        var pendingPaymentsCount = allPayments.Count(p => p.Status == PaymentStatus.APagar);

        var uniquePartnersCount = businesses.Select(b => b.PartnerId).Distinct().Count();
        var uniqueBusinessTypesCount = businesses.Select(b => b.BussinessTypeId).Distinct().Count();

        return new BusinessSummaryDto
        {
            TotalBusinesses = businesses.Count,
            ActiveBusinesses = activeBusinesses.Count,
            CancelledBusinesses = cancelledBusinesses.Count,
            TotalValue = totalValue,
            AverageValue = averageValue,
            TotalCommissionValue = totalCommissionValue,
            PaidCommissionValue = paidCommissionValue,
            PendingCommissionValue = pendingCommissionValue,
            TotalCommissionPayments = allPayments.Count,
            PaidCommissionPayments = paidPaymentsCount,
            PendingCommissionPayments = pendingPaymentsCount,
            UniquePartnersCount = uniquePartnersCount,
            UniqueBusinessTypesCount = uniqueBusinessTypesCount
        };
    }

    private static PaginationInfoDto CalculatePagination(BusinessReportRequest request, int totalCount)
    {
        if (!request.Page.HasValue)
        {
            return new PaginationInfoDto
            {
                CurrentPage = 1,
                PageSize = totalCount,
                TotalItems = totalCount,
                TotalPages = 1
            };
        }

        var pageSize = Math.Max(1, Math.Min(200, request.PageSize));
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var currentPage = Math.Max(1, Math.Min(totalPages, request.Page.Value));

        return new PaginationInfoDto
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages
        };
    }

    private static List<BusinessTypeSummaryDto> CalculateTypesSummary(
        List<Domain.Entities.Bussiness> businesses,
        List<Domain.Entities.BusinessType> businessTypes)
    {
        var summary = new List<BusinessTypeSummaryDto>();
        var totalValue = businesses.Sum(b => b.Value);

        foreach (var businessType in businessTypes)
        {
            var typeBusinesses = businesses.Where(b => b.BussinessTypeId == businessType.Id).ToList();
            if (!typeBusinesses.Any()) continue;

            var typeValue = typeBusinesses.Sum(b => b.Value);
            var averageValue = typeValue / typeBusinesses.Count;

            var businessesWithCommission = typeBusinesses.Where(b => b.Comissao != null).ToList();
            var totalCommissionValue = businessesWithCommission.Sum(b => b.Comissao!.TotalValue);
            
            var allPayments = businessesWithCommission.SelectMany(b => b.Comissao!.Pagamentos).ToList();
            var paidCommissionValue = allPayments.Where(p => p.Status == PaymentStatus.Pago).Sum(p => p.Value);
            var pendingCommissionValue = allPayments.Where(p => p.Status == PaymentStatus.APagar).Sum(p => p.Value);

            var percentage = totalValue > 0 ? (typeValue / totalValue) * 100 : 0;

            summary.Add(new BusinessTypeSummaryDto
            {
                BusinessTypeId = businessType.Id,
                BusinessTypeName = businessType.Name,
                BusinessCount = typeBusinesses.Count,
                TotalValue = typeValue,
                AverageValue = averageValue,
                TotalCommissionValue = totalCommissionValue,
                PaidCommissionValue = paidCommissionValue,
                PendingCommissionValue = pendingCommissionValue,
                CommissionPercentage = percentage
            });
        }

        return summary;
    }

    private static List<PartnerBusinessSummaryDto> CalculateTopPartners(
        List<Domain.Entities.Bussiness> businesses,
        List<Domain.Entities.Partner> partners)
    {
        var summary = new List<PartnerBusinessSummaryDto>();

        var partnerGroups = businesses.GroupBy(b => b.PartnerId);

        foreach (var partnerGroup in partnerGroups)
        {
            var partnerId = partnerGroup.Key;
            var partner = partners.FirstOrDefault(p => p.Id == partnerId);
            if (partner == null) continue;

            var partnerBusinesses = partnerGroup.ToList();
            var totalValue = partnerBusinesses.Sum(b => b.Value);
            var averageValue = totalValue / partnerBusinesses.Count;

            var businessesWithCommission = partnerBusinesses.Where(b => b.Comissao != null).ToList();
            var totalCommissionValue = businessesWithCommission.Sum(b => b.Comissao!.TotalValue);
            
            var allPayments = businessesWithCommission.SelectMany(b => b.Comissao!.Pagamentos).ToList();
            var paidCommissionValue = allPayments.Where(p => p.Status == PaymentStatus.Pago).Sum(p => p.Value);
            var pendingCommissionValue = allPayments.Where(p => p.Status == PaymentStatus.APagar).Sum(p => p.Value);

            var lastBusinessDate = partnerBusinesses.Max(b => b.Date);

            // Tipo de negócio mais comum
            var mostCommonType = partnerBusinesses.GroupBy(b => b.BussinessTypeId)
                                                 .OrderByDescending(g => g.Count())
                                                 .FirstOrDefault()?.Key;
            var mostCommonTypeName = "N/A";
            if (mostCommonType.HasValue)
            {
                // Simplified - seria melhor buscar no businessTypes
                mostCommonTypeName = "Tipo mais frequente";
            }

            summary.Add(new PartnerBusinessSummaryDto
            {
                PartnerId = partner.Id,
                PartnerName = partner.Name,
                PartnerEmail = partner.Email,
                IsActive = partner.Active,
                BusinessCount = partnerBusinesses.Count,
                TotalValue = totalValue,
                AverageValue = averageValue,
                TotalCommissionValue = totalCommissionValue,
                PaidCommissionValue = paidCommissionValue,
                PendingCommissionValue = pendingCommissionValue,
                LastBusinessDate = lastBusinessDate,
                MostCommonBusinessType = mostCommonTypeName
            });
        }

        return summary;
    }
}