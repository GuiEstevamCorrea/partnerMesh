using Application.Interfaces.IUseCases;
using Application.UseCases.ListPayments.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IListPaymentsUseCase _listPaymentsUseCase;

    public PaymentsController(IListPaymentsUseCase listPaymentsUseCase)
    {
        _listPaymentsUseCase = listPaymentsUseCase;
    }

    /// <summary>
    /// UC-60 - Lista pagamentos com filtros
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ListPaymentsResult>> ListPayments(
        [FromQuery] Guid? vetorId = null,
        [FromQuery] Guid? partnerId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? status = null,
        [FromQuery] string? tipoPagamento = null,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortDirection = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ListPaymentsRequest
            {
                VetorId = vetorId,
                PartnerId = partnerId,
                StartDate = startDate,
                EndDate = endDate,
                Status = status,
                TipoPagamento = tipoPagamento,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                PageSize = pageSize
            };

            // TODO: Extrair userId do token JWT
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var result = await _listPaymentsUseCase.ExecuteAsync(request, userId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                IsSuccess = false, 
                Message = "Erro interno do servidor.", 
                Details = ex.Message 
            });
        }
    }
}