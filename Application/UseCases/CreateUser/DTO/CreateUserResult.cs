using Application.UseCases.AuthenticateUser.DTO;

namespace Application.UseCases.CreateUser.DTO;

public sealed record CreateUserResult(
    bool IsSuccess,
    string? Message,
    UserInfo? User)
{
    public static CreateUserResult Success(UserInfo user) =>
        new(true, "Usuário criado com sucesso.", user);

    public static CreateUserResult Failure(string message) =>
        new(false, message, null);
}