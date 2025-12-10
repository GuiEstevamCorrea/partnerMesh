namespace Application.UseCases.ListPartners.DTO;

public sealed record ListPartnersRequest
{
    /// <summary>
    /// Filtro por nome do parceiro
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Filtro por email do parceiro
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Filtro por documento (CPF/CNPJ)
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Filtro por status ativo/inativo
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Filtro por ID do vetor
    /// </summary>
    public Guid? VetorId { get; init; }

    /// <summary>
    /// Filtro por ID do parceiro recomendante
    /// </summary>
    public Guid? RecommenderId { get; init; }

    /// <summary>
    /// Número da página (padrão: 1)
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Itens por página (padrão: 20, máximo: 100)
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Campo para ordenação (Name, Email, CreatedAt, etc.)
    /// </summary>
    public string? OrderBy { get; init; }

    /// <summary>
    /// Direção da ordenação (asc, desc)
    /// </summary>
    public string? OrderDirection { get; init; } = "asc";
}