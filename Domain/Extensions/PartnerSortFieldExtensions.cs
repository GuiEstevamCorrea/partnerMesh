namespace Domain.Extensions;

using Domain.ValueTypes;

public static class PartnerSortFieldExtensions
{
    /// <summary>
    /// Converte o enum para string no formato legado
    /// </summary>
    public static string ToLegacyString(this PartnerSortField field)
    {
        return field switch
        {
            PartnerSortField.Name => "name",
            PartnerSortField.CreatedAt => "createdat",
            PartnerSortField.Email => "email",
            PartnerSortField.Active => "active",
            _ => "name"
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static PartnerSortField FromLegacyString(string field)
    {
        return field?.ToLower() switch
        {
            "name" => PartnerSortField.Name,
            "createdat" => PartnerSortField.CreatedAt,
            "email" => PartnerSortField.Email,
            "active" => PartnerSortField.Active,
            _ => PartnerSortField.Name
        };
    }

    /// <summary>
    /// Tenta fazer parse de uma string para PartnerSortField
    /// </summary>
    public static bool TryParse(string? value, out PartnerSortField field)
    {
        field = PartnerSortField.Name;
        
        if (string.IsNullOrWhiteSpace(value))
            return false;

        field = FromLegacyString(value);
        return true;
    }
}
