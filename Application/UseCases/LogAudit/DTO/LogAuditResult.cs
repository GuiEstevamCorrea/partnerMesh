namespace Application.UseCases.LogAudit.DTO;

public record LogAuditResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public LogAuditData? Data { get; init; }

    public static LogAuditResult Success(LogAuditData data) 
        => new() { IsSuccess = true, Message = "Log registrado com sucesso", Data = data };

    public static LogAuditResult Failure(string message) 
        => new() { IsSuccess = false, Message = message };
}

public record LogAuditData
{
    /// <summary>
    /// ID do log de auditoria criado
    /// </summary>
    public Guid LogId { get; init; }

    /// <summary>
    /// Data e hora que o log foi registrado
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Usuário que executou a ação
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Ação executada
    /// </summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>
    /// Entidade afetada
    /// </summary>
    public string Entity { get; init; } = string.Empty;

    /// <summary>
    /// ID da entidade afetada
    /// </summary>
    public Guid EntityId { get; init; }
}

/// <summary>
/// Constantes para ações de auditoria
/// </summary>
public static class AuditActions
{
    // Autenticação
    public const string LOGIN = "LOGIN";
    public const string LOGOUT = "LOGOUT";
    public const string REFRESH_TOKEN = "REFRESH_TOKEN";
    public const string PASSWORD_CHANGE = "PASSWORD_CHANGE";

    // CRUD genérico
    public const string CREATE = "CREATE";
    public const string UPDATE = "UPDATE";
    public const string DELETE = "DELETE";
    public const string ACTIVATE = "ACTIVATE";
    public const string DEACTIVATE = "DEACTIVATE";

    // Negócios e Comissões
    public const string BUSINESS_CREATE = "BUSINESS_CREATE";
    public const string BUSINESS_UPDATE = "BUSINESS_UPDATE";
    public const string BUSINESS_CANCEL = "BUSINESS_CANCEL";
    public const string COMMISSION_PAYMENT = "COMMISSION_PAYMENT";

    // Relatórios (ações críticas de consulta)
    public const string REPORT_PARTNERS = "REPORT_PARTNERS";
    public const string REPORT_FINANCIAL = "REPORT_FINANCIAL";
    public const string REPORT_BUSINESS = "REPORT_BUSINESS";

    // Acesso a dados sensíveis
    public const string VIEW_SENSITIVE_DATA = "VIEW_SENSITIVE_DATA";
    public const string EXPORT_DATA = "EXPORT_DATA";
}

/// <summary>
/// Constantes para entidades
/// </summary>
public static class AuditEntities
{
    public const string USER = "User";
    public const string VETOR = "Vetor";
    public const string PARTNER = "Partner";
    public const string BUSINESS_TYPE = "BusinessType";
    public const string BUSINESS = "Business";
    public const string COMMISSION = "Commission";
    public const string COMMISSION_PAYMENT = "CommissionPayment";
    public const string SYSTEM = "System";
    public const string REPORT = "Report";
}