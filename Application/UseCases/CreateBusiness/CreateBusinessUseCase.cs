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
    /// Constrói a cadeia completa de recomendação, indo do parceiro que fechou até o Vetor
    /// Suporta níveis infinitos
    /// </summary>
    private async Task<List<Partner>> BuildRecommendationChainAsync(Partner startPartner)
    {
        var chain = new List<Partner> { startPartner };
        var current = startPartner;

        // Continuar até não haver mais recomendadores
        while (current.RecommenderId.HasValue)
        {
            var recommender = await _partnerRepository.GetByIdAsync(current.RecommenderId.Value);
            if (recommender == null) break;

            chain.Add(recommender);
            current = recommender;
        }

        return chain;
    }

    /// <summary>
    /// Distribui a comissão dinamicamente baseado no tamanho da cadeia
    /// Usa CommissionSettings.CalculateDistribution para obter os percentuais
    /// </summary>
    private async Task DistributeCommissionDynamicallyAsync(
        Comission commission, 
        List<Partner> chain, 
        decimal totalValue)
    {
        if (chain.Count == 0) return;

        // Obter distribuição de percentuais
        var distribution = _commissionSettings.CalculateDistribution(chain.Count);

        // O último da cadeia é o Vetor, então precisamos inverter
        // chain[0] = quem fechou, chain[n-1] = Vetor
        // distribution[0] = Vetor, distribution[n-1] = quem fechou

        for (int i = 0; i < chain.Count; i++)
        {
            var partner = chain[i];
            var percentage = distribution[chain.Count - 1 - i]; // Inverter índice
            var value = totalValue * percentage;

            if (value > 0)
            {
                // Determinar tipo de pagamento baseado na posição
                var paymentType = DeterminePaymentType(i, chain.Count);
                commission.AddPagamento(partner.Id, value, paymentType);
            }
        }
    }

    /// <summary>
    /// Determina o tipo de pagamento baseado na posição na cadeia
    /// </summary>
    /// <param name="position">Posição na cadeia (0 = quem fechou, n-1 = Vetor)</param>
    /// <param name="chainLength">Tamanho total da cadeia</param>
    private PaymentType DeterminePaymentType(int position, int chainLength)
    {
        if (position == 0)
        {
            // Quem fechou o negócio
            return chainLength == 1 ? PaymentType.Vetor : PaymentType.Participante;
        }
        else if (position == chainLength - 1)
        {
            // Último da cadeia (Vetor)
            return PaymentType.Vetor;
        }
        else if (position == 1 && chainLength == 2)
        {
            // Apenas 2 níveis: o segundo é Recomendador
            return PaymentType.Recomendador;
        }
        else
        {
            // Intermediários
            return PaymentType.Intermediario;
        }
    }
}