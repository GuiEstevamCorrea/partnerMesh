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
            ValueTypes.BusinessStatus.Ativo => "ativo",
            ValueTypes.BusinessStatus.Cancelado => "cancelado",
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
            "ativo" => ValueTypes.BusinessStatus.Ativo,
            "cancelado" => ValueTypes.BusinessStatus.Cancelado,
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
                result = ValueTypes.BusinessStatus.Ativo;
                return true;
            case "cancelado":
                result = ValueTypes.BusinessStatus.Cancelado;
                return true;
            default:
                return false;
        }
    }
}
