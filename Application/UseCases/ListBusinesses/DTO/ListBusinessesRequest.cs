using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.ListBusinesses.DTO;

public sealed record ListBusinessesRequest
{
    /// <summary>
    /// Filtro por ID do parceiro
    /// </summary>
    public Guid? PartnerId { get; init; }

    /// <summary>
    /// Filtro por ID do tipo de negócio
    /// </summary>
    public Guid? BusinessTypeId { get; init; }

    /// <summary>
    /// Filtro por status do negócio (ativo, cancelado)
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Filtro por valor mínimo
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Valor mínimo deve ser positivo")]
    public decimal? MinValue { get; init; }

    /// <summary>
    /// Filtro por valor máximo
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Valor máximo deve ser positivo")]
    public decimal? MaxValue { get; init; }

    /// <summary>
    /// Filtro por data inicial (formato: yyyy-MM-dd)
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Filtro por data final (formato: yyyy-MM-dd)
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Busca por observações (texto livre)
    /// </summary>
    [MaxLength(100, ErrorMessage = "Busca por observações deve ter no máximo 100 caracteres")]
    public string? SearchText { get; init; }

    /// <summary>
    /// Campo para ordenação (date, value, partner, businessType, status)
    /// </summary>
    public string? SortBy { get; init; } = "date";

    /// <summary>
    /// Direção da ordenação (asc, desc)
    /// </summary>
    public string? SortDirection { get; init; } = "desc";

    /// <summary>
    /// Número da página (começa em 1)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Número da página deve ser maior que 0")]
    public int Page { get; init; } = 1;

    /// <summary>
    /// Tamanho da página (máximo 100)
    /// </summary>
    [Range(1, 100, ErrorMessage = "Tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; init; } = 10;
}