namespace Domain.Extensions;

/// <summary>
/// Métodos de extensão para PaymentType
/// </summary>
public static class PaymentTypeExtensions
{
    /// <summary>
    /// Converte enum para string no formato legado
    /// </summary>
    public static string ToLegacyString(this ValueTypes.PaymentType type)
    {
        return type switch
        {
            ValueTypes.PaymentType.Vetor => "vetor",
            ValueTypes.PaymentType.Recomendador => "recomendador",
            ValueTypes.PaymentType.Participante => "participante",
            ValueTypes.PaymentType.Intermediario => "intermediario",
            _ => throw new ArgumentException($"Tipo de pagamento inválido: {type}")
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static ValueTypes.PaymentType FromLegacyString(string type)
    {
        return type?.ToLower() switch
        {
            "vetor" => ValueTypes.PaymentType.Vetor,
            "recomendador" => ValueTypes.PaymentType.Recomendador,
            "participante" => ValueTypes.PaymentType.Participante,
            "intermediario" => ValueTypes.PaymentType.Intermediario,
            _ => throw new ArgumentException($"Tipo de pagamento inválido: {type}")
        };
    }

    /// <summary>
    /// Tenta converter string para enum, retorna false se falhar
    /// </summary>
    public static bool TryParse(string type, out ValueTypes.PaymentType result)
    {
        result = ValueTypes.PaymentType.Vetor;
        
        switch (type?.ToLower())
        {
            case "vetor":
                result = ValueTypes.PaymentType.Vetor;
                return true;
            case "recomendador":
                result = ValueTypes.PaymentType.Recomendador;
                return true;
            case "participante":
                result = ValueTypes.PaymentType.Participante;
                return true;
            case "intermediario":
                result = ValueTypes.PaymentType.Intermediario;
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Retorna descrição amigável do tipo de pagamento
    /// </summary>
    public static string ToFriendlyName(this ValueTypes.PaymentType type)
    {
        return type switch
        {
            ValueTypes.PaymentType.Vetor => "Vetor",
            ValueTypes.PaymentType.Recomendador => "Nível 1",
            ValueTypes.PaymentType.Participante => "Você",
            ValueTypes.PaymentType.Intermediario => "Intermediário",
            _ => "Desconhecido"
        };
    }
}
