namespace Application.UseCases.CancelBusiness.DTO;

public sealed record CancelBusinessResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public CancelledBusinessDto? Business { get; init; }
    public CommissionCancellationSummary? CommissionSummary { get; init; }

    public static CancelBusinessResult Success(
        CancelledBusinessDto business, 
        CommissionCancellationSummary commissionSummary)
        => new() 
        { 
            IsSuccess = true, 
            Message = "Negócio cancelado com sucesso.",
            Business = business,
            CommissionSummary = commissionSummary
        };

    public static CancelBusinessResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record CancelledBusinessDto
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
    public string CancellationReason { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime CancelledAt { get; init; }
}

public sealed record CommissionCancellationSummary
{
    public Guid CommissionId { get; init; }
    public decimal OriginalTotalValue { get; init; }
    public int TotalPayments { get; init; }
    public int PendingPaymentsCancelled { get; init; }
    public int PaidPaymentsKept { get; init; }
    public decimal PendingValueCancelled { get; init; }
    public decimal PaidValueKept { get; init; }
    public IEnumerable<PaymentCancellationDetail> PaymentDetails { get; init; } = new List<PaymentCancellationDetail>();
}

public sealed record PaymentCancellationDetail
{
    public Guid PaymentId { get; init; }
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public string PaymentType { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string OriginalStatus { get; init; } = string.Empty;
    public string FinalStatus { get; init; } = string.Empty;
    public bool WasCancelled { get; init; }
    public string CancellationNote { get; init; } = string.Empty;
}