using Domain.Entities;

namespace Application.UseCases.CreateBusinessType.DTO;

public sealed record CreateBusinessTypeResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public BusinessTypeCreateDto? BusinessType { get; init; }

    public static CreateBusinessTypeResult Success(BusinessTypeCreateDto businessType)
        => new() { IsSuccess = true, Message = "Tipo de negócio criado com sucesso.", BusinessType = businessType };

    public static CreateBusinessTypeResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record BusinessTypeCreateDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Active { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModified { get; init; }
    public Guid CreatedBy { get; init; }
    public Guid? ModifiedBy { get; init; }

    public static BusinessTypeCreateDto FromEntity(BusinessType businessType)
    {
        return new BusinessTypeCreateDto
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