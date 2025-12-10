using Domain.Entities;

namespace Application.UseCases.GetPartnerTree.DTO;

public sealed record GetPartnerTreeResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public PartnerTreeDto? Tree { get; init; }
    public TreeStatistics Statistics { get; init; } = new();

    public static GetPartnerTreeResult Success(PartnerTreeDto tree, TreeStatistics statistics)
        => new() { IsSuccess = true, Message = "Árvore de parceiros obtida com sucesso.", Tree = tree, Statistics = statistics };

    public static GetPartnerTreeResult Failure(string message)
        => new() { IsSuccess = false, Message = message };
}

public sealed record PartnerTreeDto
{
    /// <summary>
    /// Informações do vetor
    /// </summary>
    public VetorInfo Vetor { get; init; } = new();

    /// <summary>
    /// Parceiros raiz (recomendados diretamente pelo vetor)
    /// </summary>
    public IEnumerable<PartnerTreeNodeDto> RootPartners { get; init; } = Enumerable.Empty<PartnerTreeNodeDto>();

    /// Órfãos: parceiros que têm recomendador que não existe mais ou está inativo
    /// </summary>
    public IEnumerable<PartnerTreeNodeDto> OrphanPartners { get; init; } = Enumerable.Empty<PartnerTreeNodeDto>();
}

public sealed record VetorInfo
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }

    public static VetorInfo FromEntity(Vetor vetor)
    {
        return new VetorInfo
        {
            Id = vetor.Id,
            Name = vetor.Name,
            Email = vetor.Email,
            IsActive = vetor.Active
        };
    }
}

public sealed record PartnerTreeNodeDto
{
    /// <summary>
    /// Dados do parceiro
    /// </summary>
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Nível na árvore (0 = recomendado direto do vetor)
    /// </summary>
    public int Level { get; init; }

    /// <summary>
    /// ID do recomendador
    /// </summary>
    public Guid? RecommenderId { get; init; }

    /// <summary>
    /// Nome do recomendador
    /// </summary>
    public string? RecommenderName { get; init; }

    /// <summary>
    /// Parceiros recomendados por este parceiro
    /// </summary>
    public IEnumerable<PartnerTreeNodeDto> Children { get; init; } = Enumerable.Empty<PartnerTreeNodeDto>();

    /// <summary>
    /// Estatísticas deste nó
    /// </summary>
    public NodeStatistics Stats { get; init; } = new();

    public static PartnerTreeNodeDto FromEntity(
        Partner partner, 
        int level, 
        string? recommenderName = null, 
        IEnumerable<PartnerTreeNodeDto>? children = null)
    {
        var childrenList = children?.ToList() ?? new List<PartnerTreeNodeDto>();
        
        return new PartnerTreeNodeDto
        {
            Id = partner.Id,
            Name = partner.Name,
            Email = partner.Email,
            PhoneNumber = partner.PhoneNumber,
            IsActive = partner.Active,
            CreatedAt = partner.CreatedAt,
            Level = level,
            RecommenderId = partner.RecommenderId,
            RecommenderName = recommenderName,
            Children = childrenList,
            Stats = new NodeStatistics
            {
                DirectChildren = childrenList.Count,
                ActiveDirectChildren = childrenList.Count(c => c.IsActive),
                TotalDescendants = CalculateTotalDescendants(childrenList),
                ActiveDescendants = CalculateActiveDescendants(childrenList),
                MaxDepth = CalculateMaxDepth(childrenList)
            }
        };
    }

    private static int CalculateTotalDescendants(IEnumerable<PartnerTreeNodeDto> children)
    {
        var childrenList = children.ToList();
        return childrenList.Count + childrenList.Sum(c => CalculateTotalDescendants(c.Children));
    }

    private static int CalculateActiveDescendants(IEnumerable<PartnerTreeNodeDto> children)
    {
        var childrenList = children.ToList();
        return childrenList.Count(c => c.IsActive) + childrenList.Sum(c => CalculateActiveDescendants(c.Children));
    }

    private static int CalculateMaxDepth(IEnumerable<PartnerTreeNodeDto> children)
    {
        var childrenList = children.ToList();
        return childrenList.Any() ? 1 + childrenList.Max(c => CalculateMaxDepth(c.Children)) : 0;
    }
}

public sealed record NodeStatistics
{
    /// <summary>
    /// Número de filhos diretos
    /// </summary>
    public int DirectChildren { get; init; }

    /// <summary>
    /// Número de filhos diretos ativos
    /// </summary>
    public int ActiveDirectChildren { get; init; }

    /// <summary>
    /// Total de descendentes (todos os níveis)
    /// </summary>
    public int TotalDescendants { get; init; }

    /// <summary>
    /// Total de descendentes ativos
    /// </summary>
    public int ActiveDescendants { get; init; }

    /// <summary>
    /// Profundidade máxima da subárvore
    /// </summary>
    public int MaxDepth { get; init; }
}

public sealed record TreeStatistics
{
    /// <summary>
    /// Total de parceiros na árvore
    /// </summary>
    public int TotalPartners { get; init; }

    /// <summary>
    /// Total de parceiros ativos
    /// </summary>
    public int ActivePartners { get; init; }

    /// <summary>
    /// Total de parceiros inativos
    /// </summary>
    public int InactivePartners { get; init; }

    /// <summary>
    /// Parceiros por nível
    /// </summary>
    public Dictionary<int, int> PartnersByLevel { get; init; } = new();

    /// <summary>
    /// Número de parfãos
    /// </summary>
    public int OrphanPartners { get; init; }

    /// <summary>
    /// Profundidade máxima da árvore
    /// </summary>
    public int MaxDepth { get; init; }
}