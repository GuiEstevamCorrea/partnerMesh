namespace Application.UseCases.UpdateVetor.DTO;

public sealed record UpdateVetorResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public VetorInfo? Vetor { get; init; }

    private UpdateVetorResult(bool isSuccess, string message, VetorInfo? vetor = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Vetor = vetor;
    }

    public static UpdateVetorResult Success(VetorInfo vetor)
        => new(true, "Vetor atualizado com sucesso.", vetor);

    public static UpdateVetorResult Failure(string message)
        => new(false, message);
}

public sealed record VetorInfo(
    Guid Id,
    string Name,
    string Email,
    bool Active,
    DateTime CreatedAt);