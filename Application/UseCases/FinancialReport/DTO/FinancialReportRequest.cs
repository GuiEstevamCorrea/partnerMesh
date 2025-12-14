namespace Application.UseCases.FinancialReport.DTO;

public record FinancialReportRequest
{
    /// <summary>
    /// ID do vetor para filtrar o relatório (opcional - usa vetor do usuário se não informado)
    /// </summary>
    public Guid? VetorId { get; init; }

    /// <summary>
    /// ID do parceiro específico para filtrar (opcional)
    /// </summary>
    public Guid? PartnerId { get; init; }

    /// <summary>
    /// ID do tipo de negócio para filtrar (opcional)
    /// </summary>
    public Guid? BusinessTypeId { get; init; }

    /// <summary>
    /// Status das comissões: null = todas, true = apenas pagas, false = apenas pendentes
    /// </summary>
    public bool? IsPaid { get; init; }

    /// <summary>
    /// Data inicial para filtro por período (opcional)
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Data final para filtro por período (opcional)
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Nível específico para filtrar (1, 2 ou 3) - opcional
    /// </summary>
    public int? Level { get; init; }
}