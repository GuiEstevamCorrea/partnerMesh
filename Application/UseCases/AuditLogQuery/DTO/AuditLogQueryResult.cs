using Domain.Entities;
using Domain.Extensions;
using Domain.ValueTypes;

namespace Application.UseCases.AuditLogQuery.DTO;

/// <summary>
/// Resultado da consulta de logs de auditoria
/// </summary>
public record AuditLogQueryResult
{
    /// <summary>
    /// Indica se a consulta foi bem-sucedida
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Mensagem de erro ou sucesso
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Dados dos logs encontrados
    /// </summary>
    public AuditLogQueryData? Data { get; init; }

    public static AuditLogQueryResult Success(AuditLogQueryData data) 
        => new() { IsSuccess = true, Message = "Logs consultados com sucesso", Data = data };

    public static AuditLogQueryResult Failure(string message) 
        => new() { IsSuccess = false, Message = message };
}

/// <summary>
/// Dados da consulta de logs com paginação
/// </summary>
public record AuditLogQueryData
{
    /// <summary>
    /// Lista de logs de auditoria
    /// </summary>
    public IEnumerable<AuditLogDto> Logs { get; init; } = Enumerable.Empty<AuditLogDto>();

    /// <summary>
    /// Total de registros encontrados
    /// </summary>
    public int TotalRecords { get; init; }

    /// <summary>
    /// Página atual
    /// </summary>
    public int CurrentPage { get; init; }

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Indica se há página anterior
    /// </summary>
    public bool HasPreviousPage { get; init; }

    /// <summary>
    /// Indica se há próxima página
    /// </summary>
    public bool HasNextPage { get; init; }
}

/// <summary>
/// DTO para exibição de log de auditoria
/// </summary>
public record AuditLogDto
{
    /// <summary>
    /// ID do log
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// ID do usuário que executou a ação
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Nome do usuário que executou a ação
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Ação executada
    /// </summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>
    /// Entidade afetada
    /// </summary>
    public string Entity { get; init; } = string.Empty;

    /// <summary>
    /// ID da entidade afetada
    /// </summary>
    public Guid EntityId { get; init; }

    /// <summary>
    /// Nome/Descrição da entidade afetada (quando disponível)
    /// </summary>
    public string? EntityName { get; init; }

    /// <summary>
    /// Dados serializados da ação
    /// </summary>
    public string Data { get; init; } = string.Empty;

    /// <summary>
    /// Data e hora da criação do log
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Converte de AuditLog para AuditLogDto
    /// </summary>
    /// <param name="auditLog">Entidade AuditLog</param>
    /// <param name="userName">Nome do usuário</param>
    /// <param name="entityName">Nome da entidade (opcional)</param>
    /// <returns>DTO para exibição</returns>
    public static AuditLogDto FromEntity(AuditLog auditLog, string userName, string? entityName = null)
    {
        return new AuditLogDto
        {
            Id = auditLog.Id,
            UserId = auditLog.UserId,
            UserName = userName,
            Action = auditLog.Action.ToLegacyString(),
            Entity = auditLog.Entity.ToLegacyString(),
            EntityId = auditLog.EntityId,
            EntityName = entityName,
            Data = auditLog.Datas,
            CreatedAt = auditLog.CreatedAt
        };
    }
}