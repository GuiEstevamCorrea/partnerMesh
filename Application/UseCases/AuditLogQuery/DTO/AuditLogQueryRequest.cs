namespace Application.UseCases.AuditLogQuery.DTO;

/// <summary>
/// Request para consulta de logs de auditoria
/// </summary>
public record AuditLogQueryRequest
{
    /// <summary>
    /// Filtro por ID do usuário
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Filtro por ação executada
    /// </summary>
    public string? Action { get; init; }

    /// <summary>
    /// Filtro por entidade afetada
    /// </summary>
    public string? Entity { get; init; }

    /// <summary>
    /// Filtro por ID da entidade afetada
    /// </summary>
    public Guid? EntityId { get; init; }

    /// <summary>
    /// Data inicial do período de consulta
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Data final do período de consulta
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Página atual (padrão: 1)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Tamanho da página (padrão: 50, máximo: 100)
    /// </summary>
    public int PageSize { get; init; } = 50;

    /// <summary>
    /// Ordenação: CreatedAt (padrão), Action, Entity, UserId
    /// </summary>
    public string OrderBy { get; init; } = "CreatedAt";

    /// <summary>
    /// Direção da ordenação: DESC (padrão), ASC
    /// </summary>
    public string OrderDirection { get; init; } = "DESC";
}