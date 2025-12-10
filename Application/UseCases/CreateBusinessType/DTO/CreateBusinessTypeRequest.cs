using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.CreateBusinessType.DTO;

public sealed record CreateBusinessTypeRequest
{
    /// <summary>
    /// Nome do tipo de negócio
    /// </summary>
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres.")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Descrição do tipo de negócio
    /// </summary>
    [Required(ErrorMessage = "Descrição é obrigatória.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Descrição deve ter entre 5 e 500 caracteres.")]
    public string Description { get; init; } = string.Empty;
}