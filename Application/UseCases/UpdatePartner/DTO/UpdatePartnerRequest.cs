using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.UpdatePartner.DTO;

public sealed record UpdatePartnerRequest
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Telefone é obrigatório.")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres.")]
    public string PhoneNumber { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido.")]
    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres.")]
    public string Email { get; init; } = string.Empty;

    public Guid? RecommenderId { get; init; }
}