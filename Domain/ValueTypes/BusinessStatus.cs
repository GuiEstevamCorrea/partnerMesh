namespace Domain.ValueTypes;

/// <summary>
/// Define os possíveis status de um negócio
/// </summary>
public enum BusinessStatus
{
    /// <summary>
    /// Negócio ativo (sem pagamentos ainda)
    /// </summary>
    Ativo = 1,

    /// <summary>
    /// Negócio cancelado
    /// </summary>
    Cancelado = 2,

    /// <summary>
    /// Negócio com pagamentos parciais
    /// </summary>
    ParcialmentePago = 3,

    /// <summary>
    /// Negócio com todos os pagamentos realizados
    /// </summary>
    TotalmentePago = 4
}
