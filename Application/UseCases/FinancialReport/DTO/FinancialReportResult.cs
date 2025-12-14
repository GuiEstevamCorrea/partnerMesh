namespace Application.UseCases.FinancialReport.DTO;

public record FinancialReportResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public FinancialReportData? Data { get; init; }

    public static FinancialReportResult Success(FinancialReportData data) 
        => new() { IsSuccess = true, Message = "Sucesso", Data = data };

    public static FinancialReportResult Failure(string message) 
        => new() { IsSuccess = false, Message = message };
}

public record FinancialReportData
{
    /// <summary>
    /// ID do vetor analisado
    /// </summary>
    public Guid VetorId { get; init; }

    /// <summary>
    /// Nome do vetor analisado
    /// </summary>
    public string VetorName { get; init; } = string.Empty;

    /// <summary>
    /// Período do relatório
    /// </summary>
    public PeriodSummaryDto Period { get; init; } = new();

    /// <summary>
    /// Totais gerais
    /// </summary>
    public FinancialTotalsDto Totals { get; init; } = new();

    /// <summary>
    /// Resumo por nível
    /// </summary>
    public IEnumerable<LevelFinancialSummaryDto> LevelsSummary { get; init; } = Enumerable.Empty<LevelFinancialSummaryDto>();

    /// <summary>
    /// Resumo por vetor (quando há múltiplos vetores)
    /// </summary>
    public IEnumerable<VetorFinancialSummaryDto> VetorsSummary { get; init; } = Enumerable.Empty<VetorFinancialSummaryDto>();

    /// <summary>
    /// Resumo por tipo de negócio
    /// </summary>
    public IEnumerable<BusinessTypeFinancialSummaryDto> BusinessTypesSummary { get; init; } = Enumerable.Empty<BusinessTypeFinancialSummaryDto>();

    /// <summary>
    /// Top parceiros por comissões recebidas
    /// </summary>
    public IEnumerable<PartnerFinancialSummaryDto> TopPartners { get; init; } = Enumerable.Empty<PartnerFinancialSummaryDto>();
}