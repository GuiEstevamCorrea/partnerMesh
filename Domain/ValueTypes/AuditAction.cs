namespace Domain.ValueTypes;

/// <summary>
/// Define as ações de auditoria do sistema
/// </summary>
public enum AuditAction
{
    // Autenticação
    Login = 1,
    Logout = 2,
    RefreshToken = 3,
    PasswordChange = 4,

    // CRUD genérico
    Create = 10,
    Update = 11,
    Delete = 12,
    Activate = 13,
    Deactivate = 14,

    // Negócios e Comissões
    BusinessCreate = 20,
    BusinessUpdate = 21,
    BusinessCancel = 22,
    CommissionPayment = 23,

    // Relatórios (ações críticas de consulta)
    ReportPartners = 30,
    ReportFinancial = 31,
    ReportBusiness = 32,

    // Acesso a dados sensíveis
    ViewSensitiveData = 40,
    ExportData = 41
}
