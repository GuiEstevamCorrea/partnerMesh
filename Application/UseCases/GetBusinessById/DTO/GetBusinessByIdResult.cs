namespace Application.UseCases.GetBusinessById.DTO;

public sealed record GetBusinessByIdResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public BusinessDetailDto? Business { get; init; }

    public static GetBusinessByIdResult Success(BusinessDetailDto business)
        => new() 
        { 
            IsSuccess = true, 
            Message = "Negócio encontrado com sucesso.",
            Business = business
        };

    public static GetBusinessByIdResult NotFound()
        => new() { IsSuccess = false, Message = "Negócio não encontrado" };

    public static GetBusinessByIdResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record BusinessDetailDto
{
    public Guid Id { get; init; }
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public string PartnerEmail { get; init; } = string.Empty;
    public string PartnerPhone { get; init; } = string.Empty;
    public Guid BusinessTypeId { get; init; }
    public string BusinessTypeName { get; init; } = string.Empty;
    public string BusinessTypeDescription { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string Observations { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? CancellationReason { get; init; }
    public DateTime? CancelledAt { get; init; }
    public DetailedCommissionInfo Commission { get; init; } = new();
}

public sealed record DetailedCommissionInfo
{
    public Guid CommissionId { get; init; }
    public decimal TotalValue { get; init; }
    public DateTime CreatedAt { get; init; }
    public int TotalPayments { get; init; }
    public int PaidPayments { get; init; }
    public int PendingPayments { get; init; }
    public int CancelledPayments { get; init; }
    public decimal TotalPaidValue { get; init; }
    public decimal TotalPendingValue { get; init; }
    public decimal TotalCancelledValue { get; init; }
    public string CommissionStatus { get; init; } = string.Empty;
    public IEnumerable<CommissionPaymentDetailDto> Payments { get; init; } = new List<CommissionPaymentDetailDto>();
}

public sealed record CommissionPaymentDetailDto
{
    public Guid PaymentId { get; init; }
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public string PaymentType { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? PaidOn { get; init; }
    public string Level { get; init; } = string.Empty; // "Nível 1", "Nível 2", "Nível 3", "Vetor"
}