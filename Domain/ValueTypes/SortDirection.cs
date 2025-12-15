namespace Domain.ValueTypes;

/// <summary>
/// Define a direção de ordenação
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Ordenação ascendente (A-Z, 0-9, mais antigo primeiro)
    /// </summary>
    Ascending = 1,

    /// <summary>
    /// Ordenação descendente (Z-A, 9-0, mais recente primeiro)
    /// </summary>
    Descending = 2
}
