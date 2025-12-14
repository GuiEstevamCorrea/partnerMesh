namespace Application.UseCases.ProcessPayment.DTO;

public sealed record ProcessPaymentResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public PaymentProcessedDto? Payment { get; init; }

    public static ProcessPaymentResult Success(PaymentProcessedDto payment)
        => new() { IsSuccess = true, Message = "Pagamento efetuado com sucesso.", Payment = payment };

    public static ProcessPaymentResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record PaymentProcessedDto
{
    public Guid Id { get; init; }
    public Guid ComissionId { get; init; }
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    public string TipoPagamento { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? PaidOn { get; init; }
    public Guid ProcessedBy { get; init; }
    public string ProcessedByName { get; init; } = string.Empty;
}