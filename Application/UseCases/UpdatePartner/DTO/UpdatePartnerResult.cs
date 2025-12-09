namespace Application.UseCases.UpdatePartner.DTO;

public sealed record UpdatePartnerResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public PartnerDto? Partner { get; init; }

    private UpdatePartnerResult(bool isSuccess, string message, PartnerDto? partner = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Partner = partner;
    }

    public static UpdatePartnerResult Success(PartnerDto partner, string message = "Parceiro atualizado com sucesso.")
        => new(true, message, partner);

    public static UpdatePartnerResult Failure(string message)
        => new(false, message);
}

public sealed record PartnerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool Active { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid VetorId { get; init; }
    public string VetorName { get; init; } = string.Empty;
    public Guid? RecommenderId { get; init; }
    public string? RecommenderName { get; init; }
}