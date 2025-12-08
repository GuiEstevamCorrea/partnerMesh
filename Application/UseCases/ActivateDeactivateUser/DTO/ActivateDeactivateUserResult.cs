namespace Application.UseCases.ActivateDeactivateUser.DTO;

public sealed record ActivateDeactivateUserResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public UserStatusInfo? User { get; init; }

    private ActivateDeactivateUserResult(bool isSuccess, string message, UserStatusInfo? user = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        User = user;
    }

    public static ActivateDeactivateUserResult Success(UserStatusInfo user, string action)
        => new(true, $"Usuário {action} com sucesso.", user);

    public static ActivateDeactivateUserResult Failure(string message)
        => new(false, message);
}

public sealed record UserStatusInfo(
    Guid Id,
    string Name,
    string Email,
    bool Active,
    string Permission,
    IEnumerable<Guid> VetorIds);