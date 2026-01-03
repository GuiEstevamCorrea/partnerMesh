namespace Application.UseCases.GetPaymentsSummary.DTO;

public sealed record GetPaymentsSummaryResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public PaymentsSummaryDto? Summary { get; init; }

    public static GetPaymentsSummaryResult Success(PaymentsSummaryDto summary)
        => new() { IsSuccess = true, Message = "Resumo de pagamentos obtido com sucesso.", Summary = summary };

    public static GetPaymentsSummaryResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record PaymentsSummaryDto
{
    public decimal TotalPaid { get; init; }
    public decimal TotalPending { get; init; }
    public decimal TotalCancelled { get; init; }
    public int CountPaid { get; init; }
    public int CountPending { get; init; }
    public int CountCancelled { get; init; }
    public decimal TotalValue => TotalPaid + TotalPending + TotalCancelled;
    public int TotalCount => CountPaid + CountPending + CountCancelled;
}