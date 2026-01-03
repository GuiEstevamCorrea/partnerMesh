namespace Application.UseCases.GetPaymentsSummary.DTO;

public sealed record GetPaymentsSummaryRequest
{
    public Guid? VetorId { get; init; }
    public Guid? PartnerId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Status { get; init; }
    public string? TipoPagamento { get; init; }
}