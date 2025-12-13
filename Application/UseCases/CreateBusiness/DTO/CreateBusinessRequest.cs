using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.CreateBusiness.DTO;

public sealed record CreateBusinessRequest
{
    /// <summary>
    /// ID do parceiro que fechou o negócio
    /// </summary>
    [Required(ErrorMessage = "Parceiro é obrigatório.")]
    public Guid PartnerId { get; init; }

    /// <summary>
    /// ID do tipo de negócio
    /// </summary>
    [Required(ErrorMessage = "Tipo de negócio é obrigatório.")]
    public Guid BusinessTypeId { get; init; }

    /// <summary>
    /// Valor do negócio
    /// </summary>
    [Required(ErrorMessage = "Valor é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero.")]
    public decimal Value { get; init; }

    /// <summary>
    /// Observações sobre o negócio
    /// </summary>
    [StringLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres.")]
    public string Observations { get; init; } = string.Empty;

    /// <summary>
    /// Data do negócio (opcional, usará data atual se não informada)
    /// </summary>
    public DateTime? Date { get; init; }
}