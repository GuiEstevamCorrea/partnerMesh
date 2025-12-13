using Application.Interfaces.IUseCases;
using Application.UseCases.CreateBusiness.DTO;
using Application.UseCases.UpdateBusiness.DTO;
using Application.UseCases.CancelBusiness.DTO;
using Application.UseCases.ListBusinesses.DTO;
using Application.UseCases.GetBusinessById.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BusinessController : ControllerBase
{
    private readonly ICreateBusinessUseCase _createBusinessUseCase;
    private readonly IUpdateBusinessUseCase _updateBusinessUseCase;
    private readonly ICancelBusinessUseCase _cancelBusinessUseCase;
    private readonly IListBusinessesUseCase _listBusinessesUseCase;
    private readonly IGetBusinessByIdUseCase _getBusinessByIdUseCase;

    public BusinessController(
        ICreateBusinessUseCase createBusinessUseCase,
        IUpdateBusinessUseCase updateBusinessUseCase,
        ICancelBusinessUseCase cancelBusinessUseCase,
        IListBusinessesUseCase listBusinessesUseCase,
        IGetBusinessByIdUseCase getBusinessByIdUseCase)
    {
        _createBusinessUseCase = createBusinessUseCase;
        _updateBusinessUseCase = updateBusinessUseCase;
        _cancelBusinessUseCase = cancelBusinessUseCase;
        _listBusinessesUseCase = listBusinessesUseCase;
        _getBusinessByIdUseCase = getBusinessByIdUseCase;
    }

    /// <summary>
    /// Obter um negócio específico por ID com detalhes completos
    /// </summary>
    /// <param name="businessId">ID do negócio a ser consultado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Detalhes completos do negócio incluindo informações de comissão</returns>
    [HttpGet("{businessId}")]
    public async Task<ActionResult<GetBusinessByIdResult>> GetBusinessById(
        [FromRoute] Guid businessId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Token de autenticação inválido" });
            }

            var result = await _getBusinessByIdUseCase.ExecuteAsync(businessId, userId);
            
            if (!result.IsSuccess)
            {
                if (result.Message.Contains("não encontrado"))
                {
                    return NotFound(new { message = result.Message });
                }
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
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
    /// Listar negócios com filtros e paginação
    /// </summary>
    /// <param name="partnerId">Filtro por ID do parceiro</param>
    /// <param name="businessTypeId">Filtro por ID do tipo de negócio</param>
    /// <param name="status">Filtro por status (ativo, cancelado)</param>
    /// <param name="minValue">Filtro por valor mínimo</param>
    /// <param name="maxValue">Filtro por valor máximo</param>
    /// <param name="startDate">Filtro por data inicial</param>
    /// <param name="endDate">Filtro por data final</param>
    /// <param name="searchText">Busca por texto nas observações</param>
    /// <param name="sortBy">Campo para ordenação</param>
    /// <param name="sortDirection">Direção da ordenação</param>
    /// <param name="page">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de negócios com informações de comissão</returns>
    [HttpGet]
    public async Task<ActionResult<ListBusinessesResult>> ListBusinesses(
        [FromQuery] Guid? partnerId = null,
        [FromQuery] Guid? businessTypeId = null,
        [FromQuery] string? status = null,
        [FromQuery] decimal? minValue = null,
        [FromQuery] decimal? maxValue = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? searchText = null,
        [FromQuery] string? sortBy = "date",
        [FromQuery] string? sortDirection = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Token de autenticação inválido" });
            }

            var request = new ListBusinessesRequest
            {
                PartnerId = partnerId,
                BusinessTypeId = businessTypeId,
                Status = status,
                MinValue = minValue,
                MaxValue = maxValue,
                StartDate = startDate,
                EndDate = endDate,
                SearchText = searchText,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                PageSize = pageSize
            };

            var result = await _listBusinessesUseCase.ExecuteAsync(request, userId);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
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
    /// Criar um novo negócio com cálculo automático de comissões
    /// </summary>
    /// <param name="request">Dados do negócio a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do negócio criado e resumo das comissões</returns>
    [HttpPost]
    public async Task<ActionResult<CreateBusinessResult>> CreateBusiness(
        [FromBody] CreateBusinessRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Token de autenticação inválido" });
            }

            var result = await _createBusinessUseCase.ExecuteAsync(request, userId);
            return Ok(result);
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
    /// Atualizar um negócio existente (apenas campos não críticos)
    /// </summary>
    /// <param name="businessId">ID do negócio a ser atualizado</param>
    /// <param name="request">Dados para atualização</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do negócio atualizado</returns>
    [HttpPut("{businessId}")]
    public async Task<ActionResult<UpdateBusinessResult>> UpdateBusiness(
        [FromRoute] Guid businessId,
        [FromBody] UpdateBusinessRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Token de autenticação inválido" });
            }

            var result = await _updateBusinessUseCase.ExecuteAsync(businessId, request, userId);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
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
    /// Cancelar um negócio existente
    /// </summary>
    /// <param name="businessId">ID do negócio a ser cancelado</param>
    /// <param name="request">Dados do cancelamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do negócio cancelado e resumo das comissões afetadas</returns>
    [HttpDelete("{businessId}")]
    public async Task<ActionResult<CancelBusinessResult>> CancelBusiness(
        [FromRoute] Guid businessId,
        [FromBody] CancelBusinessRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(new { message = "Token de autenticação inválido" });
            }

            var result = await _cancelBusinessUseCase.ExecuteAsync(businessId, request, userId);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
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
}