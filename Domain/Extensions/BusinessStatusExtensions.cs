namespace Domain.Extensions;

/// <summary>
/// Métodos de extensão para BusinessStatus
/// </summary>
public static class BusinessStatusExtensions
{
    /// <summary>
    /// Converte enum para string no formato legado
    /// </summary>
    public static string ToLegacyString(this ValueTypes.BusinessStatus status)
    {
        return status switch
        {
            ValueTypes.BusinessStatus.Ativo => "Active",
            ValueTypes.BusinessStatus.Cancelado => "Cancelled",
            ValueTypes.BusinessStatus.ParcialmentePago => "PartiallyPaid",
            ValueTypes.BusinessStatus.TotalmentePago => "FullyPaid",
            _ => throw new ArgumentException($"Status inválido: {status}")
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static ValueTypes.BusinessStatus FromLegacyString(string status)
    {
        return status?.ToLower() switch
        {
            "active" => ValueTypes.BusinessStatus.Ativo,
            "ativo" => ValueTypes.BusinessStatus.Ativo,
            "cancelled" => ValueTypes.BusinessStatus.Cancelado,
            "cancelado" => ValueTypes.BusinessStatus.Cancelado,
            "partiallypaid" => ValueTypes.BusinessStatus.ParcialmentePago,
            "parcialmentepago" => ValueTypes.BusinessStatus.ParcialmentePago,
            "fullypaid" => ValueTypes.BusinessStatus.TotalmentePago,
            "totalmentepago" => ValueTypes.BusinessStatus.TotalmentePago,
            _ => throw new ArgumentException($"Status inválido: {status}")
        };
    }

    /// <summary>
    /// Tenta converter string para enum, retorna false se falhar
    /// </summary>
    public static bool TryParse(string status, out ValueTypes.BusinessStatus result)
    {
        result = ValueTypes.BusinessStatus.Ativo;
        
        switch (status?.ToLower())
        {
            case "ativo":
            case "active":
                result = ValueTypes.BusinessStatus.Ativo;
                return true;
            case "cancelado":
            case "cancelled":
                result = ValueTypes.BusinessStatus.Cancelado;
                return true;
            case "parcialmentepago":
            case "partiallypaid":
                result = ValueTypes.BusinessStatus.ParcialmentePago;
                return true;
            case "totalmentepago":
            case "fullypaid":
                result = ValueTypes.BusinessStatus.TotalmentePago;
                return true;
            default:
                return false;
        }
    }
}
