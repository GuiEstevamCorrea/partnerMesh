using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.CancelBusiness.DTO;

public sealed record CancelBusinessRequest
{
    /// <summary>
    /// Motivo do cancelamento do negócio
    /// </summary>
    [Required(ErrorMessage = "O motivo do cancelamento é obrigatório")]
    [MaxLength(500, ErrorMessage = "O motivo deve ter no máximo 500 caracteres")]
    public string CancellationReason { get; init; } = string.Empty;

    /// <summary>
    /// Forçar cancelamento mesmo com pagamentos já efetuados (opcional)
    /// </summary>
    public bool ForceCancel { get; init; } = false;
}