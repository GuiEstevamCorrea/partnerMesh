namespace Application.UseCases.GetVetorById.DTO;

public sealed record GetVetorByIdResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public VetorDetailDto? Vetor { get; init; }

    private GetVetorByIdResult(bool isSuccess, string message, VetorDetailDto? vetor = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Vetor = vetor;
    }

    public static GetVetorByIdResult Success(VetorDetailDto vetor, string message = "Vetor obtido com sucesso.")
        => new(true, message, vetor);

    public static GetVetorByIdResult Failure(string message)
        => new(false, message);
}

public sealed record VetorDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool Active { get; init; }
    public DateTime CreatedAt { get; init; }
    public VetorStatisticsDto Statistics { get; init; } = new();
}

public sealed record VetorStatisticsDto
{
    public int TotalUsers { get; init; }
    public int TotalPartners { get; init; }
    public int ActivePartners { get; init; }
    public int InactivePartners { get; init; }
    public int TotalBusinesses { get; init; }
    public decimal TotalBusinessValue { get; init; }
    public decimal TotalCommissions { get; init; }
    public decimal PaidCommissions { get; init; }
    public decimal PendingCommissions { get; init; }
}