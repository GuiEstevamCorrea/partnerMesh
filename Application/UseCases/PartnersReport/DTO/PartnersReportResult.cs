namespace Application.UseCases.PartnersReport.DTO;

public sealed record PartnersReportResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public PartnersReportData? Report { get; init; }

    public static PartnersReportResult Success(PartnersReportData report)
        => new() { IsSuccess = true, Message = "Relatório de parceiros gerado com sucesso.", Report = report };

    public static PartnersReportResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record PartnersReportData
{
    public string VetorName { get; init; } = string.Empty;
    public Guid VetorId { get; init; }
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
    
    // Árvore hierárquica de parceiros
    public IEnumerable<PartnerTreeNodeDto> PartnersTree { get; init; } = Enumerable.Empty<PartnerTreeNodeDto>();
    
    // Resumo por nível
    public IEnumerable<LevelSummaryDto> LevelsSummary { get; init; } = Enumerable.Empty<LevelSummaryDto>();
    
    // Totais gerais
    public ReportTotalsDto Totals { get; init; } = new();
}

public sealed record PartnerTreeNodeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public int Level { get; init; }
    
    // Financeiro
    public decimal TotalReceived { get; init; }
    public decimal TotalPending { get; init; }
    public int BusinessCount { get; init; }
    
    // Hierarquia
    public Guid? RecommenderId { get; init; }
    public string RecommenderName { get; init; } = string.Empty;
    public IEnumerable<PartnerTreeNodeDto> Children { get; init; } = Enumerable.Empty<PartnerTreeNodeDto>();
}

public sealed record LevelSummaryDto
{
    public int Level { get; init; }
    public int ActivePartnersCount { get; init; }
    public int InactivePartnersCount { get; init; }
    public int TotalPartnersCount => ActivePartnersCount + InactivePartnersCount;
    public decimal TotalReceived { get; init; }
    public decimal TotalPending { get; init; }
    public int TotalBusinessCount { get; init; }
}

public sealed record ReportTotalsDto
{
    public int TotalActivePartners { get; init; }
    public int TotalInactivePartners { get; init; }
    public int TotalPartners => TotalActivePartners + TotalInactivePartners;
    public decimal TotalReceived { get; init; }
    public decimal TotalPending { get; init; }
    public int TotalBusinessCount { get; init; }
    public int MaxDepth { get; init; }
}