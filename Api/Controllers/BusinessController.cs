using Application.Interfaces.IUseCases;
using Application.UseCases.CreateBusiness.DTO;
using Application.UseCases.UpdateBusiness.DTO;
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

    public BusinessController(
        ICreateBusinessUseCase createBusinessUseCase,
        IUpdateBusinessUseCase updateBusinessUseCase)
    {
        _createBusinessUseCase = createBusinessUseCase;
        _updateBusinessUseCase = updateBusinessUseCase;
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
}