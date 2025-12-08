using Api.Authorization;
using Application.Interfaces.IUseCases;
using Application.UseCases.CreateUser.DTO;
using Application.UseCases.UpdateUser.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ICreateUserUseCase _createUserUseCase;
    private readonly IUpdateUserUseCase _updateUserUseCase;

    public UsersController(
        ICreateUserUseCase createUserUseCase,
        IUpdateUserUseCase updateUserUseCase)
    {
        _createUserUseCase = createUserUseCase;
        _updateUserUseCase = updateUserUseCase;
    }

    /// <summary>
    /// Cria um novo usuário no sistema
    /// </summary>
    /// <param name="request">Dados do usuário a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do usuário criado</returns>
    [HttpPost]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateUserResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(CreateUserResult.Failure("Usuário atual não identificado."));
        }

        var result = await _createUserUseCase.CreateAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetUser), 
            new { id = result.User!.Id }, 
            result);
    }

    /// <summary>
    /// Placeholder para obter usuário por ID (será implementado em UC-15)
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Informações do usuário</returns>
    [HttpGet("{id}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUser(Guid id)
    {
        // Será implementado no UC-15
        return Ok(new { message = "Endpoint será implementado no UC-15", userId = id });
    }

    /// <summary>
    /// Atualiza um usuário existente no sistema
    /// </summary>
    /// <param name="id">ID do usuário a ser atualizado</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do usuário atualizado</returns>
    [HttpPut("{id}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(typeof(UpdateUserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UpdateUserResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(UpdateUserResult.Failure("Usuário atual não identificado."));
        }

        var result = await _updateUserUseCase.UpdateAsync(id, request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}