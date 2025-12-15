using Domain.ValueObjects;
using Domain.ValueTypes;
using Domain.Extensions;

namespace Application.UseCases.ListPayments.DTO;

public sealed record ListPaymentsRequest
{
    public Guid? VetorId { get; init; }
    public Guid? PartnerId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Status { get; init; }
    public string? TipoPagamento { get; init; }
    public string SortBy { get; init; } = "createdAt";
    public string SortDirection { get; init; } = "desc";
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public bool IsValidStatus()
    {
        if (string.IsNullOrEmpty(Status)) return true;
        
        return Status == PaymentStatus.APagar.ToLegacyString() ||
               Status == PaymentStatus.Pago.ToLegacyString() ||
               Status == PaymentStatus.Cancelado.ToLegacyString();
    }

    public bool IsValidTipoPagamento()
    {
        if (string.IsNullOrEmpty(TipoPagamento)) return true;
        
        return Domain.Extensions.PaymentTypeExtensions.TryParse(TipoPagamento, out _);
    }
}