namespace Domain.ValueTypes;

/// <summary>
/// Define os tipos de pagamento de comissão
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// Pagamento para o vetor
    /// </summary>
    Vetor = 1,

    /// <summary>
    /// Pagamento para o recomendador direto (nível 1)
    /// </summary>
    Recomendador = 2,

    /// <summary>
    /// Pagamento para o participante (você)
    /// </summary>
    Participante = 3,

    /// <summary>
    /// Pagamento para intermediários (níveis 2 e 3)
    /// </summary>
    Intermediario = 4
}
