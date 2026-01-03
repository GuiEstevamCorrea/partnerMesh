using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetBusinessById.DTO;
using Domain.ValueObjects;
using Domain.ValueTypes;
using Domain.Extensions;

namespace Application.UseCases.GetBusinessById;

public class GetBusinessByIdUseCase : IGetBusinessByIdUseCase
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly IVetorRepository _vetorRepository;

    public GetBusinessByIdUseCase(
        IBusinessRepository businessRepository,
        ICommissionRepository commissionRepository,
        IPartnerRepository partnerRepository,
        IBusinessTypeRepository businessTypeRepository,
        IVetorRepository vetorRepository)
    {
        _businessRepository = businessRepository;
        _commissionRepository = commissionRepository;
        _partnerRepository = partnerRepository;
        _businessTypeRepository = businessTypeRepository;
        _vetorRepository = vetorRepository;
    }

    public async Task<GetBusinessByIdResult> ExecuteAsync(Guid businessId, Guid userId)
    {
        try
        {
            // Buscar o negócio
            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null)
            {
                return GetBusinessByIdResult.NotFound();
            }

            // Buscar informações do parceiro
            var partner = await _partnerRepository.GetByIdAsync(business.PartnerId);
            if (partner == null)
            {
                return GetBusinessByIdResult.Failure("Parceiro associado ao negócio não encontrado");
            }

            // Buscar informações do tipo de negócio
            var businessType = await _businessTypeRepository.GetByIdAsync(business.BussinessTypeId);
            if (businessType == null)
            {
                return GetBusinessByIdResult.Failure("Tipo de negócio associado não encontrado");
            }

            // Buscar informações da comissão
            var commission = await _commissionRepository.GetByBusinessIdAsync(businessId);
            var detailedCommission = await BuildDetailedCommissionInfo(commission);

            // Construir DTO completo
            var businessDetail = new BusinessDetailDto
            {
                Id = business.Id,
                PartnerId = business.PartnerId,
                PartnerName = partner.Name,
                PartnerEmail = partner.Email,
                PartnerPhone = partner.PhoneNumber,
                BusinessTypeId = business.BussinessTypeId,
                BusinessTypeName = businessType.Name,
                BusinessTypeDescription = businessType.Description,
                Value = business.Value,
                Status = business.Status.ToLegacyString(),
                Date = business.Date,
                Observations = business.Observations,
                CreatedAt = business.CreatedAt,
                UpdatedAt = business.Status == Domain.ValueTypes.BusinessStatus.Cancelado ? DateTime.UtcNow : null, // Simular UpdatedAt
                CancellationReason = business.Status == Domain.ValueTypes.BusinessStatus.Cancelado ? "Negócio cancelado" : null,
                CancelledAt = business.Status == Domain.ValueTypes.BusinessStatus.Cancelado ? DateTime.UtcNow : null,
                Commission = detailedCommission
            };

            return GetBusinessByIdResult.Success(businessDetail);
        }
        catch (Exception ex)
        {
            return GetBusinessByIdResult.Failure($"Erro ao buscar negócio: {ex.Message}");
        }
    }

    private async Task<DetailedCommissionInfo> BuildDetailedCommissionInfo(Domain.Entities.Comission? commission)
    {
        if (commission == null)
        {
            return new DetailedCommissionInfo
            {
                CommissionStatus = "Comissão não encontrada"
            };
        }

        var totalPayments = commission.Pagamentos.Count;
        var paidPayments = commission.Pagamentos.Count(p => p.Status == PaymentStatus.Pago);
        var pendingPayments = commission.Pagamentos.Count(p => p.Status == PaymentStatus.APagar);
        var cancelledPayments = commission.Pagamentos.Count(p => p.Status == PaymentStatus.Cancelado);

        var totalPaidValue = commission.Pagamentos
            .Where(p => p.Status == PaymentStatus.Pago)
            .Sum(p => p.Value);

        var totalPendingValue = commission.Pagamentos
            .Where(p => p.Status == PaymentStatus.APagar)
            .Sum(p => p.Value);

        var totalCancelledValue = commission.Pagamentos
            .Where(p => p.Status == PaymentStatus.Cancelado)
            .Sum(p => p.Value);

        // Determinar status da comissão
        string commissionStatus;
        if (cancelledPayments == totalPayments && totalPayments > 0)
            commissionStatus = "Totalmente Cancelado";
        else if (paidPayments == totalPayments && totalPayments > 0)
            commissionStatus = "Totalmente Pago";
        else if (paidPayments > 0)
            commissionStatus = "Parcialmente Pago";
        else if (pendingPayments > 0)
            commissionStatus = "Pendente";
        else
            commissionStatus = "Sem Pagamentos";

        // Construir detalhes dos pagamentos
        var paymentDetails = new List<CommissionPaymentDetailDto>();
        foreach (var payment in commission.Pagamentos.OrderBy(p => p.PartnerId))
        {
            string partnerName;
            
            if (payment.TipoPagamento == Domain.ValueTypes.PaymentType.Vetor)
            {
                // Para pagamentos de Vetor, primeiro tentar como Partner
                var paymentPartner = await _partnerRepository.GetByIdAsync(payment.PartnerId);
                if (paymentPartner != null)
                {
                    // É um Partner representando o Vetor
                    var vetor = await _vetorRepository.GetByIdAsync(paymentPartner.VetorId);
                    if (vetor != null)
                    {
                        partnerName = vetor.Name;
                    }
                    else
                    {
                        partnerName = "Vetor não encontrado";
                    }
                }
                else
                {
                    // É um pagamento direto para o Vetor (payment.PartnerId é na verdade um VetorId)
                    var vetor = await _vetorRepository.GetByIdAsync(payment.PartnerId);
                    if (vetor != null)
                    {
                        partnerName = vetor.Name;
                    }
                    else
                    {
                        partnerName = "Vetor não encontrado";
                    }
                }
            }
            else
            {
                // Para outros tipos, buscar no repositório de Partner
                var paymentPartner = await _partnerRepository.GetByIdAsync(payment.PartnerId);
                partnerName = paymentPartner?.Name ?? "Parceiro não encontrado";
            }
            
            // Determinar o nível baseado no tipo de pagamento
            string level = payment.TipoPagamento switch
            {
                Domain.ValueTypes.PaymentType.Vetor => "Vetor",
                Domain.ValueTypes.PaymentType.Recomendador => "Nível 1",
                Domain.ValueTypes.PaymentType.Participante => "Você",
                Domain.ValueTypes.PaymentType.Intermediario => "Intermediário",
                _ => "Não identificado"
            };

            paymentDetails.Add(new CommissionPaymentDetailDto
            {
                PaymentId = payment.Id,
                PartnerId = payment.PartnerId,
                PartnerName = partnerName,
                PaymentType = payment.TipoPagamento.ToLegacyString(),
                Value = payment.Value,
                Status = payment.Status.ToLegacyString(),
                CreatedAt = commission.CreatedAt, // Usar a data da comissão como referência
                PaidOn = payment.PaidOn,
                Level = level
            });
        }

        return new DetailedCommissionInfo
        {
            CommissionId = commission.Id,
            TotalValue = commission.TotalValue,
            CreatedAt = commission.CreatedAt,
            TotalPayments = totalPayments,
            PaidPayments = paidPayments,
            PendingPayments = pendingPayments,
            CancelledPayments = cancelledPayments,
            TotalPaidValue = totalPaidValue,
            TotalPendingValue = totalPendingValue,
            TotalCancelledValue = totalCancelledValue,
            CommissionStatus = commissionStatus,
            Payments = paymentDetails
        };
    }
}