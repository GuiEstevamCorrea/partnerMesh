namespace Domain.ValueTypes;

/// <summary>
/// Define os possíveis status de um pagamento de comissão
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Pagamento pendente (a pagar)
    /// </summary>
    APagar = 1,

    /// <summary>
    /// Pagamento realizado
    /// </summary>
    Pago = 2,

    /// <summary>
    /// Pagamento cancelado
    /// </summary>
    Cancelado = 3
}
