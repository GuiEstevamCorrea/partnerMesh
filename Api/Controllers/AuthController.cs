using Application.Interfaces.IUseCases;
using Application.UseCases.AuthenticateUser.DTO;
using Application.UseCases.RefreshToken.DTO;
using Application.UseCases.Logout.DTO;
using Application.UseCases.LogAudit.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticateUserUseCase _authenticateUserUseCase;
    private readonly IRefreshTokenUseCase _refreshTokenUseCase;
    private readonly ILogoutUseCase _logoutUseCase;
    private readonly ILogAuditUseCase _logAuditUseCase;

    public AuthController(
        IAuthenticateUserUseCase authenticateUserUseCase,
        IRefreshTokenUseCase refreshTokenUseCase,
        ILogoutUseCase logoutUseCase,
        ILogAuditUseCase logAuditUseCase)
    {
        _authenticateUserUseCase = authenticateUserUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
        _logAuditUseCase = logAuditUseCase;
    }

    /// <summary>
    /// Autentica um usuário no sistema
    /// </summary>
    /// <param name="request">Dados de login (email e senha)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Token JWT e informações do usuário autenticado</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authenticateUserUseCase.AuthenticateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        // Registrar auditoria de login
        if (result.User != null)
        {
            _ = _logAuditUseCase.ExecuteAsync(new LogAuditRequest
            {
                UserId = result.User.Id,
                Action = "Login",
                Entity = "User",
                EntityId = result.User.Id,
                Data = $"Login realizado: {result.User.Email}"
            }, cancellationToken);
        }

        return Ok(result);
    }

    /// <summary>
    /// Renova um token JWT usando um refresh token válido
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Novo token JWT e refresh token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshTokenResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RefreshTokenResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RefreshTokenResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _refreshTokenUseCase.RefreshAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Faz logout do usuário revogando o refresh token
    /// </summary>
    /// <param name="request">Refresh token a ser revogado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado do logout</returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(LogoutResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(LogoutResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken = default)
    {
        // Tentar obter o userId do token JWT (se estiver autenticado)
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        var result = await _logoutUseCase.LogoutAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        // Registrar auditoria de logout se conseguiu identificar o usuário
        if (userId.HasValue)
        {
            _ = _logAuditUseCase.ExecuteAsync(new LogAuditRequest
            {
                UserId = userId.Value,
                Action = "Logout",
                Entity = "User",
                EntityId = userId.Value,
                Data = "Logout realizado"
            }, cancellationToken);
        }

        return Ok(result);
    }
}
