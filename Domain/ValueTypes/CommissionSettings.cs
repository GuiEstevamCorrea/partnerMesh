namespace Domain.ValueTypes;

/// <summary>
/// Configurações de comissão do sistema
/// Define os percentuais de comissão distribuídos na cadeia de recomendação
/// IMPORTANTE: Quem fecha o negócio NÃO recebe comissão, apenas recomendadores e vetor
/// </summary>
public class CommissionSettings
{
    /// <summary>
    /// Percentual total de comissão sobre o valor do negócio
    /// Padrão: 10% (0.10)
    /// </summary>
    public decimal TotalPercentage { get; init; } = 0.10m;

    /// <summary>
    /// Calcula a distribuição de percentuais para uma cadeia de recomendação
    /// </summary>
    /// <param name="chainLength">Número total de recomendadores na cadeia (EXCLUINDO quem fechou)</param>
    /// <returns>Array de percentuais, onde [0] é o Vetor e [chainLength-1] é o recomendador direto</returns>
    /// <remarks>
    /// Regras de distribuição (conforme documento do projeto):
    /// - Nível 1: Vetor 50% + Level 1 50%
    /// - Nível 2: Vetor 15% + Level 1 35% + Level 2 Recomendador direto 50%
    /// - Nível 3: Vetor 10% + Level 1 15% + Nível 2 25% + Nível 3 Recomendador direto 50%
    /// - Nível 4+: Vetor 10% + Level 1 0% + Nível 2 15% + Nível 3 25% + Nível 4+ Recomendador direto 50%
    /// - O recomendador direto (mais próximo de quem fechou) sempre recebe 50%
    /// - Quem fecha o negócio NÃO recebe comissão
    /// </remarks>
    public decimal[] CalculateDistribution(int chainLength)
    {
        if (chainLength < 0)
            throw new ArgumentException("Cadeia não pode ser negativa", nameof(chainLength));

        var distribution = new decimal[chainLength];

        if (chainLength == 0)
        {
            // Sem recomendadores: ninguém recebe comissão
            return new decimal[0];
        }
        else if (chainLength == 1)
        {
            // Nível 1: Apenas Vetor recebe 100%
            distribution[0] = 1.0m; // Vetor
        }
        else if (chainLength == 2)
        {
            // Nível 1: Vetor 50% + Level 1 50%
            distribution[0] = 0.50m; // Vetor
            distribution[1] = 0.50m; // Level 1 (recomendador direto)
        }
        else if (chainLength == 3)
        {
            // Nível 2: Vetor 15% + Level 1 35% + Level 2 Recomendador direto 50%
            distribution[0] = 0.15m; // Vetor
            distribution[1] = 0.35m; // Level 1
            distribution[2] = 0.50m; // Level 2 Recomendador direto
        }
        else if (chainLength == 4)
        {
            // Nível 3: Vetor 10% + Level 1 15% + Nível 2 25% + Nível 3 Recomendador direto 50%
            distribution[0] = 0.10m; // Vetor
            distribution[1] = 0.15m; // Level 1
            distribution[2] = 0.25m; // Nível 2
            distribution[3] = 0.50m; // Nível 3 Recomendador direto
        }
        else // chainLength >= 5
        {
            // Nível 4+: Vetor 10% + Level 1 0% + Nível 2 15% + Nível 3 25% + Nível 4+ Recomendador direto 50%
            distribution[0] = 0.10m; // Vetor
            distribution[1] = 0.00m; // Level 1 (não recebe a partir do nível 4)
            distribution[2] = 0.15m; // Nível 2
            distribution[3] = 0.25m; // Nível 3
            
            // O recomendador direto (último da cadeia) sempre recebe 50%
            distribution[chainLength - 1] = 0.50m;
            
            // Intermediários entre Nível 3 e o recomendador direto recebem 0%
            for (int i = 4; i < chainLength - 1; i++)
            {
                distribution[i] = 0.00m;
            }
        }


        return distribution;
    }

    /// <summary>
    /// Valida se os percentuais estão configurados corretamente
    /// </summary>
    public bool IsValid()
    {
        return TotalPercentage > 0 &&
               TotalPercentage <= 1.0m;
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
