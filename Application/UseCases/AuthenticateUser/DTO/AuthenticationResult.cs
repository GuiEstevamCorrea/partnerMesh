using Domain.ValueTypes;

namespace Application.UseCases.AuthenticateUser.DTO;

public sealed record AuthenticationResult(
    bool IsSuccess,
    string? Token,
    string? RefreshToken,
    string? Message,
    UserInfo? User)
{
    public static AuthenticationResult Success(string token, string refreshToken, UserInfo user) =>
        new(true, token, refreshToken, null, user);

    public static AuthenticationResult Failure(string message) =>
        new(false, null, null, message, null);
}

public sealed record UserInfo(
    Guid Id,
    string Name,
    string Email,
    PermissionEnum Permission,
    List<Guid> VetorIds);