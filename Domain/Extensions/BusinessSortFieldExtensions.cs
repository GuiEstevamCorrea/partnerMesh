namespace Domain.Extensions;

using Domain.ValueTypes;

public static class BusinessSortFieldExtensions
{
    /// <summary>
    /// Converte o enum para string no formato legado
    /// </summary>
    public static string ToLegacyString(this BusinessSortField field)
    {
        return field switch
        {
            BusinessSortField.Date => "date",
            BusinessSortField.Value => "value",
            BusinessSortField.Partner => "partner",
            BusinessSortField.BusinessType => "businesstype",
            BusinessSortField.Status => "status",
            _ => "date"
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static BusinessSortField FromLegacyString(string field)
    {
        return field?.ToLower() switch
        {
            "date" => BusinessSortField.Date,
            "value" => BusinessSortField.Value,
            "partner" => BusinessSortField.Partner,
            "businesstype" => BusinessSortField.BusinessType,
            "status" => BusinessSortField.Status,
            _ => BusinessSortField.Date
        };
    }

    /// <summary>
    /// Tenta fazer parse de uma string para BusinessSortField
    /// </summary>
    public static bool TryParse(string? value, out BusinessSortField field)
    {
        field = BusinessSortField.Date;
        
        if (string.IsNullOrWhiteSpace(value))
            return false;

        field = FromLegacyString(value);
        return true;
    }
}
