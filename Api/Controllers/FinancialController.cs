using Application.Interfaces.IUseCases;
using Application.UseCases.FinancialReport.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;using Api.Extensions;
namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinancialController : ControllerBase
{
    private readonly IFinancialReportUseCase _financialReportUseCase;

    public FinancialController(IFinancialReportUseCase financialReportUseCase)
    {
        _financialReportUseCase = financialReportUseCase;
    }

    /// <summary>
    /// UC-71 - Relatório Financeiro
    /// Gera relatório completo das comissões com totais pagos, pendentes, por nível e por vetor
    /// </summary>
    [HttpGet("report")]
    public async Task<ActionResult<FinancialReportResult>> GetFinancialReport(
        [FromQuery] Guid? vetorId = null,
        [FromQuery] Guid? partnerId = null,
        [FromQuery] Guid? businessTypeId = null,
        [FromQuery] bool? isPaid = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? level = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new FinancialReportRequest
            {
                VetorId = vetorId,
                PartnerId = partnerId,
                BusinessTypeId = businessTypeId,
                IsPaid = isPaid,
                StartDate = startDate,
                EndDate = endDate,
                Level = level
            };

            var errorResult = this.GetCurrentUserIdOrError(out var userId);
            if (errorResult != null)
                return errorResult;

            var result = await _financialReportUseCase.ExecuteAsync(request, userId, cancellationToken);

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