namespace Application.UseCases.GetBusinessPayments.DTO;

public sealed record GetBusinessPaymentsResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public BusinessPaymentsDto? BusinessPayments { get; init; }

    public static GetBusinessPaymentsResult Success(BusinessPaymentsDto businessPayments)
        => new() { IsSuccess = true, Message = "Pagamentos do negócio obtidos com sucesso.", BusinessPayments = businessPayments };

    public static GetBusinessPaymentsResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record BusinessPaymentsDto
{
    public Guid BusinessId { get; init; }
    public string BusinessDescription { get; init; } = string.Empty;
    public decimal BusinessValue { get; init; }
    public DateTime BusinessDate { get; init; }
    public string BusinessStatus { get; init; } = string.Empty;
    public string PartnerName { get; init; } = string.Empty;
    public string BusinessTypeName { get; init; } = string.Empty;
    
    public decimal TotalCommission { get; init; }
    public IEnumerable<BusinessPaymentDto> Payments { get; init; } = Enumerable.Empty<BusinessPaymentDto>();
    
    public PaymentSummaryDto Summary { get; init; } = new();
}

public sealed record BusinessPaymentDto
{
    public Guid Id { get; init; }
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public string TipoPagamento { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? PaidOn { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed record PaymentSummaryDto
{
    public decimal TotalPago { get; init; }
    public decimal TotalPendente { get; init; }
    public decimal TotalCancelado { get; init; }
    public int QuantidadePagos { get; init; }
    public int QuantidadePendentes { get; init; }
    public int QuantidadeCancelados { get; init; }
}