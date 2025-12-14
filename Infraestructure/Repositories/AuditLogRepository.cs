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

    // Implementação UC-81 - Consulta avançada
    public Task<(IEnumerable<AuditLog> Logs, int TotalCount)> QueryAsync(
        Guid? userId = null,
        string? action = null,
        string? entity = null,
        Guid? entityId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 50,
        string orderBy = "CreatedAt",
        string orderDirection = "DESC",
        CancellationToken cancellationToken = default)
    {
        var query = _auditLogs.AsQueryable();

        // Aplicar filtros
        if (userId.HasValue)
            query = query.Where(log => log.UserId == userId.Value);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(log => log.Action.Contains(action, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(entity))
            query = query.Where(log => log.Entity.Contains(entity, StringComparison.OrdinalIgnoreCase));

        if (entityId.HasValue)
            query = query.Where(log => log.EntityId == entityId.Value);

        if (startDate.HasValue)
            query = query.Where(log => log.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(log => log.CreatedAt <= endDate.Value);

        var totalCount = query.Count();

        // Aplicar ordenação
        query = orderBy.ToLower() switch
        {
            "action" => orderDirection.ToUpper() == "DESC" 
                ? query.OrderByDescending(log => log.Action)
                : query.OrderBy(log => log.Action),
            "entity" => orderDirection.ToUpper() == "DESC" 
                ? query.OrderByDescending(log => log.Entity)
                : query.OrderBy(log => log.Entity),
            "userid" => orderDirection.ToUpper() == "DESC" 
                ? query.OrderByDescending(log => log.UserId)
                : query.OrderBy(log => log.UserId),
            _ => orderDirection.ToUpper() == "DESC" 
                ? query.OrderByDescending(log => log.CreatedAt)
                : query.OrderBy(log => log.CreatedAt)
        };

        // Aplicar paginação
        var logs = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult((logs.AsEnumerable(), totalCount));
    }

    public Task<int> CountAsync(
        Guid? userId = null,
        string? action = null,
        string? entity = null,
        Guid? entityId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _auditLogs.AsQueryable();

        // Aplicar os mesmos filtros do QueryAsync
        if (userId.HasValue)
            query = query.Where(log => log.UserId == userId.Value);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(log => log.Action.Contains(action, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(entity))
            query = query.Where(log => log.Entity.Contains(entity, StringComparison.OrdinalIgnoreCase));

        if (entityId.HasValue)
            query = query.Where(log => log.EntityId == entityId.Value);

        if (startDate.HasValue)
            query = query.Where(log => log.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(log => log.CreatedAt <= endDate.Value);

        return Task.FromResult(query.Count());
    }
}