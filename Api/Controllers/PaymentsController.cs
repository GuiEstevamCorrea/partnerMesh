using Api.Extensions;
using Api.Models;
using Application.Interfaces.IUseCases;
using Application.UseCases.ListPayments.DTO;
using Application.UseCases.ProcessPayment.DTO;
using Application.UseCases.GetCancelledBusinessSummary.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IListPaymentsUseCase _listPaymentsUseCase;
    private readonly IProcessPaymentUseCase _processPaymentUseCase;
    private readonly ICancelledBusinessSummaryUseCase _cancelledBusinessSummaryUseCase;

    public PaymentsController(
        IListPaymentsUseCase listPaymentsUseCase,
        IProcessPaymentUseCase processPaymentUseCase,
        ICancelledBusinessSummaryUseCase cancelledBusinessSummaryUseCase)
    {
        _listPaymentsUseCase = listPaymentsUseCase;
        _processPaymentUseCase = processPaymentUseCase;
        _cancelledBusinessSummaryUseCase = cancelledBusinessSummaryUseCase;
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

            // Retornar no formato esperado pelo frontend
            var response = new
            {
                items = result.Payments,
                totalItems = result.Pagination.TotalItems,
                page = result.Pagination.Page,
                pageSize = result.Pagination.PageSize,
                totalPages = result.Pagination.TotalPages
            };

            return Ok(response);
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
    /// UC-61 - Processa múltiplos pagamentos em lote
    /// </summary>
    [HttpPost("process")]
    public async Task<ActionResult> ProcessMultiplePayments(
        [FromBody] ProcessMultiplePaymentsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null || request.PaymentIds == null || !request.PaymentIds.Any())
            {
                return BadRequest(new { IsSuccess = false, Message = "IDs dos pagamentos são obrigatórios." });
            }

            var errorResult = this.GetCurrentUserIdOrError(out var userId);
            if (errorResult != null)
                return errorResult;

            var results = new List<object>();
            var successCount = 0;
            var errorCount = 0;

            foreach (var paymentId in request.PaymentIds)
            {
                var processRequest = new Application.UseCases.ProcessPayment.DTO.ProcessPaymentRequest
                {
                    PaymentId = paymentId,
                    Observations = request.Observations
                };

                var result = await _processPaymentUseCase.ExecuteAsync(processRequest, userId, cancellationToken);
                
                if (result.IsSuccess)
                {
                    successCount++;
                }
                else
                {
                    errorCount++;
                    results.Add(new { PaymentId = paymentId, Error = result.Message });
                }
            }

            if (errorCount == 0)
            {
                return Ok(new { 
                    IsSuccess = true, 
                    Message = $"{successCount} pagamento(s) processado(s) com sucesso." 
                });
            }
            else if (successCount == 0)
            {
                return BadRequest(new { 
                    IsSuccess = false, 
                    Message = $"Erro ao processar todos os {errorCount} pagamento(s).",
                    Errors = results
                });
            }
            else
            {
                return Ok(new { 
                    IsSuccess = true, 
                    Message = $"{successCount} pagamento(s) processado(s) com sucesso. {errorCount} erro(s).",
                    PartialSuccess = true,
                    Errors = results
                });
            }
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

    /// <summary>
    /// Obter resumo dos valores de negócios cancelados
    /// </summary>
    [HttpGet("cancelled-business-summary")]
    public async Task<ActionResult<CancelledBusinessSummaryResult>> GetCancelledBusinessSummary(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var errorResult = this.GetCurrentUserIdOrError(out var userId);
            if (errorResult != null)
                return errorResult;

            var result = await _cancelledBusinessSummaryUseCase.ExecuteAsync(userId, cancellationToken);

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