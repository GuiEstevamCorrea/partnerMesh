namespace Application.UseCases.GetPartnerTree.DTO;

public sealed record GetPartnerTreeRequest
{
    /// <summary>
    /// ID do vetor para filtrar a árvore (opcional, se não informado usa o vetor do usuário atual)
    /// </summary>
    public Guid? VetorId { get; init; }

    /// <summary>
    /// ID do parceiro raiz para iniciar a árvore (opcional, se não informado mostra toda a árvore do vetor)
    /// </summary>
    public Guid? RootPartnerId { get; init; }

    /// <summary>
    /// Profundidade máxima da árvore (padrão: ilimitado)
    /// </summary>
    public int? MaxDepth { get; init; }

    /// <summary>
    /// Incluir apenas parceiros ativos (padrão: false - inclui todos)
    /// </summary>
    public bool OnlyActive { get; init; } = false;
}