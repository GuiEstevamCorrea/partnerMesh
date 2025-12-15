namespace Domain.Extensions;

using Domain.ValueTypes;

public static class PartnerReportSortFieldExtensions
{
    /// <summary>
    /// Converte o enum para string no formato legado
    /// </summary>
    public static string ToLegacyString(this PartnerReportSortField field)
    {
        return field switch
        {
            PartnerReportSortField.Name => "name",
            PartnerReportSortField.Level => "level",
            PartnerReportSortField.TotalReceived => "totalreceived",
            PartnerReportSortField.TotalPending => "totalpending",
            _ => "name"
        };
    }

    /// <summary>
    /// Converte string do formato legado para enum
    /// </summary>
    public static PartnerReportSortField FromLegacyString(string field)
    {
        return field?.ToLower() switch
        {
            "name" => PartnerReportSortField.Name,
            "level" => PartnerReportSortField.Level,
            "totalreceived" => PartnerReportSortField.TotalReceived,
            "totalpending" => PartnerReportSortField.TotalPending,
            _ => PartnerReportSortField.Name
        };
    }

    /// <summary>
    /// Tenta fazer parse de uma string para PartnerReportSortField
    /// </summary>
    public static bool TryParse(string? value, out PartnerReportSortField field)
    {
        field = PartnerReportSortField.Name;
        
        if (string.IsNullOrWhiteSpace(value))
            return false;

        field = FromLegacyString(value);
        return true;
    }
}
