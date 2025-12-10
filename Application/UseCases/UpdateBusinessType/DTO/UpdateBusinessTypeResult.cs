using Domain.Entities;

namespace Application.UseCases.UpdateBusinessType.DTO;

public class UpdateBusinessTypeResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }

    public static UpdateBusinessTypeResult From(BusinessType businessType)
    {
        return new UpdateBusinessTypeResult
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