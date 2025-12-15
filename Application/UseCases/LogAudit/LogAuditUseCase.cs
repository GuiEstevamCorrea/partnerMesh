using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.LogAudit.DTO;
using Domain.Entities;
using Domain.Extensions;

namespace Application.UseCases.LogAudit;

public class LogAuditUseCase : ILogAuditUseCase
{
    private readonly IAuditLogRepository _auditLogRepository;

    public LogAuditUseCase(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<LogAuditResult> ExecuteAsync(LogAuditRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validações básicas
            if (request.UserId == Guid.Empty)
            {
                return LogAuditResult.Failure("ID do usuário é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(request.Action))
            {
                return LogAuditResult.Failure("Ação é obrigatória.");
            }

            if (string.IsNullOrWhiteSpace(request.Entity))
            {
                return LogAuditResult.Failure("Entidade é obrigatória.");
            }

            if (request.EntityId == Guid.Empty)
            {
                return LogAuditResult.Failure("ID da entidade é obrigatório.");
            }

            // Converter strings para enums
            if (!AuditActionExtensions.TryParse(request.Action, out var actionEnum))
            {
                return LogAuditResult.Failure($"Ação de auditoria inválida: {request.Action}");
            }

            if (!AuditEntityTypeExtensions.TryParse(request.Entity, out var entityEnum))
            {
                return LogAuditResult.Failure($"Tipo de entidade inválido: {request.Entity}");
            }

            // Criar entidade de auditoria
            var auditLog = new AuditLog(
                userId: request.UserId,
                action: actionEnum,
                entity: entityEnum,
                entityId: request.EntityId,
                datas: request.Data ?? string.Empty
            );

            // Salvar no repositório
            var savedAuditLog = await _auditLogRepository.CreateAsync(auditLog, cancellationToken);

            // Retornar resultado de sucesso
            var resultData = new LogAuditData
            {
                LogId = savedAuditLog.Id,
                CreatedAt = savedAuditLog.CreatedAt,
                UserId = savedAuditLog.UserId,
                Action = savedAuditLog.Action.ToLegacyString(),
                Entity = savedAuditLog.Entity.ToLegacyString(),
                EntityId = savedAuditLog.EntityId
            };

            return LogAuditResult.Success(resultData);
        }
        catch (Exception ex)
        {
            // Em um sistema de auditoria, é crítico que os logs sejam registrados
            // Porém, se falhar, não deve quebrar o fluxo principal da aplicação
            return LogAuditResult.Failure($"Erro ao registrar log de auditoria: {ex.Message}");
        }
    }
}