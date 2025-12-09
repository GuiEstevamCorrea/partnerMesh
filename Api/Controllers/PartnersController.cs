using Api.Authorization;
using Application.Interfaces.IUseCases;
using Application.UseCases.CreatePartner.DTO;
using Application.UseCases.UpdatePartner.DTO;
using Application.UseCases.ActivateDeactivatePartner.DTO;
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
    private readonly IUpdatePartnerUseCase _updatePartnerUseCase;
    private readonly IActivateDeactivatePartnerUseCase _activateDeactivatePartnerUseCase;

    public PartnersController(
        ICreatePartnerUseCase createPartnerUseCase,
        IUpdatePartnerUseCase updatePartnerUseCase,
        IActivateDeactivatePartnerUseCase activateDeactivatePartnerUseCase)
    {
        _createPartnerUseCase = createPartnerUseCase;
        _updatePartnerUseCase = updatePartnerUseCase;
        _activateDeactivatePartnerUseCase = activateDeactivatePartnerUseCase;
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
    /// Atualiza um parceiro existente no sistema
    /// </summary>
    /// <param name="id">ID do parceiro a ser atualizado</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do parceiro atualizado</returns>
    [HttpPut("{id}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(typeof(Application.UseCases.UpdatePartner.DTO.UpdatePartnerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Application.UseCases.UpdatePartner.DTO.UpdatePartnerResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePartner(Guid id, [FromBody] Application.UseCases.UpdatePartner.DTO.UpdatePartnerRequest request, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(Application.UseCases.UpdatePartner.DTO.UpdatePartnerResult.Failure("Usuário atual não identificado."));
        }

        var result = await _updatePartnerUseCase.UpdateAsync(id, request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Message == "Parceiro não encontrado.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// UC-32: Ativar/Desativar Parceiro
    /// </summary>
    /// <param name="id">ID do parceiro</param>
    /// <param name="request">Dados da operação de ativação/desativação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpPatch("{id:guid}/status")]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateDeactivatePartner(Guid id, [FromBody] ActivateDeactivatePartnerRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return BadRequest(ActivateDeactivatePartnerResult.Failure("Request body is required"));

        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(ActivateDeactivatePartnerResult.Failure("Usuário atual não identificado."));
        }

        // Definir o ID do parceiro na request
        request.PartnerId = id;

        var result = await _activateDeactivatePartnerUseCase.ActivateDeactivateAsync(id, request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Message == "Parceiro não encontrado.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
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