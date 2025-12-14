using Application.UseCases.AuditLogQuery.DTO;

namespace Application.Interfaces.IUseCases;

/// <summary>
/// Interface para UC-81 - Consultar Logs de Auditoria
/// Restrito ao Admin Global
/// </summary>
public interface IAuditLogQueryUseCase
{
    /// <summary>
    /// Consulta logs de auditoria com filtros
    /// </summary>
    /// <param name="request">Parâmetros de consulta</param>
    /// <returns>Resultado da consulta com logs paginados</returns>
    Task<AuditLogQueryResult> ExecuteAsync(AuditLogQueryRequest request);
}