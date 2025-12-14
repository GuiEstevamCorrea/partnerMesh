using Application.Interfaces.IUseCases;
using Application.UseCases.LogAudit.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller responsável por operações de auditoria do sistema
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly ILogAuditUseCase _logAuditUseCase;

    public AuditController(ILogAuditUseCase logAuditUseCase)
    {
        _logAuditUseCase = logAuditUseCase;
    }

    /// <summary>
    /// Health check do controller de auditoria
    /// </summary>
    /// <returns>Status do controller</returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { 
            controller = "AuditController", 
            status = "healthy", 
            timestamp = DateTime.UtcNow 
        });
    }

    /// <summary>
    /// Registra uma ação de auditoria no sistema
    /// </summary>
    /// <param name="request">Dados da ação de auditoria</param>
    /// <returns>Resultado do registro de auditoria</returns>
    [HttpPost("log")]
    public async Task<IActionResult> LogAudit([FromBody] LogAuditRequest request)
    {
        try
        {
            var result = await _logAuditUseCase.ExecuteAsync(request);
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { 
                    message = result.Message
                });
            }

            return Ok(result);
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
    /// Exemplo de endpoint que demonstra uso automático de auditoria
    /// </summary>
    /// <param name="entityId">ID da entidade</param>
    /// <returns>Confirmação da operação</returns>
    [HttpPost("example/{entityId}")]
    public async Task<IActionResult> ExampleOperation(Guid entityId)
    {
        try
        {
            // Simular uma operação que requer auditoria
            var auditRequest = new LogAuditRequest
            {
                UserId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), // Exemplo de GUID
                Action = AuditActions.UPDATE,
                Entity = AuditEntities.PARTNER,
                EntityId = entityId,
                Data = "{\"operation\": \"example\", \"timestamp\": \"" + DateTime.UtcNow.ToString("O") + "\"}"
            };

            var auditResult = await _logAuditUseCase.ExecuteAsync(auditRequest);
            
            if (!auditResult.IsSuccess)
            {
                return BadRequest(new { 
                    message = "Falha ao registrar auditoria", 
                    error = auditResult.Message 
                });
            }

            return Ok(new { 
                message = "Operação executada com sucesso",
                auditLogId = auditResult.Data?.LogId,
                createdAt = auditResult.Data?.CreatedAt
            });
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