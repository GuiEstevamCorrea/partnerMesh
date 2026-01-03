using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.AuditLogQuery.DTO;
using Domain.Extensions;

namespace Application.UseCases.AuditLogQuery;

/// <summary>
/// UC-81 - Consultar Logs de Auditoria
/// Restrito ao Admin Global
/// </summary>
public class AuditLogQueryUseCase : IAuditLogQueryUseCase
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IVetorRepository _vetorRepository;

    public AuditLogQueryUseCase(
        IAuditLogRepository auditLogRepository,
        IUserRepository userRepository,
        IPartnerRepository partnerRepository,
        IBusinessRepository businessRepository,
        IVetorRepository vetorRepository)
    {
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
        _partnerRepository = partnerRepository;
        _businessRepository = businessRepository;
        _vetorRepository = vetorRepository;
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
            var logDtos = await ConvertToDto(logs);

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

    private async Task<List<AuditLogDto>> ConvertToDto(IEnumerable<Domain.Entities.AuditLog> logs)
    {
        var logsList = logs.ToList();
        if (!logsList.Any())
            return new List<AuditLogDto>();

        // Buscar todos os usuários únicos
        var userIds = logsList.Select(l => l.UserId).Distinct().ToList();
        var userDict = new Dictionary<Guid, string>();
        
        foreach (var userId in userIds)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
                userDict[user.Id] = user.Name;
        }

        // Buscar todas as entidades únicas por tipo
        var entityIdsByType = logsList
            .GroupBy(l => l.Entity.ToLegacyString())
            .ToDictionary(g => g.Key, g => g.Select(l => l.EntityId).Distinct().ToList());

        var entityNames = new Dictionary<Guid, string>();

        foreach (var entityType in entityIdsByType)
        {
            switch (entityType.Key)
            {
                case "User":
                    foreach (var id in entityType.Value)
                    {
                        var user = await _userRepository.GetByIdAsync(id);
                        if (user != null)
                            entityNames[user.Id] = user.Name;
                    }
                    break;

                case "Partner":
                    foreach (var id in entityType.Value)
                    {
                        var partner = await _partnerRepository.GetByIdAsync(id);
                        if (partner != null)
                            entityNames[partner.Id] = partner.Name;
                    }
                    break;

                case "Business":
                    foreach (var id in entityType.Value)
                    {
                        var business = await _businessRepository.GetByIdAsync(id);
                        if (business != null)
                            entityNames[business.Id] = $"Negócio - R$ {business.Value:F2}";
                    }
                    break;

                case "Vector":
                    foreach (var id in entityType.Value)
                    {
                        var vector = await _vetorRepository.GetByIdAsync(id);
                        if (vector != null)
                            entityNames[vector.Id] = vector.Name;
                    }
                    break;
            }
        }

        // Converter para DTOs
        return logsList.Select(log =>
        {
            var userName = userDict.TryGetValue(log.UserId, out var name) ? name : "Usuário Desconhecido";
            var entityName = entityNames.TryGetValue(log.EntityId, out var entName) ? entName : null;
            return AuditLogDto.FromEntity(log, userName, entityName);
        }).ToList();
    }

    private record ValidationResult(bool IsValid, string ErrorMessage);
}