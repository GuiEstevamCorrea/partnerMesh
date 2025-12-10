using Api.Authorization;
using Application.Interfaces.IUseCases;
using Application.UseCases.CreateBusinessType.DTO;
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

    public BusinessTypesController(ICreateBusinessTypeUseCase createBusinessTypeUseCase)
    {
        _createBusinessTypeUseCase = createBusinessTypeUseCase;
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