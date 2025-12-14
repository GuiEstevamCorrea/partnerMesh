namespace Application.UseCases.ProcessPayment.DTO;

public sealed record ProcessPaymentRequest
{
    public Guid PaymentId { get; init; }
    public string? Observations { get; init; }

    public bool IsValid()
    {
        return PaymentId != Guid.Empty;
    }
}