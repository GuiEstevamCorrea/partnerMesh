using Domain.Entities;

namespace Application.UseCases.ListPartners.DTO;

public sealed record ListPartnersResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public IEnumerable<PartnerDto> Partners { get; init; } = Enumerable.Empty<PartnerDto>();
    public PaginationInfo Pagination { get; init; } = new();

    public static ListPartnersResult Success(IEnumerable<PartnerDto> partners, PaginationInfo pagination)
        => new() { IsSuccess = true, Message = "Parceiros listados com sucesso.", Partners = partners, Pagination = pagination };

    public static ListPartnersResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record PartnerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public Guid VetorId { get; init; }
    public string VetorName { get; init; } = string.Empty;
    public Guid? RecommenderId { get; init; }
    public string? RecommenderName { get; init; }
    public DateTime CreatedAt { get; init; }

    public static PartnerDto FromEntity(Partner partner, string vetorName = "", string? recommenderName = null)
    {
        return new PartnerDto
        {
            Id = partner.Id,
            Name = partner.Name,
            Email = partner.Email,
            PhoneNumber = partner.PhoneNumber,
            IsActive = partner.Active,
            VetorId = partner.VetorId,
            VetorName = vetorName,
            RecommenderId = partner.RecommenderId,
            RecommenderName = recommenderName,
            CreatedAt = partner.CreatedAt
        };
    }
}

public sealed record PaginationInfo
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}