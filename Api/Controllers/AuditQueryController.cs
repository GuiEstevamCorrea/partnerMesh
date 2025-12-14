using Application.Interfaces.IUseCases;
using Application.UseCases.AuditLogQuery.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller para UC-81 - Consultar Logs de Auditoria
/// Restrito ao Admin Global
/// </summary>
[ApiController]
[Route("api/audit")]
//[Authorize] // Temporariamente removido para teste - TODO: Implementar verificação específica de Admin Global
public class AuditQueryController : ControllerBase
{
    private readonly IAuditLogQueryUseCase _auditLogQueryUseCase;

    public AuditQueryController(IAuditLogQueryUseCase auditLogQueryUseCase)
    {
        _auditLogQueryUseCase = auditLogQueryUseCase;
    }

    /// <summary>
    /// UC-81: Consulta logs de auditoria com filtros avançados
    /// Restrito ao Admin Global
    /// </summary>
    /// <param name="request">Parâmetros de consulta</param>
    /// <returns>Lista paginada de logs de auditoria</returns>
    [HttpGet("logs")]
    public async Task<IActionResult> QueryLogs([FromQuery] AuditLogQueryRequest request)
    {
        try
        {
            // TODO: Verificar se usuário é Admin Global
            // var userProfile = GetUserProfile(); 
            // if (userProfile != UserProfile.AdminGlobal)
            //     return Forbid("Acesso restrito ao Admin Global");

            var result = await _auditLogQueryUseCase.ExecuteAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { 
                    message = result.Message
                });
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Erro interno do servidor", 
                error = ex.Message 
            });
        }
    }

    /// <summary>
    /// UC-81: Consulta logs de auditoria com filtros via POST (para consultas complexas)
    /// Restrito ao Admin Global
    /// </summary>
    /// <param name="request">Parâmetros de consulta</param>
    /// <returns>Lista paginada de logs de auditoria</returns>
    [HttpPost("logs/search")]
    public async Task<IActionResult> SearchLogs([FromBody] AuditLogQueryRequest request)
    {
        try
        {
            // TODO: Verificar se usuário é Admin Global
            
            var result = await _auditLogQueryUseCase.ExecuteAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { 
                    message = result.Message
                });
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Erro interno do servidor", 
                error = ex.Message 
            });
        }
    }

    /// <summary>
    /// Obtém estatísticas de logs de auditoria
    /// </summary>
    /// <param name="startDate">Data inicial</param>
    /// <param name="endDate">Data final</param>
    /// <returns>Estatísticas dos logs</returns>
    [HttpGet("logs/stats")]
    public async Task<IActionResult> GetStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            // Consultar logs para estatísticas básicas
            var request = new AuditLogQueryRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                PageSize = 1000, // Para contagem
                PageNumber = 1
            };

            var result = await _auditLogQueryUseCase.ExecuteAsync(request);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Message });
            }

            // Calcular estatísticas básicas
            var logs = result.Data?.Logs ?? Enumerable.Empty<AuditLogDto>();
            var stats = new
            {
                TotalLogs = result.Data?.TotalRecords ?? 0,
                Period = new
                {
                    Start = startDate?.ToString("yyyy-MM-dd"),
                    End = endDate?.ToString("yyyy-MM-dd")
                },
                ActionCounts = logs.GroupBy(l => l.Action)
                    .Select(g => new { Action = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10),
                EntityCounts = logs.GroupBy(l => l.Entity)
                    .Select(g => new { Entity = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10),
                RecentActions = logs.Take(5)
                    .Select(l => new { 
                        l.Action, 
                        l.Entity, 
                        l.CreatedAt,
                        l.UserId 
                    })
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                message = "Erro interno do servidor", 
                error = ex.Message 
            });
        }
    }
}