using Application.UseCases.AuthenticateUser.DTO;

namespace Application.UseCases.RefreshToken.DTO;

public sealed record RefreshTokenResult(
    bool IsSuccess,
    string? Token,
    string? RefreshToken,
    string? Message,
    UserInfo? User)
{
    public static RefreshTokenResult Success(string token, string refreshToken, UserInfo user) =>
        new(true, token, refreshToken, null, user);

    public static RefreshTokenResult Failure(string message) =>
        new(false, null, null, message, null);
}