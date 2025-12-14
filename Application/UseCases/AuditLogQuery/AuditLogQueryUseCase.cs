using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.AuditLogQuery.DTO;

namespace Application.UseCases.AuditLogQuery;

/// <summary>
/// UC-81 - Consultar Logs de Auditoria
/// Restrito ao Admin Global
/// </summary>
public class AuditLogQueryUseCase : IAuditLogQueryUseCase
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogQueryUseCase(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<AuditLogQueryResult> ExecuteAsync(AuditLogQueryRequest request)
    {
        try
        {
            // Validar parâmetros de entrada
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsValid)
            {
                return AuditLogQueryResult.Failure(validationResult.ErrorMessage);
            }

            // Normalizar parâmetros
            var normalizedRequest = NormalizeRequest(request);

            // Executar consulta
            var (logs, totalCount) = await _auditLogRepository.QueryAsync(
                userId: normalizedRequest.UserId,
                action: normalizedRequest.Action,
                entity: normalizedRequest.Entity,
                entityId: normalizedRequest.EntityId,
                startDate: normalizedRequest.StartDate,
                endDate: normalizedRequest.EndDate,
                pageNumber: normalizedRequest.PageNumber,
                pageSize: normalizedRequest.PageSize,
                orderBy: normalizedRequest.OrderBy,
                orderDirection: normalizedRequest.OrderDirection);

            // Converter para DTOs
            var logDtos = logs.Select(AuditLogDto.FromEntity).ToList();

            // Calcular informações de paginação
            var totalPages = (int)Math.Ceiling((double)totalCount / normalizedRequest.PageSize);
            var hasNextPage = normalizedRequest.PageNumber < totalPages;
            var hasPreviousPage = normalizedRequest.PageNumber > 1;

            var queryData = new AuditLogQueryData
            {
                Logs = logDtos,
                TotalRecords = totalCount,
                CurrentPage = normalizedRequest.PageNumber,
                PageSize = normalizedRequest.PageSize,
                TotalPages = totalPages,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage
            };

            return AuditLogQueryResult.Success(queryData);
        }
        catch (Exception ex)
        {
            return AuditLogQueryResult.Failure($"Erro interno ao consultar logs: {ex.Message}");
        }
    }

    private ValidationResult ValidateRequest(AuditLogQueryRequest request)
    {
        // Validar página
        if (request.PageNumber < 1)
        {
            return new ValidationResult(false, "Número da página deve ser maior que zero");
        }

        // Validar tamanho da página
        if (request.PageSize < 1 || request.PageSize > 100)
        {
            return new ValidationResult(false, "Tamanho da página deve estar entre 1 e 100");
        }

        // Validar período de datas
        if (request.StartDate.HasValue && request.EndDate.HasValue && 
            request.StartDate.Value > request.EndDate.Value)
        {
            return new ValidationResult(false, "Data inicial não pode ser maior que data final");
        }

        // Validar ordenação
        var validOrderFields = new[] { "createdat", "action", "entity", "userid" };
        if (!validOrderFields.Contains(request.OrderBy.ToLower()))
        {
            return new ValidationResult(false, "Campo de ordenação inválido. Use: CreatedAt, Action, Entity, UserId");
        }

        var validOrderDirections = new[] { "asc", "desc" };
        if (!validOrderDirections.Contains(request.OrderDirection.ToLower()))
        {
            return new ValidationResult(false, "Direção de ordenação inválida. Use: ASC ou DESC");
        }

        return new ValidationResult(true, string.Empty);
    }

    private AuditLogQueryRequest NormalizeRequest(AuditLogQueryRequest request)
    {
        return request with
        {
            Action = string.IsNullOrWhiteSpace(request.Action) ? null : request.Action.Trim(),
            Entity = string.IsNullOrWhiteSpace(request.Entity) ? null : request.Entity.Trim(),
            OrderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "CreatedAt" : request.OrderBy.Trim(),
            OrderDirection = string.IsNullOrWhiteSpace(request.OrderDirection) ? "DESC" : request.OrderDirection.Trim(),
            PageSize = request.PageSize <= 0 ? 50 : Math.Min(request.PageSize, 100),
            PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber
        };
    }

    private record ValidationResult(bool IsValid, string ErrorMessage);
}