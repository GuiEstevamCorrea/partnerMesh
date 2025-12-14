using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetBusinessPayments.DTO;
using Domain.ValueObjects;

namespace Application.UseCases.GetBusinessPayments;

public class GetBusinessPaymentsUseCase : IGetBusinessPaymentsUseCase
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;

    public GetBusinessPaymentsUseCase(
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

    public async Task<GetBusinessPaymentsResult> ExecuteAsync(
        Guid businessId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar entrada
            if (businessId == Guid.Empty)
            {
                return GetBusinessPaymentsResult.Failure("ID do negócio é obrigatório.");
            }

            // Buscar o negócio
            var business = await _businessRepository.GetByIdAsync(businessId, cancellationToken);
            if (business == null)
            {
                return GetBusinessPaymentsResult.Failure("Negócio não encontrado.");
            }

            // TODO: Implementar validação de acesso por vetor quando necessário
            // Verificar se o usuário tem acesso a este negócio baseado no vetor

            // Buscar a comissão associada ao negócio
            var commission = await _commissionRepository.GetByBusinessIdAsync(businessId, cancellationToken);
            if (commission == null)
            {
                return GetBusinessPaymentsResult.Failure("Comissões não encontradas para este negócio.");
            }

            // Buscar dados complementares
            var partner = await _partnerRepository.GetByIdAsync(business.PartnerId, cancellationToken);
            var businessType = await _businessTypeRepository.GetByIdAsync(business.BussinessTypeId, cancellationToken);

            // Converter pagamentos para DTOs
            var paymentDtos = new List<BusinessPaymentDto>();
            foreach (var payment in commission.Pagamentos)
            {
                var paymentPartner = await _partnerRepository.GetByIdAsync(payment.PartnerId, cancellationToken);
                
                var paymentDto = new BusinessPaymentDto
                {
                    Id = payment.Id,
                    PartnerId = payment.PartnerId,
                    PartnerName = paymentPartner?.Name ?? "Parceiro não encontrado",
                    TipoPagamento = payment.TipoPagamento,
                    Value = payment.Value,
                    Status = payment.Status,
                    PaidOn = payment.PaidOn,
                    CreatedAt = commission.CreatedAt
                };

                paymentDtos.Add(paymentDto);
            }

            // Calcular resumo
            var summary = new PaymentSummaryDto
            {
                TotalPago = paymentDtos.Where(p => p.Status == ComissionPayment.Pago).Sum(p => p.Value),
                TotalPendente = paymentDtos.Where(p => p.Status == ComissionPayment.APagar).Sum(p => p.Value),
                TotalCancelado = paymentDtos.Where(p => p.Status == ComissionPayment.Cancelado).Sum(p => p.Value),
                QuantidadePagos = paymentDtos.Count(p => p.Status == ComissionPayment.Pago),
                QuantidadePendentes = paymentDtos.Count(p => p.Status == ComissionPayment.APagar),
                QuantidadeCancelados = paymentDtos.Count(p => p.Status == ComissionPayment.Cancelado)
            };

            // Criar DTO principal
            var businessPaymentsDto = new BusinessPaymentsDto
            {
                BusinessId = business.Id,
                BusinessDescription = business.Observations,
                BusinessValue = business.Value,
                BusinessDate = business.CreatedAt,
                BusinessStatus = business.Status,
                PartnerName = partner?.Name ?? "Parceiro não encontrado",
                BusinessTypeName = businessType?.Name ?? "Tipo não encontrado",
                TotalCommission = commission.TotalValue,
                Payments = paymentDtos.OrderByDescending(p => p.CreatedAt),
                Summary = summary
            };

            return GetBusinessPaymentsResult.Success(businessPaymentsDto);
        }
        catch (Exception ex)
        {
            return GetBusinessPaymentsResult.Failure($"Erro interno: {ex.Message}");
        }
    }
}