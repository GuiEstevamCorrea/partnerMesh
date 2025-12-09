namespace Application.UseCases.CreateVetor.DTO;

public sealed record CreateVetorResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public VetorInfo? Vetor { get; init; }

    private CreateVetorResult(bool isSuccess, string message, VetorInfo? vetor = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Vetor = vetor;
    }

    public static CreateVetorResult Success(VetorInfo vetor)
        => new(true, "Vetor criado com sucesso.", vetor);

    public static CreateVetorResult Failure(string message)
        => new(false, message);
}

public sealed record VetorInfo(
    Guid Id,
    string Name,
    string Email,
    bool Active,
    DateTime CreatedAt);