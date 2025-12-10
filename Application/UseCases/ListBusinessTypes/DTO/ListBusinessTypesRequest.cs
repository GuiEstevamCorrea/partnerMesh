namespace Application.UseCases.ListBusinessTypes.DTO;

public sealed record ListBusinessTypesRequest
{
    /// <summary>
    /// Filtro por nome do tipo de negócio
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Filtro por descrição
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Filtro por status ativo/inativo
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Número da página (padrão: 1)
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Itens por página (padrão: 20, máximo: 100)
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Campo para ordenação (Name, Description, CreatedAt, etc.)
    /// </summary>
    public string? OrderBy { get; init; }

    /// <summary>
    /// Direção da ordenação (asc, desc)
    /// </summary>
    public string? OrderDirection { get; init; } = "asc";
}