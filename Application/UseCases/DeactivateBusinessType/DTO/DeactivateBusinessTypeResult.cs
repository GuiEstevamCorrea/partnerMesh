using Domain.Entities;

namespace Application.UseCases.DeactivateBusinessType.DTO;

public class DeactivateBusinessTypeResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }

    public static DeactivateBusinessTypeResult Success(BusinessType businessType)
    {
        return new DeactivateBusinessTypeResult
        {
            IsSuccess = true,
            Message = businessType.Active ? "Tipo de negócio ativado com sucesso." : "Tipo de negócio inativado com sucesso.",
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

    public static DeactivateBusinessTypeResult Failure(string message)
    {
        return new DeactivateBusinessTypeResult
        {
            IsSuccess = false,
            Message = message
        };
    }

    // Manter compatibilidade com código existente
    public static DeactivateBusinessTypeResult From(BusinessType businessType) => Success(businessType);
}