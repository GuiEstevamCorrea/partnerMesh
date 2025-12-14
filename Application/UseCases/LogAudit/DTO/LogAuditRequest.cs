namespace Application.UseCases.LogAudit.DTO;

public record LogAuditRequest
{
    /// <summary>
    /// ID do usuário que executou a ação
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Ação executada (CREATE, UPDATE, DELETE, LOGIN, LOGOUT, PAYMENT, etc.)
    /// </summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>
    /// Nome da entidade afetada (User, Partner, Business, Commission, etc.)
    /// </summary>
    public string Entity { get; init; } = string.Empty;

    /// <summary>
    /// ID da entidade afetada
    /// </summary>
    public Guid EntityId { get; init; }

    /// <summary>
    /// Dados serializados da ação (JSON com dados relevantes)
    /// </summary>
    public string Data { get; init; } = string.Empty;
}