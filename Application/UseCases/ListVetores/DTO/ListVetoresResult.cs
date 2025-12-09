namespace Application.UseCases.ListVetores.DTO;

public sealed record ListVetoresResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public PagedVetores? Data { get; init; }

    private ListVetoresResult(bool isSuccess, string message, PagedVetores? data = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }

    public static ListVetoresResult Success(PagedVetores data)
        => new(true, "Vetores listados com sucesso.", data);

    public static ListVetoresResult Failure(string message)
        => new(false, message);
}

public sealed record PagedVetores(
    IEnumerable<VetorListItem> Vetores,
    int CurrentPage,
    int PageSize,
    int TotalVetores,
    int TotalPages)
{
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}

public sealed record VetorListItem(
    Guid Id,
    string Name,
    string Email,
    bool Active,
    DateTime CreatedAt,
    int TotalUsuarios,
    int UsuariosAtivos,
    int TotalParceiros,
    int ParceirosAtivos);