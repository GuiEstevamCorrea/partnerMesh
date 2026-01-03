using Api.Authorization;
using Application.Interfaces.IUseCases;
using Application.UseCases.CreateUser.DTO;
using Application.UseCases.UpdateUser.DTO;
using Application.UseCases.ChangePassword.DTO;
using Application.UseCases.ActivateDeactivateUser.DTO;
using Application.UseCases.ListUsers.DTO;
using Application.UseCases.GetUserById.DTO;
using Application.UseCases.LogAudit.DTO;
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
    private readonly IChangePasswordUseCase _changePasswordUseCase;
    private readonly IActivateDeactivateUserUseCase _activateDeactivateUserUseCase;
    private readonly IListUsersUseCase _listUsersUseCase;
    private readonly IGetUserByIdUseCase _getUserByIdUseCase;
    private readonly ILogAuditUseCase _logAuditUseCase;

    public UsersController(
        ICreateUserUseCase createUserUseCase,
        IUpdateUserUseCase updateUserUseCase,
        IChangePasswordUseCase changePasswordUseCase,
        IActivateDeactivateUserUseCase activateDeactivateUserUseCase,
        IListUsersUseCase listUsersUseCase,
        IGetUserByIdUseCase getUserByIdUseCase,
        ILogAuditUseCase logAuditUseCase)
    {
        _createUserUseCase = createUserUseCase;
        _updateUserUseCase = updateUserUseCase;
        _changePasswordUseCase = changePasswordUseCase;
        _activateDeactivateUserUseCase = activateDeactivateUserUseCase;
        _listUsersUseCase = listUsersUseCase;
        _getUserByIdUseCase = getUserByIdUseCase;
        _logAuditUseCase = logAuditUseCase;
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

        // Registrar auditoria
        _ = _logAuditUseCase.ExecuteAsync(new LogAuditRequest
        {
            UserId = currentUserId,
            Action = "Create",
            Entity = "User",
            EntityId = result.User!.Id,
            Data = $"Usuário criado: {result.User.Name} ({result.User.Email})"
        }, cancellationToken);

        return CreatedAtAction(
            nameof(GetUser), 
            new { id = result.User!.Id }, 
            result);
    }

    /// <summary>
    /// Lista usuários com filtros e paginação
    /// </summary>
    /// <param name="name">Filtro por nome (contém)</param>
    /// <param name="email">Filtro por email (contém)</param>
    /// <param name="permission">Filtro por permissão</param>
    /// <param name="vetorId">Filtro por vetor</param>
    /// <param name="active">Filtro por status ativo/inativo</param>
    /// <param name="page">Página (padrão: 1)</param>
    /// <param name="pageSize">Itens por página (padrão: 10, máximo: 100)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de usuários</returns>
    [HttpGet]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(typeof(ListUsersResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ListUsersResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListUsers(
        [FromQuery] string? name = null,
        [FromQuery] string? email = null,
        [FromQuery] int? permission = null,
        [FromQuery] Guid? vetorId = null,
        [FromQuery] bool? active = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(ListUsersResult.Failure("Usuário atual não identificado."));
        }

        // Converter permission de int para enum se fornecido
        Domain.ValueTypes.PermissionEnum? permissionEnum = null;
        if (permission.HasValue && Enum.IsDefined(typeof(Domain.ValueTypes.PermissionEnum), permission.Value))
        {
            permissionEnum = (Domain.ValueTypes.PermissionEnum)permission.Value;
        }

        var request = new ListUsersRequest(
            name,
            email,
            permissionEnum,
            vetorId,
            active,
            page,
            pageSize);

        var result = await _listUsersUseCase.ListAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtém dados detalhados de um usuário por ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações detalhadas do usuário</returns>
    [HttpGet("{id}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(typeof(GetUserByIdResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GetUserByIdResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetUserByIdResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(GetUserByIdResult.Failure("Usuário atual não identificado."));
        }

        var result = await _getUserByIdUseCase.GetAsync(id, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Message == "Usuário não encontrado.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
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

        // Registrar auditoria
        _ = _logAuditUseCase.ExecuteAsync(new LogAuditRequest
        {
            UserId = currentUserId,
            Action = "Update",
            Entity = "User",
            EntityId = id,
            Data = $"Usuário atualizado"
        }, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Permite ao usuário alterar sua própria senha
    /// </summary>
    /// <param name="request">Dados para alteração de senha</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpPatch("change-password")]
    [ProducesResponseType(typeof(ChangePasswordResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ChangePasswordResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangeOwnPassword([FromBody] ChangeOwnPasswordRequest request, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(ChangePasswordResult.Failure("Usuário atual não identificado."));
        }

        var result = await _changePasswordUseCase.ChangeOwnPasswordAsync(currentUserId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        // Registrar auditoria
        _ = _logAuditUseCase.ExecuteAsync(new LogAuditRequest
        {
            UserId = currentUserId,
            Action = "Update",
            Entity = "User",
            EntityId = currentUserId,
            Data = "Senha alterada"
        }, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Permite ao administrador global resetar a senha de qualquer usuário
    /// </summary>
    /// <param name="id">ID do usuário que terá a senha resetada</param>
    /// <param name="request">Nova senha</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpPatch("{id}/reset-password")]
    [AuthorizePermission("AdminGlobal")]
    [ProducesResponseType(typeof(ChangePasswordResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ChangePasswordResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual (admin) do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var adminUserId))
        {
            return BadRequest(ChangePasswordResult.Failure("Administrador não identificado."));
        }

        var result = await _changePasswordUseCase.ResetPasswordAsync(id, request, adminUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        // Registrar auditoria
        _ = _logAuditUseCase.ExecuteAsync(new LogAuditRequest
        {
            UserId = adminUserId,
            Action = "Update",
            Entity = "User",
            EntityId = id,
            Data = "Senha resetada pelo administrador"
        }, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Ativa um usuário inativo
    /// </summary>
    /// <param name="id">ID do usuário a ser ativado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpPatch("{id}/activate")]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(typeof(ActivateDeactivateUserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActivateDeactivateUserResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(Guid id, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(ActivateDeactivateUserResult.Failure("Usuário atual não identificado."));
        }

        var result = await _activateDeactivateUserUseCase.ActivateUserAsync(id, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        // Registrar auditoria
        _ = _logAuditUseCase.ExecuteAsync(new LogAuditRequest
        {
            UserId = currentUserId,
            Action = "Update",
            Entity = "User",
            EntityId = id,
            Data = "Usuário ativado"
        }, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Desativa um usuário ativo (respeitando a regra: vetor deve ter ao menos 1 admin)
    /// </summary>
    /// <param name="id">ID do usuário a ser desativado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpPatch("{id}/deactivate")]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(typeof(ActivateDeactivateUserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActivateDeactivateUserResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(ActivateDeactivateUserResult.Failure("Usuário atual não identificado."));
        }

        var result = await _activateDeactivateUserUseCase.DeactivateUserAsync(id, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}