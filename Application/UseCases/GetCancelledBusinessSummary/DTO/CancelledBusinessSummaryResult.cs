namespace Application.UseCases.GetCancelledBusinessSummary.DTO;

public sealed record CancelledBusinessSummaryResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public CancelledBusinessSummaryDto? Summary { get; init; }

    public static CancelledBusinessSummaryResult Success(CancelledBusinessSummaryDto summary)
        => new() { IsSuccess = true, Message = "Resumo de negócios cancelados obtido com sucesso.", Summary = summary };

    public static CancelledBusinessSummaryResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record CancelledBusinessSummaryDto
{
    public int TotalCancelledBusinesses { get; init; }
    public decimal TotalCancelledValue { get; init; }
    public decimal TotalCancelledCommissions { get; init; }
    public decimal TotalCancelledPayments { get; init; }
    public int CancelledPaymentsCount { get; init; }
    public decimal PaidBeforeCancellation { get; init; }
    public int PaidBeforeCancellationCount { get; init; }
}