namespace Domain.ValueTypes;

/// <summary>
/// Configurações de comissão do sistema
/// Define os percentuais de comissão distribuídos na cadeia de recomendação
/// Suporta níveis infinitos de recomendação com regras dinâmicas
/// </summary>
public class CommissionSettings
{
    /// <summary>
    /// Percentual total de comissão sobre o valor do negócio
    /// Padrão: 10% (0.10)
    /// </summary>
    public decimal TotalPercentage { get; init; } = 0.10m;

    /// <summary>
    /// Percentual fixo que quem fechou o negócio sempre recebe
    /// Padrão: 50% (0.50) do total da comissão
    /// </summary>
    public decimal CloserPercentage { get; init; } = 0.50m;

    /// <summary>
    /// Calcula a distribuição de percentuais para uma cadeia de recomendação
    /// </summary>
    /// <param name="chainLength">Número total de pessoas na cadeia (incluindo quem fechou)</param>
    /// <returns>Array de percentuais, onde [0] é o Vetor e [chainLength-1] é quem fechou</returns>
    /// <remarks>
    /// Regras de distribuição:
    /// - Quem fecha sempre recebe 50%
    /// - 2 pessoas: Vetor 50% / Closer 50%
    /// - 3 pessoas: Vetor 15% / Finder1 35% / Closer 50%
    /// - 4 pessoas: Vetor 10% / Finder1 15% / Finder2 25% / Closer 50%
    /// - 5+ pessoas: Vetor 10% / Finder1 0% / Finder2 15% / Finder3 25% / ... / Closer 50%
    /// A partir do 4º nível, Finder1 não recebe mais (0%)
    /// </remarks>
    public decimal[] CalculateDistribution(int chainLength)
    {
        if (chainLength <= 0)
            throw new ArgumentException("Cadeia deve ter pelo menos 1 pessoa", nameof(chainLength));

        var distribution = new decimal[chainLength];

        // Quem fechou sempre recebe 50%
        distribution[chainLength - 1] = CloserPercentage;

        if (chainLength == 1)
        {
            // Apenas 1 pessoa (Vetor): recebe 100%
            distribution[0] = 1.0m;
        }
        else if (chainLength == 2)
        {
            // Vetor -> Closer: 50% / 50%
            distribution[0] = 0.50m; // Vetor
        }
        else if (chainLength == 3)
        {
            // Vetor -> Finder1 -> Closer: 15% / 35% / 50%
            distribution[0] = 0.15m; // Vetor
            distribution[1] = 0.35m; // Finder1
        }
        else if (chainLength == 4)
        {
            // Vetor -> Finder1 -> Finder2 -> Closer: 10% / 15% / 25% / 50%
            distribution[0] = 0.10m; // Vetor
            distribution[1] = 0.15m; // Finder1
            distribution[2] = 0.25m; // Finder2
        }
        else // chainLength >= 5
        {
            // CORRENTE DINÂMICA: Os percentuais "andam" para frente conforme novos níveis entram
            
            // Vetor sempre recebe 10% a partir do 4º nível
            distribution[0] = 0.10m;

            // Todos os intermediários entre Vetor e os 2 últimos ativos recebem 0%
            // Os 2 últimos antes do Closer sempre recebem 15% e 25%
            for (int i = 1; i < chainLength - 2; i++)
            {
                distribution[i] = 0.00m;
            }

            distribution[chainLength - 3] = 0.15m;
            distribution[chainLength - 2] = 0.25m;
        }

        return distribution;
    }

    /// <summary>
    /// Valida se os percentuais estão configurados corretamente
    /// </summary>
    public bool IsValid()
    {
        return TotalPercentage > 0 &&
               TotalPercentage <= 1.0m &&
               CloserPercentage > 0 &&
               CloserPercentage <= 1.0m;
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
