using Domain.Entities;

namespace Application.UseCases.ListBusinessTypes.DTO;

public sealed record ListBusinessTypesResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public IEnumerable<BusinessTypeDto> BusinessTypes { get; init; } = Enumerable.Empty<BusinessTypeDto>();
    public PaginationInfo Pagination { get; init; } = new();

    public static ListBusinessTypesResult Success(IEnumerable<BusinessTypeDto> businessTypes, PaginationInfo pagination)
        => new() { IsSuccess = true, Message = "Tipos de negócio listados com sucesso.", BusinessTypes = businessTypes, Pagination = pagination };

    public static ListBusinessTypesResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record BusinessTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Active { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModified { get; init; }
    public Guid CreatedBy { get; init; }
    public Guid? ModifiedBy { get; init; }

    public static BusinessTypeDto FromEntity(BusinessType businessType)
    {
        return new BusinessTypeDto
        {
            Id = businessType.Id,
            Name = businessType.Name,
            Description = businessType.Description,
            Active = businessType.Active,
            CreatedAt = businessType.CreatedAt,
            LastModified = businessType.LastModified,
            CreatedBy = businessType.CreatedBy,
            ModifiedBy = businessType.ModifiedBy
        };
    }
}

public sealed record PaginationInfo
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}