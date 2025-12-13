using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.CancelBusiness.DTO;
using Domain.ValueObjects;

namespace Application.UseCases.CancelBusiness;

public class CancelBusinessUseCase : ICancelBusinessUseCase
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;

    public CancelBusinessUseCase(
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

    public async Task<CancelBusinessResult> ExecuteAsync(Guid businessId, CancelBusinessRequest request, Guid userId)
    {
        // Buscar o negócio
        var business = await _businessRepository.GetByIdAsync(businessId);
        if (business == null)
        {
            return CancelBusinessResult.Failure("Negócio não encontrado");
        }

        // Verificar se já está cancelado
        if (business.Status == "cancelado")
        {
            return CancelBusinessResult.Failure("Negócio já está cancelado");
        }

        // Buscar a comissão associada
        var commission = await _commissionRepository.GetByBusinessIdAsync(businessId);
        if (commission == null)
        {
            return CancelBusinessResult.Failure("Comissão não encontrada para este negócio");
        }

        // Verificar se há pagamentos já efetuados
        var paidPaymentsCount = commission.GetPaidPaymentsCount();
        var paidPaymentsValue = commission.GetPaidPaymentsValue();

        if (paidPaymentsCount > 0 && !request.ForceCancel)
        {
            return CancelBusinessResult.Failure(
                $"Não é possível cancelar o negócio pois há {paidPaymentsCount} pagamento(s) já efetuado(s) " +
                $"no valor total de {paidPaymentsValue:C}. Use ForceCancel=true se desejar cancelar mesmo assim.");
        }

        // Coletar dados antes do cancelamento para o relatório
        var pendingPaymentsCount = commission.GetPendingPaymentsCount();
        var pendingPaymentsValue = commission.GetPendingPaymentsValue();

        // Buscar informações dos parceiros para o relatório detalhado
        var paymentDetails = new List<PaymentCancellationDetail>();
        foreach (var payment in commission.Pagamentos)
        {
            var paymentPartner = await _partnerRepository.GetByIdAsync(payment.PartnerId);
            var wasCancelled = payment.Status == ComissionPayment.APagar;
            
            paymentDetails.Add(new PaymentCancellationDetail
            {
                PaymentId = payment.Id,
                PartnerId = payment.PartnerId,
                PartnerName = paymentPartner?.Name ?? "Partner não encontrado",
                PaymentType = payment.TipoPagamento,
                Value = payment.Value,
                OriginalStatus = payment.Status,
                FinalStatus = wasCancelled ? ComissionPayment.Cancelado : payment.Status,
                WasCancelled = wasCancelled,
                CancellationNote = wasCancelled ? "Cancelado devido ao cancelamento do negócio" : 
                                 payment.Status == ComissionPayment.Pago ? "Mantido - já estava pago" : "Não alterado"
            });
        }

        // Cancelar pagamentos pendentes
        commission.CancelPendingPayments();

        // Cancelar o negócio
        business.CancelBusiness();

        // Salvar alterações
        await _businessRepository.UpdateAsync(business);
        await _commissionRepository.UpdateAsync(commission);

        // Buscar informações para o DTO
        var partner = await _partnerRepository.GetByIdAsync(business.PartnerId);
        var businessType = await _businessTypeRepository.GetByIdAsync(business.BussinessTypeId);

        // Preparar resposta
        var cancelledBusinessDto = new CancelledBusinessDto
        {
            Id = business.Id,
            PartnerId = business.PartnerId,
            PartnerName = partner?.Name ?? "Partner não encontrado",
            BusinessTypeId = business.BussinessTypeId,
            BusinessTypeName = businessType?.Name ?? "Tipo não encontrado",
            Value = business.Value,
            Status = business.Status,
            Date = business.Date,
            Observations = business.Observations,
            CancellationReason = request.CancellationReason,
            CreatedAt = business.CreatedAt,
            CancelledAt = DateTime.UtcNow
        };

        var commissionSummary = new CommissionCancellationSummary
        {
            CommissionId = commission.Id,
            OriginalTotalValue = commission.TotalValue,
            TotalPayments = commission.Pagamentos.Count,
            PendingPaymentsCancelled = pendingPaymentsCount,
            PaidPaymentsKept = paidPaymentsCount,
            PendingValueCancelled = pendingPaymentsValue,
            PaidValueKept = paidPaymentsValue,
            PaymentDetails = paymentDetails
        };

        return CancelBusinessResult.Success(cancelledBusinessDto, commissionSummary);
    }
}