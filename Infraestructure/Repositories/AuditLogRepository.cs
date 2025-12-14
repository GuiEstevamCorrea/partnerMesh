using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infraestructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private static readonly List<AuditLog> _auditLogs = new();

    public Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        _auditLogs.Add(auditLog);
        return Task.FromResult(auditLog);
    }

    public Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_auditLogs.AsEnumerable());
    }

    public Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var logs = _auditLogs.Where(log => log.UserId == userId);
        return Task.FromResult(logs);
    }

    public Task<IEnumerable<AuditLog>> GetByEntityAsync(string entity, Guid entityId, CancellationToken cancellationToken = default)
    {
        var logs = _auditLogs.Where(log => log.Entity == entity && log.EntityId == entityId);
        return Task.FromResult(logs);
    }

    public Task<IEnumerable<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        var logs = _auditLogs.Where(log => log.Action == action);
        return Task.FromResult(logs);
    }

    public Task<IEnumerable<AuditLog>> GetByPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var logs = _auditLogs.Where(log => log.CreatedAt >= startDate && log.CreatedAt <= endDate);
        return Task.FromResult(logs);
    }
}