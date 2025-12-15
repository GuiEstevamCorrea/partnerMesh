using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ListBusinesses.DTO;
using Domain.ValueObjects;
using Domain.ValueTypes;
using Domain.Extensions;

namespace Application.UseCases.ListBusinesses;

public class ListBusinessesUseCase : IListBusinessesUseCase
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;

    public ListBusinessesUseCase(
        IBusinessRepository businessRepository,
        ICommissionRepository commissionRepository,
        IPartnerRepository partnerRepository,
        IBusinessTypeRepository businessTypeRepository)
    {
        _businessRepository = businessRepository;
        _commissionRepository = commissionRepository;
        _partnerRepository = partnerRepository;
        _businessTypeRepository = businessTypeRepository;
    }

    public async Task<ListBusinessesResult> ExecuteAsync(ListBusinessesRequest request, Guid userId)
    {
        try
        {
            // Validar filtros
            if (request.MinValue.HasValue && request.MaxValue.HasValue && request.MinValue > request.MaxValue)
            {
                return ListBusinessesResult.Failure("Valor mínimo não pode ser maior que o valor máximo");
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate > request.EndDate)
            {
                return ListBusinessesResult.Failure("Data inicial não pode ser maior que a data final");
            }

            // Converter strings de ordenação para enums
            var sortField = BusinessSortField.Date;
            var sortDirection = SortDirection.Descending;
            
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                BusinessSortFieldExtensions.TryParse(request.SortBy, out sortField);
            }
            
            if (!string.IsNullOrWhiteSpace(request.SortDirection))
            {
                SortDirectionExtensions.TryParse(request.SortDirection, out sortDirection);
            }

            // Buscar negócios com filtros e paginação
            var (businesses, totalCount) = await _businessRepository.GetWithFiltersAsync(
                partnerId: request.PartnerId,
                businessTypeId: request.BusinessTypeId,
                status: request.Status,
                minValue: request.MinValue,
                maxValue: request.MaxValue,
                startDate: request.StartDate,
                endDate: request.EndDate,
                searchText: request.SearchText,
                sortBy: sortField,
                sortDirection: sortDirection,
                page: request.Page,
                pageSize: request.PageSize);

            // Converter para DTOs
            var businessDtos = new List<BusinessListDto>();
            
            foreach (var business in businesses)
            {
                // Buscar informações do parceiro e tipo de negócio
                var partner = await _partnerRepository.GetByIdAsync(business.PartnerId);
                var businessType = await _businessTypeRepository.GetByIdAsync(business.BussinessTypeId);
                
                // Buscar informações da comissão
                var commission = await _commissionRepository.GetByBusinessIdAsync(business.Id);
                var commissionInfo = await BuildCommissionInfo(commission);

                businessDtos.Add(new BusinessListDto
                {
                    Id = business.Id,
                    PartnerId = business.PartnerId,
                    PartnerName = partner?.Name ?? "Partner não encontrado",
                    BusinessTypeId = business.BussinessTypeId,
                    BusinessTypeName = businessType?.Name ?? "Tipo não encontrado",
                    Value = business.Value,
                    Status = business.Status.ToLegacyString(),
                    Date = business.Date,
                    Observations = business.Observations,
                    CreatedAt = business.CreatedAt,
                    Commission = commissionInfo
                });
            }

            // Calcular informações de paginação
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var pagination = new PaginationInfo
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = totalPages,
                HasPrevious = request.Page > 1,
                HasNext = request.Page < totalPages
            };

            // Calcular sumário
            var summary = await CalculateBusinessSummary(businesses);

            return ListBusinessesResult.Success(businessDtos, pagination, summary);
        }
        catch (Exception ex)
        {
            return ListBusinessesResult.Failure($"Erro ao listar negócios: {ex.Message}");
        }
    }

    private async Task<CommissionInfo> BuildCommissionInfo(Domain.Entities.Comission? commission)
    {
        if (commission == null)
        {
            return new CommissionInfo
            {
                CommissionStatus = "Não encontrada"
            };
        }

        var totalPayments = commission.Pagamentos.Count;
        var paidPayments = commission.Pagamentos.Count(p => p.Status == ComissionPayment.Pago);
        var pendingPayments = commission.Pagamentos.Count(p => p.Status == ComissionPayment.APagar);
        var cancelledPayments = commission.Pagamentos.Count(p => p.Status == ComissionPayment.Cancelado);
        
        var paidValue = commission.Pagamentos
            .Where(p => p.Status == ComissionPayment.Pago)
            .Sum(p => p.Value);
        
        var pendingValue = commission.Pagamentos
            .Where(p => p.Status == ComissionPayment.APagar)
            .Sum(p => p.Value);

        // Determinar status da comissão
        string commissionStatus;
        if (cancelledPayments == totalPayments)
            commissionStatus = "Cancelado";
        else if (paidPayments == totalPayments)
            commissionStatus = "Totalmente Pago";
        else if (paidPayments > 0)
            commissionStatus = "Parcialmente Pago";
        else
            commissionStatus = "Pendente";

        return new CommissionInfo
        {
            CommissionId = commission.Id,
            TotalValue = commission.TotalValue,
            TotalPayments = totalPayments,
            PaidPayments = paidPayments,
            PendingPayments = pendingPayments,
            CancelledPayments = cancelledPayments,
            PaidValue = paidValue,
            PendingValue = pendingValue,
            CommissionStatus = commissionStatus
        };
    }

    private async Task<BusinessSummary> CalculateBusinessSummary(IEnumerable<Domain.Entities.Bussiness> businesses)
    {
        var businessList = businesses.ToList();
        
        var totalValue = businessList.Sum(b => b.Value);
        var activeCount = businessList.Count(b => b.Status == Domain.ValueTypes.BusinessStatus.Ativo);
        var cancelledCount = businessList.Count(b => b.Status == Domain.ValueTypes.BusinessStatus.Cancelado);

        decimal totalCommissions = 0;
        decimal paidCommissions = 0;
        decimal pendingCommissions = 0;

        foreach (var business in businessList)
        {
            var commission = await _commissionRepository.GetByBusinessIdAsync(business.Id);
            if (commission != null)
            {
                totalCommissions += commission.TotalValue;
                paidCommissions += commission.Pagamentos
                    .Where(p => p.Status == ComissionPayment.Pago)
                    .Sum(p => p.Value);
                pendingCommissions += commission.Pagamentos
                    .Where(p => p.Status == ComissionPayment.APagar)
                    .Sum(p => p.Value);
            }
        }

        return new BusinessSummary
        {
            TotalBusinesses = businessList.Count,
            ActiveBusinesses = activeCount,
            CancelledBusinesses = cancelledCount,
            TotalValue = totalValue,
            TotalCommissions = totalCommissions,
            PaidCommissions = paidCommissions,
            PendingCommissions = pendingCommissions
        };
    }
}