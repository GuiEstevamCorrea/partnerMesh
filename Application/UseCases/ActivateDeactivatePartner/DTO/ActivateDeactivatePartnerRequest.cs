using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.ActivateDeactivatePartner.DTO;

public sealed record ActivateDeactivatePartnerRequest
{
    public Guid PartnerId { get; set; }
    
    [Required(ErrorMessage = "Status ativo é obrigatório.")]
    public bool Active { get; init; }

    public string? Reason { get; init; }
}