namespace Application.UseCases.ChangePassword.DTO;

public sealed record ChangePasswordResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;

    private ChangePasswordResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static ChangePasswordResult Success()
        => new(true, "Senha alterada com sucesso.");

    public static ChangePasswordResult Failure(string message)
        => new(false, message);
}