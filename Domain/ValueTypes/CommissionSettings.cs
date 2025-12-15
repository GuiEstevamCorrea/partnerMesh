namespace Domain.ValueTypes;

/// <summary>
/// Configurações de comissão do sistema
/// Define os percentuais de comissão distribuídos na cadeia de recomendação
/// </summary>
public class CommissionSettings
{
    /// <summary>
    /// Percentual total de comissão sobre o valor do negócio
    /// Padrão: 10% (0.10)
    /// </summary>
    public decimal TotalPercentage { get; init; } = 0.10m;

    /// <summary>
    /// Percentual para parceiro de nível 1 (recomendador direto)
    /// Padrão: 5% (0.05)
    /// </summary>
    public decimal Level1Percentage { get; init; } = 0.05m;

    /// <summary>
    /// Percentual para parceiro de nível 2 (segundo nível da cadeia)
    /// Padrão: 3% (0.03)
    /// </summary>
    public decimal Level2Percentage { get; init; } = 0.03m;

    /// <summary>
    /// Percentual para parceiro de nível 3 (terceiro nível da cadeia)
    /// Padrão: 2% (0.02)
    /// </summary>
    public decimal Level3Percentage { get; init; } = 0.02m;

    /// <summary>
    /// Retorna o percentual de comissão para um determinado nível
    /// </summary>
    /// <param name="level">Nível de recomendação</param>
    /// <returns>Percentual de comissão para o nível especificado</returns>
    public decimal GetPercentageForLevel(RecommendationLevel level)
    {
        return level switch
        {
            RecommendationLevel.Level1 => Level1Percentage,
            RecommendationLevel.Level2 => Level2Percentage,
            RecommendationLevel.Level3 => Level3Percentage,
            _ => 0m
        };
    }

    /// <summary>
    /// Retorna o percentual de comissão para um determinado nível (versão int)
    /// </summary>
    /// <param name="level">Nível de recomendação (1, 2 ou 3)</param>
    /// <returns>Percentual de comissão para o nível especificado</returns>
    public decimal GetPercentageForLevel(int level)
    {
        return level switch
        {
            1 => Level1Percentage,
            2 => Level2Percentage,
            3 => Level3Percentage,
            _ => 0m
        };
    }

    /// <summary>
    /// Valida se os percentuais estão configurados corretamente
    /// </summary>
    public bool IsValid()
    {
        // A soma dos percentuais individuais deve ser igual ao percentual total
        var sum = Level1Percentage + Level2Percentage + Level3Percentage;
        return Math.Abs(sum - TotalPercentage) < 0.0001m &&
               TotalPercentage > 0 &&
               Level1Percentage >= 0 &&
               Level2Percentage >= 0 &&
               Level3Percentage >= 0;
    }

    /// <summary>
    /// Instância padrão das configurações de comissão
    /// </summary>
    /// <remarks>
    /// Futuramente, esta configuração pode ser:
    /// - Carregada de um arquivo de configuração (appsettings.json)
    /// - Armazenada em banco de dados para configuração dinâmica
    /// - Diferente por Vetor, permitindo políticas de comissão customizadas
    /// - Versionada para manter histórico de mudanças
    /// </remarks>
    public static CommissionSettings Default => new();
}
