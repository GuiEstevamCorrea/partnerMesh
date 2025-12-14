namespace Application.UseCases.BusinessReport.DTO;

public record BusinessReportRequest
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
    /// Valor mínimo do negócio (opcional)
    /// </summary>
    public decimal? MinValue { get; init; }

    /// <summary>
    /// Valor máximo do negócio (opcional)
    /// </summary>
    public decimal? MaxValue { get; init; }

    /// <summary>
    /// Data inicial para filtro por período (opcional)
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Data final para filtro por período (opcional)
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Status do negócio: null = todos, "ativo" = apenas ativos, "cancelado" = apenas cancelados
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Status das comissões: null = todos, true = comissões pagas, false = comissões pendentes
    /// </summary>
    public bool? CommissionPaid { get; init; }

    /// <summary>
    /// Campo para ordenação (date, value, partner, type)
    /// </summary>
    public string SortBy { get; init; } = "date";

    /// <summary>
    /// Direção da ordenação (asc ou desc)
    /// </summary>
    public string SortDirection { get; init; } = "desc";

    /// <summary>
    /// Página para paginação (opcional)
    /// </summary>
    public int? Page { get; init; }

    /// <summary>
    /// Tamanho da página para paginação (padrão 50)
    /// </summary>
    public int PageSize { get; init; } = 50;
}