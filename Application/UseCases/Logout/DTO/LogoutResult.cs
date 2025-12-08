namespace Application.UseCases.Logout.DTO;

public sealed record LogoutResult(
    bool IsSuccess,
    string Message)
{
    public static LogoutResult Success() =>
        new(true, "Logout realizado com sucesso.");

    public static LogoutResult Failure(string message) =>
        new(false, message);
}