namespace Application.UseCases.UpdateBusiness.DTO;

public sealed record UpdateBusinessResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public UpdatedBusinessDto? Business { get; init; }
    public bool CommissionRecalculated { get; init; } = false;
    public string? CommissionInfo { get; init; }

    public static UpdateBusinessResult Success(UpdatedBusinessDto business, bool commissionRecalculated = false, string? commissionInfo = null)
        => new() 
        { 
            IsSuccess = true, 
            Message = "Negócio atualizado com sucesso.",
            Business = business,
            CommissionRecalculated = commissionRecalculated,
            CommissionInfo = commissionInfo
        };

    public static UpdateBusinessResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record UpdatedBusinessDto
{
    public Guid Id { get; init; }
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public Guid BusinessTypeId { get; init; }
    public string BusinessTypeName { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string Observations { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}