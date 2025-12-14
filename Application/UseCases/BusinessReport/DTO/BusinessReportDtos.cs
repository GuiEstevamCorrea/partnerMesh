namespace Application.UseCases.BusinessReport.DTO;

/// <summary>
/// Informações sobre o período analisado
/// </summary>
public record PeriodInfoDto
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string Description { get; init; } = "Todos os períodos";
}

/// <summary>
/// Item individual do relatório de negócios
/// </summary>
public record BusinessReportItemDto
{
    public Guid Id { get; init; }
    public decimal Value { get; init; }
    public DateTime Date { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Observations { get; init; } = string.Empty;

    // Dados do parceiro
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public string PartnerEmail { get; init; } = string.Empty;
    public bool PartnerActive { get; init; }

    // Dados do tipo de negócio
    public Guid BusinessTypeId { get; init; }
    public string BusinessTypeName { get; init; } = string.Empty;
    public string BusinessTypeDescription { get; init; } = string.Empty;

    // Dados da comissão
    public Guid? CommissionId { get; init; }
    public decimal CommissionTotalValue { get; init; }
    public DateTime? CommissionCreatedAt { get; init; }
    public int PaidPaymentsCount { get; init; }
    public int PendingPaymentsCount { get; init; }
    public int TotalPaymentsCount { get; init; }
    public decimal PaidCommissionValue { get; init; }
    public decimal PendingCommissionValue { get; init; }
    public string CommissionStatus { get; init; } = string.Empty; // "Totalmente Paga", "Parcialmente Paga", "Pendente", "Sem Comissão"
}

/// <summary>
/// Resumo executivo do relatório
/// </summary>
public record BusinessSummaryDto
{
    public int TotalBusinesses { get; init; }
    public int ActiveBusinesses { get; init; }
    public int CancelledBusinesses { get; init; }
    public decimal TotalValue { get; init; }
    public decimal AverageValue { get; init; }
    public decimal TotalCommissionValue { get; init; }
    public decimal PaidCommissionValue { get; init; }
    public decimal PendingCommissionValue { get; init; }
    public int TotalCommissionPayments { get; init; }
    public int PaidCommissionPayments { get; init; }
    public int PendingCommissionPayments { get; init; }
    public int UniquePartnersCount { get; init; }
    public int UniqueBusinessTypesCount { get; init; }
}

/// <summary>
/// Informações de paginação
/// </summary>
public record PaginationInfoDto
{
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}

/// <summary>
/// Resumo por tipo de negócio
/// </summary>
public record BusinessTypeSummaryDto
{
    public Guid BusinessTypeId { get; init; }
    public string BusinessTypeName { get; init; } = string.Empty;
    public int BusinessCount { get; init; }
    public decimal TotalValue { get; init; }
    public decimal AverageValue { get; init; }
    public decimal TotalCommissionValue { get; init; }
    public decimal PaidCommissionValue { get; init; }
    public decimal PendingCommissionValue { get; init; }
    public decimal CommissionPercentage { get; init; } // Percentual em relação ao total
}

/// <summary>
/// Resumo por parceiro
/// </summary>
public record PartnerBusinessSummaryDto
{
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public string PartnerEmail { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int BusinessCount { get; init; }
    public decimal TotalValue { get; init; }
    public decimal AverageValue { get; init; }
    public decimal TotalCommissionValue { get; init; }
    public decimal PaidCommissionValue { get; init; }
    public decimal PendingCommissionValue { get; init; }
    public DateTime LastBusinessDate { get; init; }
    public string MostCommonBusinessType { get; init; } = string.Empty;
}