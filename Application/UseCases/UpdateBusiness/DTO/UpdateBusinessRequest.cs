using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.UpdateBusiness.DTO;

public sealed record UpdateBusinessRequest
{
    /// <summary>
    /// Valor do negócio - somente valores positivos
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor do negócio deve ser positivo")]
    public decimal? Value { get; init; }

    /// <summary>
    /// Observações sobre o negócio - máximo 1000 caracteres
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
    public string? Observations { get; init; }

    /// <summary>
    /// Verifica se há pelo menos um campo para atualizar
    /// </summary>
    public bool HasAnyFieldToUpdate()
    {
        return Value.HasValue || !string.IsNullOrEmpty(Observations);
    }
}