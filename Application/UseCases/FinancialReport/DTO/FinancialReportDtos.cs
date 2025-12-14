namespace Application.UseCases.FinancialReport.DTO;

/// <summary>
/// Informações sobre o período analisado
/// </summary>
public record PeriodSummaryDto
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string Description { get; init; } = "Todos os períodos";
}

/// <summary>
/// Totais financeiros gerais
/// </summary>
public record FinancialTotalsDto
{
    /// <summary>
    /// Total de comissões pagas
    /// </summary>
    public decimal TotalPaid { get; init; }

    /// <summary>
    /// Total de comissões pendentes
    /// </summary>
    public decimal TotalPending { get; init; }

    /// <summary>
    /// Total geral (pago + pendente)
    /// </summary>
    public decimal TotalGeneral => TotalPaid + TotalPending;

    /// <summary>
    /// Quantidade de comissões pagas
    /// </summary>
    public int PaidCount { get; init; }

    /// <summary>
    /// Quantidade de comissões pendentes
    /// </summary>
    public int PendingCount { get; init; }

    /// <summary>
    /// Total de negócios que geraram as comissões
    /// </summary>
    public int BusinessCount { get; init; }

    /// <summary>
    /// Valor médio das comissões pagas
    /// </summary>
    public decimal AveragePaidCommission => PaidCount > 0 ? TotalPaid / PaidCount : 0;

    /// <summary>
    /// Valor médio das comissões pendentes
    /// </summary>
    public decimal AveragePendingCommission => PendingCount > 0 ? TotalPending / PendingCount : 0;
}

/// <summary>
/// Resumo financeiro por nível hierárquico
/// </summary>
public record LevelFinancialSummaryDto
{
    public int Level { get; init; }
    public decimal TotalPaid { get; init; }
    public decimal TotalPending { get; init; }
    public decimal TotalGeneral => TotalPaid + TotalPending;
    public int PaidCount { get; init; }
    public int PendingCount { get; init; }
    public int PartnersCount { get; init; }
    public decimal AveragePerPartner => PartnersCount > 0 ? TotalGeneral / PartnersCount : 0;
}

/// <summary>
/// Resumo financeiro por vetor
/// </summary>
public record VetorFinancialSummaryDto
{
    public Guid VetorId { get; init; }
    public string VetorName { get; init; } = string.Empty;
    public decimal TotalPaid { get; init; }
    public decimal TotalPending { get; init; }
    public decimal TotalGeneral => TotalPaid + TotalPending;
    public int PaidCount { get; init; }
    public int PendingCount { get; init; }
    public int PartnersCount { get; init; }
    public int BusinessCount { get; init; }
}

/// <summary>
/// Resumo financeiro por tipo de negócio
/// </summary>
public record BusinessTypeFinancialSummaryDto
{
    public Guid BusinessTypeId { get; init; }
    public string BusinessTypeName { get; init; } = string.Empty;
    public decimal TotalPaid { get; init; }
    public decimal TotalPending { get; init; }
    public decimal TotalGeneral => TotalPaid + TotalPending;
    public int PaidCount { get; init; }
    public int PendingCount { get; init; }
    public int BusinessCount { get; init; }
    public decimal AverageCommissionPerBusiness => BusinessCount > 0 ? TotalGeneral / BusinessCount : 0;
}

/// <summary>
/// Resumo financeiro por parceiro
/// </summary>
public record PartnerFinancialSummaryDto
{
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public string PartnerEmail { get; init; } = string.Empty;
    public int Level { get; init; }
    public decimal TotalPaid { get; init; }
    public decimal TotalPending { get; init; }
    public decimal TotalGeneral => TotalPaid + TotalPending;
    public int PaidCount { get; init; }
    public int PendingCount { get; init; }
    public int BusinessCount { get; init; }
    public bool IsActive { get; init; }
    public DateTime LastBusinessDate { get; init; }
}