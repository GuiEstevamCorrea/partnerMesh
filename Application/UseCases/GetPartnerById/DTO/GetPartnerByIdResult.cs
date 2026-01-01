using Domain.Entities;

namespace Application.UseCases.GetPartnerById.DTO;

public sealed record GetPartnerByIdResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public PartnerDetailDto? Partner { get; init; }

    public static GetPartnerByIdResult Success(PartnerDetailDto partner)
        => new() { IsSuccess = true, Message = "Parceiro encontrado com sucesso.", Partner = partner };

    public static GetPartnerByIdResult NotFound()
        => new() { IsSuccess = false, Message = "Parceiro não encontrado." };

    public static GetPartnerByIdResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record PartnerDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    
    // Informações do Vetor
    public Guid VetorId { get; init; }
    public string VetorName { get; init; } = string.Empty;
    public string VetorEmail { get; init; } = string.Empty;
    
    // Informações do Recomendador
    public Guid? RecommenderId { get; init; }
    public string? RecommenderName { get; init; }
    public string? RecommenderEmail { get; init; }
    public int Level { get; init; }
    
    // Estatísticas
    public int TotalRecommended { get; init; }
    public int ActiveRecommended { get; init; }
    public int InactiveRecommended { get; init; }
    
    // Lista de Recomendados
    public IEnumerable<RecommendedPartnerDto> RecommendedPartners { get; init; } = Enumerable.Empty<RecommendedPartnerDto>();

    public static PartnerDetailDto FromEntity(
        Partner partner, 
        Vetor vetor, 
        Partner? recommender = null,
        IEnumerable<Partner>? recommendedPartners = null,
        int level = 0)
    {
        var recommended = recommendedPartners?.ToList() ?? new List<Partner>();
        
        return new PartnerDetailDto
        {
            Id = partner.Id,
            Name = partner.Name,
            Email = partner.Email,
            PhoneNumber = partner.PhoneNumber,
            IsActive = partner.Active,
            CreatedAt = partner.CreatedAt,
            VetorId = vetor.Id,
            VetorName = vetor.Name,
            VetorEmail = vetor.Email,
            RecommenderId = recommender?.Id,
            RecommenderName = recommender?.Name,
            RecommenderEmail = recommender?.Email,
            Level = level,
            TotalRecommended = recommended.Count,
            ActiveRecommended = recommended.Count(p => p.Active),
            InactiveRecommended = recommended.Count(p => !p.Active),
            RecommendedPartners = recommended.Select(RecommendedPartnerDto.FromEntity)
        };
    }
}

public sealed record RecommendedPartnerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }

    public static RecommendedPartnerDto FromEntity(Partner partner)
    {
        return new RecommendedPartnerDto
        {
            Id = partner.Id,
            Name = partner.Name,
            Email = partner.Email,
            IsActive = partner.Active,
            CreatedAt = partner.CreatedAt
        };
    }
}