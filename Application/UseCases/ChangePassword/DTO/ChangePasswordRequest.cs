namespace Application.UseCases.ChangePassword.DTO;

/// <summary>
/// Request para alteração da própria senha pelo usuário
/// </summary>
public sealed record ChangeOwnPasswordRequest(
    string CurrentPassword,
    string NewPassword);

/// <summary>
/// Request para reset de senha por administrador
/// </summary>
public sealed record ResetPasswordRequest(
    string NewPassword);