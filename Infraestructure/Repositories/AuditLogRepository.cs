using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Extensions;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly PartnerMeshDbContext _context;

    public AuditLogRepository(PartnerMeshDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return auditLog;
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(log => log.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entity, Guid entityId, CancellationToken cancellationToken = default)
    {
        var entityEnum = AuditEntityTypeExtensions.FromLegacyString(entity);
        return await _context.AuditLogs
            .Where(log => log.Entity == entityEnum && log.EntityId == entityId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        var actionEnum = AuditActionExtensions.FromLegacyString(action);
        return await _context.AuditLogs
            .Where(log => log.Action == actionEnum)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(log => log.CreatedAt >= startDate && log.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);
    }

    // Implementação UC-81 - Consulta avançada
    public async Task<(IEnumerable<AuditLog> Logs, int TotalCount)> QueryAsync(
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
        var query = _context.AuditLogs.AsQueryable();

        // Aplicar filtros
        if (userId.HasValue)
            query = query.Where(log => log.UserId == userId.Value);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(log => log.Action.ToLegacyString().Contains(action, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(entity))
            query = query.Where(log => log.Entity.ToLegacyString().Contains(entity, StringComparison.OrdinalIgnoreCase));

        if (entityId.HasValue)
            query = query.Where(log => log.EntityId == entityId.Value);

        if (startDate.HasValue)
            query = query.Where(log => log.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(log => log.CreatedAt <= endDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

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
        var logs = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (logs.AsEnumerable(), totalCount);
    }

    public async Task<int> CountAsync(
        Guid? userId = null,
        string? action = null,
        string? entity = null,
        Guid? entityId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsQueryable();

        // Aplicar os mesmos filtros do QueryAsync
        if (userId.HasValue)
            query = query.Where(log => log.UserId == userId.Value);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(log => log.Action.ToLegacyString().Contains(action, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(entity))
            query = query.Where(log => log.Entity.ToLegacyString().Contains(entity, StringComparison.OrdinalIgnoreCase));

        if (entityId.HasValue)
            query = query.Where(log => log.EntityId == entityId.Value);

        if (startDate.HasValue)
            query = query.Where(log => log.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(log => log.CreatedAt <= endDate.Value);

        return await query.CountAsync(cancellationToken);
    }
}