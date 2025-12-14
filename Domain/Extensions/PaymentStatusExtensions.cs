namespace Domain.Extensions;

/// <summary>
/// Métodos de extensão para PaymentStatus
/// </summary>
public static class PaymentStatusExtensions
{
    /// <summary>
    /// Converte enum para string no formato legado
    /// </summary>
    public static string ToLegacyString(this ValueTypes.PaymentStatus status)
    {
        return status switch
        {
            ValueTypes.PaymentStatus.APagar => "a_pagar",
            ValueTypes.PaymentStatus.Pago => "pago",
            ValueTypes.PaymentStatus.Cancelado => "cancelado",
            _ => throw new ArgumentException($"Status inválido: {status}")
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static ValueTypes.PaymentStatus FromLegacyString(string status)
    {
        return status?.ToLower() switch
        {
            "a_pagar" => ValueTypes.PaymentStatus.APagar,
            "pago" => ValueTypes.PaymentStatus.Pago,
            "cancelado" => ValueTypes.PaymentStatus.Cancelado,
            _ => throw new ArgumentException($"Status inválido: {status}")
        };
    }

    /// <summary>
    /// Tenta converter string para enum, retorna false se falhar
    /// </summary>
    public static bool TryParse(string status, out ValueTypes.PaymentStatus result)
    {
        result = ValueTypes.PaymentStatus.APagar;
        
        switch (status?.ToLower())
        {
            case "a_pagar":
                result = ValueTypes.PaymentStatus.APagar;
                return true;
            case "pago":
                result = ValueTypes.PaymentStatus.Pago;
                return true;
            case "cancelado":
                result = ValueTypes.PaymentStatus.Cancelado;
                return true;
            default:
                return false;
        }
    }
}
