using Domain.Entities;

namespace Application.UseCases.GetBusinessTypeById.DTO;

public sealed record GetBusinessTypeByIdResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public BusinessTypeDetailDto? BusinessType { get; init; }

    public static GetBusinessTypeByIdResult Success(BusinessTypeDetailDto businessType)
        => new() { IsSuccess = true, Message = "Tipo de negócio encontrado com sucesso.", BusinessType = businessType };

    public static GetBusinessTypeByIdResult NotFound()
        => new() { IsSuccess = false, Message = "Tipo de negócio não encontrado." };

    public static GetBusinessTypeByIdResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record BusinessTypeDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Active { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModified { get; init; }
    public Guid CreatedBy { get; init; }
    public Guid? ModifiedBy { get; init; }

    public static BusinessTypeDetailDto FromEntity(BusinessType businessType)
    {
        return new BusinessTypeDetailDto
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