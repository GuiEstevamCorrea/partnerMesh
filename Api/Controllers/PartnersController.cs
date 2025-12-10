using Api.Authorization;
using Application.Interfaces.IUseCases;
using Application.UseCases.CreatePartner.DTO;
using Application.UseCases.UpdatePartner.DTO;
using Application.UseCases.ActivateDeactivatePartner.DTO;
using Application.UseCases.ListPartners.DTO;
using Application.UseCases.GetPartnerById.DTO;
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
    private readonly IListPartnersUseCase _listPartnersUseCase;
    private readonly IGetPartnerByIdUseCase _getPartnerByIdUseCase;

    public PartnersController(
        ICreatePartnerUseCase createPartnerUseCase,
        IUpdatePartnerUseCase updatePartnerUseCase,
        IActivateDeactivatePartnerUseCase activateDeactivatePartnerUseCase,
        IListPartnersUseCase listPartnersUseCase,
        IGetPartnerByIdUseCase getPartnerByIdUseCase)
    {
        _createPartnerUseCase = createPartnerUseCase;
        _updatePartnerUseCase = updatePartnerUseCase;
        _activateDeactivatePartnerUseCase = activateDeactivatePartnerUseCase;
        _listPartnersUseCase = listPartnersUseCase;
        _getPartnerByIdUseCase = getPartnerByIdUseCase;
    }

    /// <summary>
    /// UC-33: Listar Parceiros
    /// </summary>
    /// <param name="name">Filtro por nome</param>
    /// <param name="email">Filtro por email</param>
    /// <param name="phoneNumber">Filtro por telefone</param>
    /// <param name="isActive">Filtro por status ativo/inativo</param>
    /// <param name="vetorId">Filtro por ID do vetor</param>
    /// <param name="recommenderId">Filtro por ID do recomendador</param>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Itens por página</param>
    /// <param name="orderBy">Campo para ordenação</param>
    /// <param name="orderDirection">Direção da ordenação (asc/desc)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de parceiros</returns>
    [HttpGet]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListPartners(
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] string? phoneNumber,
        [FromQuery] bool? isActive,
        [FromQuery] Guid? vetorId,
        [FromQuery] Guid? recommenderId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? orderBy = "name",
        [FromQuery] string? orderDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(ListPartnersResult.Failure("Usuário atual não identificado."));
        }

        var request = new ListPartnersRequest
        {
            Name = name,
            Email = email,
            PhoneNumber = phoneNumber,
            IsActive = isActive,
            VetorId = vetorId,
            RecommenderId = recommenderId,
            Page = page,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDirection = orderDirection
        };

        var result = await _listPartnersUseCase.ListAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
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
            nameof(GetPartnerById), 
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
    /// UC-34: Obter Parceiro por ID
    /// </summary>
    /// <param name="id">ID do parceiro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações detalhadas do parceiro</returns>
    [HttpGet("{id:guid}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor", "Operador")]
    [ProducesResponseType(typeof(GetPartnerByIdResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GetPartnerByIdResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPartnerById(Guid id, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(GetPartnerByIdResult.Failure("Usuário atual não identificado."));
        }

        var result = await _getPartnerByIdUseCase.GetByIdAsync(id, currentUserId, cancellationToken);

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
}