using Api.Authorization;
using Application.Interfaces.IUseCases;
using Application.UseCases.CreateBusinessType.DTO;
using Application.UseCases.UpdateBusinessType.DTO;
using Application.UseCases.DeactivateBusinessType.DTO;
using Application.UseCases.ListBusinessTypes.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/business-types")]
[Authorize]
public class BusinessTypesController : ControllerBase
{
    private readonly ICreateBusinessTypeUseCase _createBusinessTypeUseCase;
    private readonly IUpdateBusinessTypeUseCase _updateBusinessTypeUseCase;
    private readonly IDeactivateBusinessTypeUseCase _deactivateBusinessTypeUseCase;
    private readonly IListBusinessTypesUseCase _listBusinessTypesUseCase;

    public BusinessTypesController(
        ICreateBusinessTypeUseCase createBusinessTypeUseCase,
        IUpdateBusinessTypeUseCase updateBusinessTypeUseCase,
        IDeactivateBusinessTypeUseCase deactivateBusinessTypeUseCase,
        IListBusinessTypesUseCase listBusinessTypesUseCase)
    {
        _createBusinessTypeUseCase = createBusinessTypeUseCase;
        _updateBusinessTypeUseCase = updateBusinessTypeUseCase;
        _deactivateBusinessTypeUseCase = deactivateBusinessTypeUseCase;
        _listBusinessTypesUseCase = listBusinessTypesUseCase;
    }

    /// <summary>
    /// UC-40: Criar Tipo de Negócio
    /// </summary>
    /// <param name="request">Dados do tipo de negócio a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do tipo de negócio criado</returns>
    [HttpPost]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(typeof(CreateBusinessTypeResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateBusinessTypeResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateBusinessType([FromBody] CreateBusinessTypeRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return BadRequest(CreateBusinessTypeResult.Failure("Request body is required"));

        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(CreateBusinessTypeResult.Failure("Usuário atual não identificado."));
        }

        var result = await _createBusinessTypeUseCase.CreateAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetBusinessTypeById),
            new { id = result.BusinessType!.Id },
            result);
    }

    /// <summary>
    /// UC-43: Listar Tipos de Negócio
    /// </summary>
    /// <param name="request">Filtros e parâmetros de paginação</param>
    /// <returns>Lista de tipos de negócio com paginação</returns>
    [HttpGet]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(typeof(ListBusinessTypesResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListBusinessTypes([FromQuery] ListBusinessTypesRequest request)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(new { message = "Usuário atual não identificado." });
        }

        var result = await _listBusinessTypesUseCase.ExecuteAsync(request, currentUserId);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// UC-41: Atualizar Tipo de Negócio
    /// </summary>
    /// <param name="id">ID do tipo de negócio</param>
    /// <param name="request">Dados para atualização do tipo de negócio</param>
    /// <returns>Informações do tipo de negócio atualizado</returns>
    [HttpPut("{id:guid}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(typeof(UpdateBusinessTypeResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateBusinessType(Guid id, [FromBody] UpdateBusinessTypeRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required" });

        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(new { message = "Usuário atual não identificado." });
        }

        try
        {
            var result = await _updateBusinessTypeUseCase.ExecuteAsync(id, request, currentUserId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// UC-42: Remover/Inativar Tipo de Negócio
    /// </summary>
    /// <param name="id">ID do tipo de negócio</param>
    /// <returns>Informações do tipo de negócio inativado</returns>
    [HttpDelete("{id:guid}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(typeof(DeactivateBusinessTypeResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateBusinessType(Guid id)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(new { message = "Usuário atual não identificado." });
        }

        try
        {
            var result = await _deactivateBusinessTypeUseCase.ExecuteAsync(id, currentUserId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno do servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Placeholder para obter tipo de negócio por ID
    /// </summary>
    /// <param name="id">ID do tipo de negócio</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do tipo de negócio</returns>
    [HttpGet("{id:guid}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetBusinessTypeById(Guid id, CancellationToken cancellationToken = default)
    {
        // TODO: Implementar UC-44 (Obter Tipo por ID)
        return Ok(new { message = "UC-44 não implementado ainda.", businessTypeId = id });
    }
}