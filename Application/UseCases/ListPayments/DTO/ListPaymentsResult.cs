using Application.UseCases.ListPartners.DTO;
using System.Text.Json.Serialization;

namespace Application.UseCases.ListPayments.DTO;

public sealed record ListPaymentsResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public IEnumerable<PaymentListDto> Payments { get; init; } = Enumerable.Empty<PaymentListDto>();
    public PaginationInfo Pagination { get; init; } = new();

    public static ListPaymentsResult Success(IEnumerable<PaymentListDto> payments, PaginationInfo pagination)
        => new() { IsSuccess = true, Message = "Pagamentos listados com sucesso.", Payments = payments, Pagination = pagination };

    public static ListPaymentsResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record PaymentListDto
{
    public Guid Id { get; init; }
    public Guid ComissionId { get; init; }
    public Guid PartnerId { get; init; }
    public string PartnerName { get; init; } = string.Empty;
    
    // Campos para compatibilidade com frontend
    public Guid RecipientId { get; init; }
    public string RecipientName { get; init; } = string.Empty;
    public string RecipientType { get; init; } = string.Empty;
    
    public string TipoPagamento { get; init; } = string.Empty;
    public int Level { get; init; }
    public decimal Value { get; init; }
    public string Status { get; init; } = string.Empty;
    
    [JsonPropertyName("paidOn")]
    public DateTime? PaidOn { get; init; }
    
    [JsonPropertyName("paidAt")]
    public DateTime? PaidAt => PaidOn;
    
    public DateTime CreatedAt { get; init; }
    
    // Dados do negócio associado
    public Guid BusinessId { get; init; }
    public string BusinessDescription { get; init; } = string.Empty;
    public decimal BusinessTotalValue { get; init; }
    public DateTime BusinessDate { get; init; }
    public string BusinessStatus { get; init; } = string.Empty;
    
    // Dados do vetor
    public Guid VetorId { get; init; }
    public string VetorName { get; init; } = string.Empty;
}