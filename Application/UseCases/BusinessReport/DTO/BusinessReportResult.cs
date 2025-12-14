namespace Application.UseCases.BusinessReport.DTO;

public record BusinessReportResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public BusinessReportData? Data { get; init; }

    public static BusinessReportResult Success(BusinessReportData data) 
        => new() { IsSuccess = true, Message = "Sucesso", Data = data };

    public static BusinessReportResult Failure(string message) 
        => new() { IsSuccess = false, Message = message };
}

public record BusinessReportData
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
    public PeriodInfoDto Period { get; init; } = new();

    /// <summary>
    /// Lista de negócios filtrados
    /// </summary>
    public IEnumerable<BusinessReportItemDto> Businesses { get; init; } = Enumerable.Empty<BusinessReportItemDto>();

    /// <summary>
    /// Resumo executivo
    /// </summary>
    public BusinessSummaryDto Summary { get; init; } = new();

    /// <summary>
    /// Informações de paginação
    /// </summary>
    public PaginationInfoDto Pagination { get; init; } = new();

    /// <summary>
    /// Resumo por tipo de negócio
    /// </summary>
    public IEnumerable<BusinessTypeSummaryDto> TypesSummary { get; init; } = Enumerable.Empty<BusinessTypeSummaryDto>();

    /// <summary>
    /// Top parceiros por volume de negócios
    /// </summary>
    public IEnumerable<PartnerBusinessSummaryDto> TopPartners { get; init; } = Enumerable.Empty<PartnerBusinessSummaryDto>();
}