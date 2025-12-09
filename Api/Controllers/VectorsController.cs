using Api.Authorization;
using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.CreateVetor.DTO;
using Application.UseCases.UpdateVetor.DTO;
using Application.UseCases.DeactivateVetor.DTO;
using Application.UseCases.ListVetores.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/vectors")]
[Authorize]
public class VectorsController : ControllerBase
{
    private readonly IVetorRepository _vetorRepository;
    private readonly ICreateVetorUseCase _createVetorUseCase;
    private readonly IUpdateVetorUseCase _updateVetorUseCase;
    private readonly IDeactivateVetorUseCase _deactivateVetorUseCase;
    private readonly IListVetoresUseCase _listVetoresUseCase;

    public VectorsController(
        IVetorRepository vetorRepository,
        ICreateVetorUseCase createVetorUseCase,
        IUpdateVetorUseCase updateVetorUseCase,
        IDeactivateVetorUseCase deactivateVetorUseCase,
        IListVetoresUseCase listVetoresUseCase)
    {
        _vetorRepository = vetorRepository;
        _createVetorUseCase = createVetorUseCase;
        _updateVetorUseCase = updateVetorUseCase;
        _deactivateVetorUseCase = deactivateVetorUseCase;
        _listVetoresUseCase = listVetoresUseCase;
    }

    /// <summary>
    /// Cria um novo vetor no sistema
    /// </summary>
    /// <param name="request">Dados do vetor a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do vetor criado</returns>
    [HttpPost]
    [AuthorizePermission("AdminGlobal")]
    [ProducesResponseType(typeof(CreateVetorResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateVetorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateVector([FromBody] CreateVetorRequest request, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(CreateVetorResult.Failure("Usuário atual não identificado."));
        }

        var result = await _createVetorUseCase.CreateAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetVector), 
            new { id = result.Vetor!.Id }, 
            result);
    }

    /// <summary>
    /// Atualiza um vetor existente no sistema
    /// </summary>
    /// <param name="id">ID do vetor a ser atualizado</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do vetor atualizado</returns>
    [HttpPut("{id}")]
    [AuthorizePermission("AdminGlobal")]
    [ProducesResponseType(typeof(Application.UseCases.UpdateVetor.DTO.UpdateVetorResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Application.UseCases.UpdateVetor.DTO.UpdateVetorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVector(Guid id, [FromBody] Application.UseCases.UpdateVetor.DTO.UpdateVetorRequest request, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(Application.UseCases.UpdateVetor.DTO.UpdateVetorResult.Failure("Usuário atual não identificado."));
        }

        var result = await _updateVetorUseCase.UpdateAsync(id, request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Message == "Vetor não encontrado.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Desativa um vetor no sistema
    /// </summary>
    /// <param name="id">ID do vetor a ser desativado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da desativação</returns>
    [HttpPatch("{id}/deactivate")]
    [AuthorizePermission("AdminGlobal")]
    [ProducesResponseType(typeof(Application.UseCases.DeactivateVetor.DTO.DeactivateVetorResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Application.UseCases.DeactivateVetor.DTO.DeactivateVetorResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateVector(Guid id, CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(Application.UseCases.DeactivateVetor.DTO.DeactivateVetorResult.Failure("Usuário atual não identificado."));
        }

        var result = await _deactivateVetorUseCase.DeactivateAsync(id, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Message == "Vetor não encontrado.")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtém um vetor por ID
    /// </summary>
    /// <param name="id">ID do vetor</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do vetor</returns>
    [HttpGet("{id}")]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetVector(Guid id, CancellationToken cancellationToken = default)
    {
        var vetor = await _vetorRepository.GetByIdAsync(id, cancellationToken);
        
        if (vetor == null)
        {
            return NotFound(new { message = "Vetor não encontrado." });
        }

        var result = new 
        { 
            id = vetor.Id, 
            name = vetor.Name, 
            email = vetor.Email, 
            active = vetor.Active,
            createdAt = vetor.CreatedAt
        };

        return Ok(result);
    }

    /// <summary>
    /// Lista vetores com filtros e paginação
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <param name="name">Filtro por nome</param>
    /// <param name="email">Filtro por email</param>
    /// <param name="active">Filtrar por status ativo (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de vetores com estatísticas</returns>
    [HttpGet]
    [AuthorizePermission("AdminGlobal", "AdminVetor")]
    [ProducesResponseType(typeof(ListVetoresResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ListVetoresResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetVectors(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null,
        [FromQuery] string? email = null,
        [FromQuery] bool? active = null,
        CancellationToken cancellationToken = default)
    {
        // Obter ID do usuário atual do token JWT
        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(ListVetoresResult.Failure("Usuário atual não identificado."));
        }

        var request = new ListVetoresRequest(
            Name: name,
            Email: email,
            Active: active,
            Page: page,
            PageSize: pageSize
        );

        var result = await _listVetoresUseCase.ListAsync(request, currentUserId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}