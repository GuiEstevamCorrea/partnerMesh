using Application.Interfaces.IUseCases;
using Application.UseCases.ListPayments.DTO;
using Application.UseCases.ProcessPayment.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;using Api.Extensions;
namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IListPaymentsUseCase _listPaymentsUseCase;
    private readonly IProcessPaymentUseCase _processPaymentUseCase;

    public PaymentsController(
        IListPaymentsUseCase listPaymentsUseCase,
        IProcessPaymentUseCase processPaymentUseCase)
    {
        _listPaymentsUseCase = listPaymentsUseCase;
        _processPaymentUseCase = processPaymentUseCase;
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

            var errorResult = this.GetCurrentUserIdOrError(out var userId);
            if (errorResult != null)
                return errorResult;

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

    /// <summary>
    /// UC-61 - Efetua pagamento (marca como pago)
    /// </summary>
    [HttpPut("{paymentId}/process")]
    public async Task<ActionResult<ProcessPaymentResult>> ProcessPayment(
        Guid paymentId,
        [FromBody] ProcessPaymentRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (paymentId == Guid.Empty)
            {
                return BadRequest(new { IsSuccess = false, Message = "ID do pagamento é obrigatório." });
            }

            var processRequest = new ProcessPaymentRequest
            {
                PaymentId = paymentId,
                Observations = request?.Observations
            };

            var errorResult = this.GetCurrentUserIdOrError(out var userId);
            if (errorResult != null)
                return errorResult;

            var result = await _processPaymentUseCase.ExecuteAsync(processRequest, userId, cancellationToken);

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