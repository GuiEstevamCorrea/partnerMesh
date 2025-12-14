using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entity, Guid entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    // Métodos para UC-81 - Consulta avançada de logs
    /// <summary>
    /// Consulta logs com filtros avançados e paginação
    /// </summary>
    Task<(IEnumerable<AuditLog> Logs, int TotalCount)> QueryAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta total de logs que atendem aos filtros especificados
    /// </summary>
    Task<int> CountAsync(
        Guid? userId = null,
        string? action = null,
        string? entity = null,
        Guid? entityId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}