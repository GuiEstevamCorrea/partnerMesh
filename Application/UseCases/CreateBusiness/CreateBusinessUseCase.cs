using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.CreateBusiness.DTO;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.CreateBusiness;

public class CreateBusinessUseCase : ICreateBusinessUseCase
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;

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
                    ReceiverType = p.TipoPagamento,
                    Value = p.Value,
                    Status = p.Status
                }).ToList()
            }
        );

        return result;
    }

    private async Task<Comission> CreateCommissionAsync(Bussiness business, Partner partner)
    {
        var commissionValue = business.Value * 0.10m; // 10% do valor do negócio
        var commission = new Comission(business.Id, commissionValue);

        // Construir a cadeia de recomendação até 3 níveis
        var recommendationChain = await BuildRecommendationChainAsync(partner, 3);

        // Distribuir comissões baseado no nível da recomendação
        switch (recommendationChain.Count)
        {
            case 1: // Nível 1
                await DistributeLevel1CommissionAsync(commission, recommendationChain, commissionValue);
                break;
            case 2: // Nível 2
                await DistributeLevel2CommissionAsync(commission, recommendationChain, commissionValue);
                break;
            case 3: // Nível 3
                await DistributeLevel3CommissionAsync(commission, recommendationChain, commissionValue);
                break;
        }

        await _commissionRepository.AddAsync(commission);
        return commission;
    }

    private async Task<List<Partner>> BuildRecommendationChainAsync(Partner startPartner, int maxLevels)
    {
        var chain = new List<Partner> { startPartner };
        var current = startPartner;

        while (chain.Count < maxLevels && current.RecommenderId.HasValue)
        {
            var recommender = await _partnerRepository.GetByIdAsync(current.RecommenderId.Value);
            if (recommender == null) break;

            chain.Add(recommender);
            current = recommender;
        }

        return chain;
    }

    private async Task DistributeLevel1CommissionAsync(Comission commission, List<Partner> chain, decimal totalValue)
    {
        var partner = chain[0];
        var recommender = chain.Count > 1 ? chain[1] : null;

        if (recommender != null)
        {
            // Vetor: 50%, Recomendador: 50%
            var vetorValue = totalValue * 0.50m;
            var recommenderValue = totalValue * 0.50m;

            commission.AddPagamento(partner.Id, vetorValue, Domain.ValueTypes.PaymentType.Vetor);
            commission.AddPagamento(recommender.Id, recommenderValue, Domain.ValueTypes.PaymentType.Recomendador);
        }
        else
        {
            // Apenas o vetor recebe 100%
            commission.AddPagamento(partner.Id, totalValue, Domain.ValueTypes.PaymentType.Vetor);
        }
    }

    private async Task DistributeLevel2CommissionAsync(Comission commission, List<Partner> chain, decimal totalValue)
    {
        var you = chain[0];        // Você
        var level1 = chain[1];     // Nível 1 (Intermediário)
        var level2 = chain[2];     // Nível 2 (Vetor)

        // Vetor (Level 2): 15%, Você: 35%, Intermediário (Level 1): 50%
        var vetorValue = totalValue * 0.15m;
        var youValue = totalValue * 0.35m;
        var intermediaryValue = totalValue * 0.50m;

        commission.AddPagamento(level2.Id, vetorValue, Domain.ValueTypes.PaymentType.Vetor);
        commission.AddPagamento(you.Id, youValue, Domain.ValueTypes.PaymentType.Participante);
        commission.AddPagamento(level1.Id, intermediaryValue, Domain.ValueTypes.PaymentType.Intermediario);
    }

    private async Task DistributeLevel3CommissionAsync(Comission commission, List<Partner> chain, decimal totalValue)
    {
        var you = chain[0];        // Você
        var level1 = chain[1];     // Nível 1
        var level2 = chain[2];     // Nível 2
        var level3 = chain[3];     // Nível 3 (Vetor)

        // Vetor (Level 3): 10%, Você: 15%, Level1: 25%, Level2: 50%
        var vetorValue = totalValue * 0.10m;
        var youValue = totalValue * 0.15m;
        var level1Value = totalValue * 0.25m;
        var level2Value = totalValue * 0.50m;

        commission.AddPagamento(level3.Id, vetorValue, Domain.ValueTypes.PaymentType.Vetor);
        commission.AddPagamento(you.Id, youValue, Domain.ValueTypes.PaymentType.Participante);
        commission.AddPagamento(level1.Id, level1Value, Domain.ValueTypes.PaymentType.Intermediario);
        commission.AddPagamento(level2.Id, level2Value, Domain.ValueTypes.PaymentType.Intermediario);
    }
}