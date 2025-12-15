namespace Domain.Extensions;

using Domain.ValueTypes;

public static class SortDirectionExtensions
{
    /// <summary>
    /// Converte o enum para string no formato legado ("asc"/"desc")
    /// </summary>
    public static string ToLegacyString(this SortDirection direction)
    {
        return direction switch
        {
            SortDirection.Ascending => "asc",
            SortDirection.Descending => "desc",
            _ => "asc"
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static SortDirection FromLegacyString(string direction)
    {
        return direction?.ToLower() switch
        {
            "desc" => SortDirection.Descending,
            "asc" => SortDirection.Ascending,
            _ => SortDirection.Ascending
        };
    }

    /// <summary>
    /// Tenta fazer parse de uma string para SortDirection
    /// </summary>
    public static bool TryParse(string? value, out SortDirection direction)
    {
        direction = SortDirection.Ascending;
        
        if (string.IsNullOrWhiteSpace(value))
            return false;

        direction = FromLegacyString(value);
        return true;
    }
}
