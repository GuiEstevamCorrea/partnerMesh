using Api.Authorization;
using Application.Interfaces.IUseCases;
using Application.UseCases.CreatePartner.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/partners")]
[Authorize]
public class PartnersController : ControllerBase
{
    private readonly ICreatePartnerUseCase _createPartnerUseCase;

    public PartnersController(ICreatePartnerUseCase createPartnerUseCase)
    {
        _createPartnerUseCase = createPartnerUseCase;
    }

    /// <summary>
    /// Cria um novo parceiro no sistema
    /// </summary>
    /// <param name="request">Dados do parceiro a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do parceiro criado</returns>
    [HttpPost]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(typeof(CreatePartnerResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreatePartnerResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreatePartner([FromBody] CreatePartnerRequest request, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(CreatePartnerResult.Failure("Usuário atual não identificado."));
        }

        var result = await _createPartnerUseCase.CreateAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetPartner), 
            new { id = result.Partner!.Id }, 
            result);
    }

    /// <summary>
    /// Obtém um parceiro por ID (endpoint placeholder)
    /// </summary>
    /// <param name="id">ID do parceiro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do parceiro</returns>
    [HttpGet("{id}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetPartner(Guid id, CancellationToken cancellationToken = default)
    {
        // TODO: Implementar UC-34 (Obter Parceiro por ID)
        return Ok(new { message = "UC-34 não implementado ainda.", partnerId = id });
    }
}