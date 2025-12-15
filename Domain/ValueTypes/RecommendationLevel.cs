namespace Domain.ValueTypes;

/// <summary>
/// Define os níveis de recomendação na cadeia de parceiros
/// </summary>
public enum RecommendationLevel
{
    /// <summary>
    /// Nível 1 - Parceiro direto (recomendador imediato)
    /// Recebe 5% de comissão
    /// </summary>
    Level1 = 1,

    /// <summary>
    /// Nível 2 - Segundo nível da cadeia (recomendador do recomendador)
    /// Recebe 3% de comissão
    /// </summary>
    Level2 = 2,

    /// <summary>
    /// Nível 3 - Terceiro nível da cadeia (recomendador do recomendador do recomendador)
    /// Recebe 2% de comissão
    /// </summary>
    Level3 = 3
}
