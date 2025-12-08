namespace Application.UseCases.UpdateUser.DTO;

public sealed record UpdateUserResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public UserInfo? User { get; init; }

    private UpdateUserResult(bool isSuccess, string message, UserInfo? user = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        User = user;
    }

    public static UpdateUserResult Success(UserInfo user)
        => new(true, "Usuário atualizado com sucesso.", user);

    public static UpdateUserResult Failure(string message)
        => new(false, message);
}

public sealed record UserInfo(
    Guid Id,
    string Name,
    string Email,
    string Permission,
    bool Active,
    DateTime CreatedAt,
    IEnumerable<Guid> VetorIds);