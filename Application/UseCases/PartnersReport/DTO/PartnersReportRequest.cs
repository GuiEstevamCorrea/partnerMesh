namespace Application.UseCases.PartnersReport.DTO;

public sealed record PartnersReportRequest
{
    public Guid? VetorId { get; init; }
    public bool? ActiveOnly { get; init; }
    public int? Level { get; init; } // Filtrar por nível específico
    public string SortBy { get; init; } = "name"; // name, level, totalReceived, totalPending
    public string SortDirection { get; init; } = "asc";
}