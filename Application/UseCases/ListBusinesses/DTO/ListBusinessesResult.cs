namespace Application.UseCases.ListBusinesses.DTO;

public sealed record ListBusinessesResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public IEnumerable<BusinessListDto> Businesses { get; init; } = new List<BusinessListDto>();
    public PaginationInfo Pagination { get; init; } = new();
    public BusinessSummary Summary { get; init; } = new();

    public static ListBusinessesResult Success(
        IEnumerable<BusinessListDto> businesses,
        PaginationInfo pagination,
        BusinessSummary summary)
        => new()
        {
            IsSuccess = true,
            Message = "Negócios listados com sucesso.",
            Businesses = businesses,
            Pagination = pagination,
            Summary = summary
        };

    public static ListBusinessesResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record BusinessListDto
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
    public DateTime CreatedAt { get; init; }
    public CommissionInfo Commission { get; init; } = new();
}

public sealed record CommissionInfo
{
    public Guid CommissionId { get; init; }
    public decimal TotalValue { get; init; }
    public int TotalPayments { get; init; }
    public int PaidPayments { get; init; }
    public int PendingPayments { get; init; }
    public int CancelledPayments { get; init; }
    public decimal PaidValue { get; init; }
    public decimal PendingValue { get; init; }
    public string CommissionStatus { get; init; } = string.Empty; // "Totalmente Pago", "Parcialmente Pago", "Pendente", "Cancelado"
}

public sealed record PaginationInfo
{
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public bool HasPrevious { get; init; }
    public bool HasNext { get; init; }
}

public sealed record BusinessSummary
{
    public int TotalBusinesses { get; init; }
    public int ActiveBusinesses { get; init; }
    public int CancelledBusinesses { get; init; }
    public decimal TotalValue { get; init; }
    public decimal TotalCommissions { get; init; }
    public decimal PaidCommissions { get; init; }
    public decimal PendingCommissions { get; init; }
}