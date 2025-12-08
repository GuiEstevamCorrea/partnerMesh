using Application.Interfaces.IUseCases;
using Application.UseCases.AuthenticateUser.DTO;
using Application.UseCases.RefreshToken.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticateUserUseCase _authenticateUserUseCase;
    private readonly IRefreshTokenUseCase _refreshTokenUseCase;

    public AuthController(
        IAuthenticateUserUseCase authenticateUserUseCase,
        IRefreshTokenUseCase refreshTokenUseCase)
    {
        _authenticateUserUseCase = authenticateUserUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
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
}
