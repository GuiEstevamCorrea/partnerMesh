namespace Application.UseCases.DeactivateVetor.DTO;

public sealed record DeactivateVetorResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public VetorInfo? Vetor { get; init; }

    private DeactivateVetorResult(bool isSuccess, string message, VetorInfo? vetor = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Vetor = vetor;
    }

    public static DeactivateVetorResult Success(VetorInfo vetor)
        => new(true, "Vetor inativado com sucesso.", vetor);

    public static DeactivateVetorResult Failure(string message)
        => new(false, message);
}

public sealed record VetorInfo(
    Guid Id,
    string Name,
    string Email,
    bool Active,
    DateTime CreatedAt);