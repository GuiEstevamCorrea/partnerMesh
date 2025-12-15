namespace Domain.Extensions;

/// <summary>
/// Métodos de extensão para AuditEntityType
/// </summary>
public static class AuditEntityTypeExtensions
{
    /// <summary>
    /// Converte enum para string no formato legado (PascalCase)
    /// </summary>
    public static string ToLegacyString(this ValueTypes.AuditEntityType entityType)
    {
        return entityType switch
        {
            ValueTypes.AuditEntityType.User => "User",
            ValueTypes.AuditEntityType.Vetor => "Vetor",
            ValueTypes.AuditEntityType.Partner => "Partner",
            ValueTypes.AuditEntityType.BusinessType => "BusinessType",
            ValueTypes.AuditEntityType.Business => "Business",
            ValueTypes.AuditEntityType.Commission => "Commission",
            ValueTypes.AuditEntityType.CommissionPayment => "CommissionPayment",
            ValueTypes.AuditEntityType.System => "System",
            ValueTypes.AuditEntityType.Report => "Report",
            _ => throw new ArgumentException($"Tipo de entidade inválido: {entityType}")
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static ValueTypes.AuditEntityType FromLegacyString(string entityType)
    {
        return entityType switch
        {
            "User" => ValueTypes.AuditEntityType.User,
            "Vetor" => ValueTypes.AuditEntityType.Vetor,
            "Partner" => ValueTypes.AuditEntityType.Partner,
            "BusinessType" => ValueTypes.AuditEntityType.BusinessType,
            "Business" => ValueTypes.AuditEntityType.Business,
            "Commission" => ValueTypes.AuditEntityType.Commission,
            "CommissionPayment" => ValueTypes.AuditEntityType.CommissionPayment,
            "System" => ValueTypes.AuditEntityType.System,
            "Report" => ValueTypes.AuditEntityType.Report,
            _ => throw new ArgumentException($"Tipo de entidade inválido: {entityType}")
        };
    }

    /// <summary>
    /// Tenta converter string para enum, retorna false se falhar
    /// </summary>
    public static bool TryParse(string entityType, out ValueTypes.AuditEntityType result)
    {
        result = ValueTypes.AuditEntityType.System;
        
        try
        {
            result = FromLegacyString(entityType);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
