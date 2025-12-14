using Domain.Entities;

namespace Application.UseCases.CreateBusiness.DTO;

public sealed record CreateBusinessResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public BusinessDto? Business { get; init; }
    public CommissionSummaryDto? CommissionSummary { get; init; }

    public static CreateBusinessResult Success(BusinessDto business, CommissionSummaryDto commissionSummary)
        => new() 
        { 
            IsSuccess = true, 
            Message = "Negócio criado com sucesso.", 
            Business = business,
            CommissionSummary = commissionSummary
        };

    public static CreateBusinessResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record BusinessDto
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

    public static BusinessDto FromEntity(Bussiness business, string partnerName = "", string businessTypeName = "")
    {
        return new BusinessDto
        {
            Id = business.Id,
            PartnerId = business.PartnerId,
            PartnerName = partnerName,
            BusinessTypeId = business.BussinessTypeId,
            BusinessTypeName = businessTypeName,
            Value = business.Value,
            Status = business.Status.ToLegacyString(),
            Date = business.Date,
            Observations = business.Observations,
            CreatedAt = business.CreatedAt
        };
    }
}

public sealed record CommissionSummaryDto
{
    public Guid CommissionId { get; init; }
    public decimal TotalCommissionValue { get; init; }
    public IEnumerable<CommissionPaymentDto> Payments { get; init; } = new List<CommissionPaymentDto>();
}

public sealed record CommissionPaymentDto
{
    public Guid Id { get; init; }
    public Guid ReceiverId { get; init; }
    public string ReceiverName { get; init; } = string.Empty;
    public string ReceiverType { get; init; } = string.Empty; // "vetor" ou "parceiro"
    public int Level { get; init; }
    public decimal Value { get; init; }
    public string Status { get; init; } = string.Empty;
}