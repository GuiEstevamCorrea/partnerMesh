using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Extensions;

/// <summary>
/// Extensões para Controllers para facilitar operações comuns
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Extrai o UserId do token JWT de forma consistente
    /// </summary>
    /// <param name="controller">Controller atual</param>
    /// <param name="userId">UserId extraído se sucesso</param>
    /// <returns>Resultado da operação de extração</returns>
    public static ExtractUserIdResult TryGetCurrentUserId(this ControllerBase controller, out Guid userId)
    {
        userId = Guid.Empty;

        var userIdClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return ExtractUserIdResult.Failure("Token JWT não contém claim de usuário.");
        }

        if (!Guid.TryParse(userIdClaim, out userId))
        {
            return ExtractUserIdResult.Failure("UserId no token JWT está em formato inválido.");
        }

        return ExtractUserIdResult.Success();
    }

    /// <summary>
    /// Extrai o UserId do token JWT e retorna ActionResult apropriado em caso de erro
    /// </summary>
    /// <param name="controller">Controller atual</param>
    /// <param name="userId">UserId extraído se sucesso</param>
    /// <returns>ActionResult de erro ou null se sucesso</returns>
    public static ActionResult? GetCurrentUserIdOrError(this ControllerBase controller, out Guid userId)
    {
        var result = controller.TryGetCurrentUserId(out userId);
        
        if (!result.IsSuccess)
        {
            return controller.BadRequest(new { message = result.ErrorMessage });
        }

        return null;
    }

    /// <summary>
    /// Verifica se o usuário atual é Admin Global
    /// </summary>
    /// <param name="controller">Controller atual</param>
    /// <returns>True se é Admin Global</returns>
    public static bool IsCurrentUserAdminGlobal(this ControllerBase controller)
    {
        var userRole = controller.User.FindFirst(ClaimTypes.Role)?.Value;
        return userRole == "AdminGlobal";
    }

    /// <summary>
    /// Verifica se o usuário atual tem uma das permissões especificadas
    /// </summary>
    /// <param name="controller">Controller atual</param>
    /// <param name="permissions">Permissões aceitas</param>
    /// <returns>True se tem permissão</returns>
    public static bool CurrentUserHasPermission(this ControllerBase controller, params string[] permissions)
    {
        var userRole = controller.User.FindFirst(ClaimTypes.Role)?.Value;
        return !string.IsNullOrEmpty(userRole) && permissions.Contains(userRole);
    }
}

/// <summary>
/// Resultado da operação de extração de UserId
/// </summary>
public record ExtractUserIdResult
{
    public bool IsSuccess { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;

    private ExtractUserIdResult(bool isSuccess, string errorMessage = "")
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static ExtractUserIdResult Success() => new(true);
    public static ExtractUserIdResult Failure(string errorMessage) => new(false, errorMessage);
}