using Application.Interfaces.IUseCases;
using Application.UseCases.PartnersReport.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;using Api.Extensions;
namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IPartnersReportUseCase _partnersReportUseCase;

    public ReportsController(IPartnersReportUseCase partnersReportUseCase)
    {
        _partnersReportUseCase = partnersReportUseCase;
    }

    /// <summary>
    /// UC-70 - Relatório de Parceiros
    /// Gera relatório completo dos parceiros com árvore hierárquica, totais por nível e resumo financeiro
    /// </summary>
    [HttpGet("partners")]
    public async Task<ActionResult<PartnersReportResult>> GetPartnersReport(
        [FromQuery] Guid? vetorId = null,
        [FromQuery] bool? activeOnly = null,
        [FromQuery] int? level = null,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new PartnersReportRequest
            {
                VetorId = vetorId,
                ActiveOnly = activeOnly,
                Level = level,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var errorResult = this.GetCurrentUserIdOrError(out var userId);
            if (errorResult != null)
                return errorResult;

            var result = await _partnersReportUseCase.ExecuteAsync(request, userId, cancellationToken);

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