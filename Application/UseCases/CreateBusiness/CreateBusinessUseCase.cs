using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.CreateBusiness.DTO;
using Domain.Entities;
using Domain.Extensions;
using Domain.ValueObjects;
using Domain.ValueTypes;

namespace Application.UseCases.CreateBusiness;

public class CreateBusinessUseCase : ICreateBusinessUseCase
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly CommissionSettings _commissionSettings;

    public CreateBusinessUseCase(
        IBusinessRepository businessRepository,
        ICommissionRepository commissionRepository,
        IPartnerRepository partnerRepository,
        IBusinessTypeRepository businessTypeRepository)
    {
        _businessRepository = businessRepository;
        _commissionRepository = commissionRepository;
        _partnerRepository = partnerRepository;
        _businessTypeRepository = businessTypeRepository;
        _commissionSettings = CommissionSettings.Default;
    }

    public async Task<CreateBusinessResult> ExecuteAsync(CreateBusinessRequest request, Guid userId)
    {
        // Validar se o partner existe
        var partner = await _partnerRepository.GetByIdAsync(request.PartnerId);
        if (partner == null)
            throw new ArgumentException("Partner não encontrado", nameof(request.PartnerId));

        // Validar se o tipo de business existe
        var businessType = await _businessTypeRepository.GetByIdAsync(request.BusinessTypeId);
        if (businessType == null)
            throw new ArgumentException("Tipo de negócio não encontrado", nameof(request.BusinessTypeId));

        // Criar o negócio
        var business = new Bussiness(
            request.PartnerId,
            request.BusinessTypeId,
            request.Value,
            request.Observations);

        await _businessRepository.AddAsync(business);

        // Calcular e criar comissão (10% do valor do negócio)
        var commission = await CreateCommissionAsync(business, partner);

        var result = CreateBusinessResult.Success(
            new BusinessDto
            {
                Id = business.Id,
                PartnerId = business.PartnerId,
                BusinessTypeId = business.BussinessTypeId,
                Value = business.Value,
                Observations = business.Observations,
                CreatedAt = business.CreatedAt,
                Status = business.Status.ToLegacyString(),
                Date = business.Date
            },
            new CommissionSummaryDto
            {
                CommissionId = commission.Id,
                TotalCommissionValue = commission.TotalValue,
                Payments = commission.Pagamentos.Select(p => new CommissionPaymentDto
                {
                    Id = p.Id,
                    ReceiverId = p.PartnerId,
                    ReceiverType = p.TipoPagamento.ToLegacyString(),
                    Value = p.Value,
                    Status = p.Status.ToLegacyString()
                }).ToList()
            }
        );

        return result;
    }

    private async Task<Comission> CreateCommissionAsync(Bussiness business, Partner partner)
    {
        // Calcular comissão usando configuração tipada
        var commissionValue = business.Value * _commissionSettings.TotalPercentage;
        var commission = new Comission(business.Id, commissionValue);

        // Construir a cadeia completa de recomendação (sem limite)
        var recommendationChain = await BuildRecommendationChainAsync(partner);

        // Distribuir comissões dinamicamente baseado no tamanho da cadeia
        await DistributeCommissionDynamicallyAsync(commission, recommendationChain, commissionValue);

        await _commissionRepository.AddAsync(commission);
        return commission;
    }

    /// <summary>
    /// Constrói a cadeia de recomendação completa, EXCLUINDO quem fechou o negócio
    /// Considera toda a estrutura de recomendação até o Vetor
    /// </summary>
    private async Task<List<Partner>> BuildRecommendationChainAsync(Partner partnerWhoClosedDeal)
    {
        var chain = new List<Partner>();
        var current = partnerWhoClosedDeal;

        // Construir cadeia a partir de quem fechou o negócio (EXCLUINDO ele próprio)
        // Subir na hierarquia até chegar ao topo (Vetor não é Partner, então parar quando não houver RecommenderId)
        while (current.RecommenderId.HasValue)
        {
            var recommender = await _partnerRepository.GetByIdAsync(current.RecommenderId.Value);
            if (recommender == null) break;

            chain.Add(recommender);
            current = recommender;
        }

        // A cadeia está ordenada: [0] = recomendador direto, [n-1] = parceiro mais próximo do Vetor
        return chain;
    }

    /// <summary>
    /// Distribui a comissão dinamicamente baseado no tamanho da cadeia
    /// Usa CommissionSettings.CalculateDistribution para obter os percentuais
    /// IMPORTANTE: A distribuição sempre inclui o Vetor na posição [0]
    /// </summary>
    private async Task DistributeCommissionDynamicallyAsync(
        Comission commission, 
        List<Partner> partnerChain, 
        decimal totalValue)
    {
        // A cadeia SEMPRE inclui o Vetor, mesmo que não seja um Partner
        // chain.Count = partnerChain.Count + 1 (para incluir o Vetor)
        var totalChainLength = partnerChain.Count + 1;
        
        if (totalChainLength <= 1)
        {
            // Sem parceiros na cadeia, só o Vetor receberia, mas como não é Partner, não faz pagamento
            return;
        }

        // Obter distribuição de percentuais (incluindo o Vetor)
        var distribution = _commissionSettings.CalculateDistribution(totalChainLength);

        // Distribuir para o Vetor (se existir como Partner - caso especial)
        // Por enquanto, pular a posição [0] do distribution que é o Vetor
        
        // Distribuir para os parceiros na cadeia
        for (int i = 0; i < partnerChain.Count; i++)
        {
            var partner = partnerChain[i];
            // partnerChain[0] = recomendador direto -> distribution[totalChainLength - 1]
            // partnerChain[n-1] = último da hierarquia -> distribution[1] (posição 0 é do Vetor)
            var distributionIndex = totalChainLength - 1 - i;
            var percentage = distribution[distributionIndex];
            var value = totalValue * percentage;

            if (value > 0)
            {
                // Determinar tipo de pagamento baseado na posição na cadeia
                var paymentType = DeterminePaymentTypeForPartner(i, partnerChain.Count);
                commission.AddPagamento(partner.Id, value, paymentType);
            }
        }

        // Adicionar pagamento para o Vetor (se necessário)
        await AddVetorPaymentAsync(commission, partnerChain, totalValue, distribution[0]);
    }

    /// <summary>
    /// Adiciona pagamento para o Vetor se necessário
    /// </summary>
    private async Task AddVetorPaymentAsync(Comission commission, List<Partner> partnerChain, decimal totalValue, decimal vetorPercentage)
    {
        if (partnerChain.Count == 0 || vetorPercentage <= 0) return;

        // Pegar o Vetor do primeiro parceiro da cadeia (todos pertencem ao mesmo Vetor)
        var firstPartner = partnerChain[0];
        var vetorValue = totalValue * vetorPercentage;
        
        // Buscar se existe um Partner que representa o Vetor
        // Isso pode ser implementado de diferentes formas:
        // 1. O Vetor pode ter um Partner associado
        // 2. Pode haver uma convenção específica no sistema
        
        // Por enquanto, vou implementar buscando por Partners do mesmo VetorId que não têm RecommenderId
        var vetorAsPartners = await _partnerRepository.GetByVetorIdAsync(firstPartner.VetorId);
        var vetorPartner = vetorAsPartners.FirstOrDefault(p => !p.RecommenderId.HasValue);
        
        if (vetorPartner != null)
        {
            commission.AddPagamento(vetorPartner.Id, vetorValue, PaymentType.Vetor);
        }
        
        // Se não encontrou o Vetor como Partner, ele não recebe comissão diretamente
        // Isso pode estar correto dependendo da regra de negócio
    }

    /// <summary>
    /// Determina o tipo de pagamento para parceiros (excluindo Vetor)
    /// </summary>
    private PaymentType DeterminePaymentTypeForPartner(int position, int partnerChainLength)
    {
        if (partnerChainLength == 0)
        {
            throw new InvalidOperationException("Cadeia de parceiros vazia");
        }
        else if (position == 0)
        {
            // Primeiro da cadeia de parceiros = Recomendador direto
            return PaymentType.Recomendador;
        }
        else
        {
            // Posições intermediárias na cadeia de parceiros
            return PaymentType.Intermediario;
        }
    }
}