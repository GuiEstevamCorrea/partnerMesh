using System.Text.Json.Serialization;

namespace Application.UseCases.ListUsers.DTO;

public sealed record ListUsersResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public PagedUsers? Data { get; init; }

    private ListUsersResult(bool isSuccess, string message, PagedUsers? data = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }

    public static ListUsersResult Success(PagedUsers data)
        => new(true, "Usuários listados com sucesso.", data);

    public static ListUsersResult Failure(string message)
        => new(false, message);
}

public sealed record PagedUsers(
    IEnumerable<UserListItem> Users,
    int CurrentPage,
    int PageSize,
    int TotalUsers,
    int TotalPages)
{
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}

public sealed record UserListItem(
    Guid Id,
    string Name,
    string Email,
    string Permission,
    [property: JsonPropertyName("isActive")] bool Active,
    DateTime CreatedAt,
    IEnumerable<VetorInfo> Vetores);

public sealed record VetorInfo(
    Guid Id,
    string Name);