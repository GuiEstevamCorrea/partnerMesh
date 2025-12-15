namespace Domain.Extensions;

using Domain.ValueTypes;

public static class RecommendationLevelExtensions
{
    /// <summary>
    /// Converte um inteiro para RecommendationLevel
    /// </summary>
    public static RecommendationLevel ToRecommendationLevel(this int level)
    {
        return level switch
        {
            1 => RecommendationLevel.Level1,
            2 => RecommendationLevel.Level2,
            3 => RecommendationLevel.Level3,
            _ => throw new ArgumentException($"Nível de recomendação inválido: {level}. Deve ser 1, 2 ou 3.", nameof(level))
        };
    }

    /// <summary>
    /// Tenta converter um inteiro para RecommendationLevel
    /// </summary>
    public static bool TryToRecommendationLevel(this int level, out RecommendationLevel recommendationLevel)
    {
        recommendationLevel = RecommendationLevel.Level1;
        
        if (level < 1 || level > 3)
            return false;

        recommendationLevel = level.ToRecommendationLevel();
        return true;
    }

    /// <summary>
    /// Converte RecommendationLevel para inteiro
    /// </summary>
    public static int ToInt(this RecommendationLevel level)
    {
        return (int)level;
    }
}
