namespace Domain.Extensions;

using Domain.ValueTypes;

public static class PaymentSortFieldExtensions
{
    /// <summary>
    /// Converte o enum para string no formato legado
    /// </summary>
    public static string ToLegacyString(this PaymentSortField field)
    {
        return field switch
        {
            PaymentSortField.CreatedAt => "createdat",
            PaymentSortField.Value => "value",
            PaymentSortField.Status => "status",
            PaymentSortField.PaidOn => "paidon",
            _ => "createdat"
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static PaymentSortField FromLegacyString(string field)
    {
        return field?.ToLower() switch
        {
            "createdat" => PaymentSortField.CreatedAt,
            "value" => PaymentSortField.Value,
            "status" => PaymentSortField.Status,
            "paidon" => PaymentSortField.PaidOn,
            _ => PaymentSortField.CreatedAt
        };
    }

    /// <summary>
    /// Tenta fazer parse de uma string para PaymentSortField
    /// </summary>
    public static bool TryParse(string? value, out PaymentSortField field)
    {
        field = PaymentSortField.CreatedAt;
        
        if (string.IsNullOrWhiteSpace(value))
            return false;

        field = FromLegacyString(value);
        return true;
    }
}
