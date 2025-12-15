namespace Domain.Extensions;

/// <summary>
/// Métodos de extensão para AuditAction
/// </summary>
public static class AuditActionExtensions
{
    /// <summary>
    /// Converte enum para string no formato legado (maiúsculas com underscores)
    /// </summary>
    public static string ToLegacyString(this ValueTypes.AuditAction action)
    {
        return action switch
        {
            ValueTypes.AuditAction.Login => "LOGIN",
            ValueTypes.AuditAction.Logout => "LOGOUT",
            ValueTypes.AuditAction.RefreshToken => "REFRESH_TOKEN",
            ValueTypes.AuditAction.PasswordChange => "PASSWORD_CHANGE",
            ValueTypes.AuditAction.Create => "CREATE",
            ValueTypes.AuditAction.Update => "UPDATE",
            ValueTypes.AuditAction.Delete => "DELETE",
            ValueTypes.AuditAction.Activate => "ACTIVATE",
            ValueTypes.AuditAction.Deactivate => "DEACTIVATE",
            ValueTypes.AuditAction.BusinessCreate => "BUSINESS_CREATE",
            ValueTypes.AuditAction.BusinessUpdate => "BUSINESS_UPDATE",
            ValueTypes.AuditAction.BusinessCancel => "BUSINESS_CANCEL",
            ValueTypes.AuditAction.CommissionPayment => "COMMISSION_PAYMENT",
            ValueTypes.AuditAction.ReportPartners => "REPORT_PARTNERS",
            ValueTypes.AuditAction.ReportFinancial => "REPORT_FINANCIAL",
            ValueTypes.AuditAction.ReportBusiness => "REPORT_BUSINESS",
            ValueTypes.AuditAction.ViewSensitiveData => "VIEW_SENSITIVE_DATA",
            ValueTypes.AuditAction.ExportData => "EXPORT_DATA",
            _ => throw new ArgumentException($"Ação de auditoria inválida: {action}")
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static ValueTypes.AuditAction FromLegacyString(string action)
    {
        return action?.ToUpper() switch
        {
            "LOGIN" => ValueTypes.AuditAction.Login,
            "LOGOUT" => ValueTypes.AuditAction.Logout,
            "REFRESH_TOKEN" => ValueTypes.AuditAction.RefreshToken,
            "PASSWORD_CHANGE" => ValueTypes.AuditAction.PasswordChange,
            "CREATE" => ValueTypes.AuditAction.Create,
            "UPDATE" => ValueTypes.AuditAction.Update,
            "DELETE" => ValueTypes.AuditAction.Delete,
            "ACTIVATE" => ValueTypes.AuditAction.Activate,
            "DEACTIVATE" => ValueTypes.AuditAction.Deactivate,
            "BUSINESS_CREATE" => ValueTypes.AuditAction.BusinessCreate,
            "BUSINESS_UPDATE" => ValueTypes.AuditAction.BusinessUpdate,
            "BUSINESS_CANCEL" => ValueTypes.AuditAction.BusinessCancel,
            "COMMISSION_PAYMENT" => ValueTypes.AuditAction.CommissionPayment,
            "REPORT_PARTNERS" => ValueTypes.AuditAction.ReportPartners,
            "REPORT_FINANCIAL" => ValueTypes.AuditAction.ReportFinancial,
            "REPORT_BUSINESS" => ValueTypes.AuditAction.ReportBusiness,
            "VIEW_SENSITIVE_DATA" => ValueTypes.AuditAction.ViewSensitiveData,
            "EXPORT_DATA" => ValueTypes.AuditAction.ExportData,
            _ => throw new ArgumentException($"Ação de auditoria inválida: {action}")
        };
    }

    /// <summary>
    /// Tenta converter string para enum, retorna false se falhar
    /// </summary>
    public static bool TryParse(string action, out ValueTypes.AuditAction result)
    {
        result = ValueTypes.AuditAction.Create;
        
        try
        {
            result = FromLegacyString(action);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
