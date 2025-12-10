namespace Application.UseCases.GetPartnerById.DTO;

public sealed record GetPartnerByIdRequest
{
    /// <summary>
    /// ID do parceiro a ser buscado
    /// </summary>
    public Guid PartnerId { get; init; }
}