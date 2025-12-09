namespace Application.UseCases.ListVetores.DTO;

public sealed record ListVetoresRequest(
    string? Name = null,
    string? Email = null,
    bool? Active = null,
    int Page = 1,
    int PageSize = 10)
{
    public int Page { get; init; } = Math.Max(1, Page);
    public int PageSize { get; init; } = Math.Max(1, Math.Min(100, PageSize)); // Máximo 100 por página
}