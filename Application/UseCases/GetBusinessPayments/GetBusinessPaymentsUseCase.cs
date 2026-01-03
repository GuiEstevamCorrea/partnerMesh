using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetBusinessPayments.DTO;
using Domain.ValueObjects;
using Domain.ValueTypes;
using Domain.Extensions;

namespace Application.UseCases.GetBusinessPayments;

public class GetBusinessPaymentsUseCase : IGetBusinessPaymentsUseCase
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVetorRepository _vetorRepository;

    public GetBusinessPaymentsUseCase(
        IBusinessRepository businessRepository,
        ICommissionRepository commissionRepository,
        IPartnerRepository partnerRepository,
        IBusinessTypeRepository businessTypeRepository,
        IUserRepository userRepository,
        IVetorRepository vetorRepository)
    {
        _businessRepository = businessRepository;
        _commissionRepository = commissionRepository;
        _partnerRepository = partnerRepository;
        _businessTypeRepository = businessTypeRepository;
        _userRepository = userRepository;
        _vetorRepository = vetorRepository;
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

            // Validar acesso do usuário ao vetor do negócio
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null || !user.Active)
            {
                return GetBusinessPaymentsResult.Failure("Usuário não encontrado ou inativo.");
            }

            // Buscar o parceiro para obter o vetorId
            var businessPartner = await _partnerRepository.GetByIdAsync(business.PartnerId, cancellationToken);
            if (businessPartner == null)
            {
                return GetBusinessPaymentsResult.Failure("Parceiro do negócio não encontrado.");
            }

            // Verificar se o usuário tem acesso ao vetor do negócio
            // AdminGlobal tem acesso a todos os vetores
            var hasGlobalAccess = user.Permission.HasFlag(Domain.ValueTypes.PermissionEnum.AdminGlobal);
            if (!hasGlobalAccess)
            {
                // Para outros usuários, verificar se estão vinculados ao vetor
                var hasVetorAccess = user.UserVetores.Any(uv => uv.VetorId == businessPartner.VetorId && uv.Active);
                if (!hasVetorAccess)
                {
                    return GetBusinessPaymentsResult.Failure("Usuário não tem acesso aos pagamentos deste negócio.");
                }
            }

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
                string partnerName;
                
                // Verificar se é pagamento para vetor ou parceiro
                if (payment.TipoPagamento == PaymentType.Vetor)
                {
                    // Para pagamentos de Vetor, primeiro tentar como Partner
                    var paymentPartner = await _partnerRepository.GetByIdAsync(payment.PartnerId, cancellationToken);
                    if (paymentPartner != null)
                    {
                        // É um Partner representando o Vetor
                        var vetor = await _vetorRepository.GetByIdAsync(paymentPartner.VetorId, cancellationToken);
                        partnerName = vetor?.Name ?? "Vetor não encontrado";
                    }
                    else
                    {
                        // É um pagamento direto para o Vetor (payment.PartnerId é na verdade um VetorId)
                        var vetor = await _vetorRepository.GetByIdAsync(payment.PartnerId, cancellationToken);
                        partnerName = vetor?.Name ?? "Vetor não encontrado";
                    }
                }
                else
                {
                    var paymentPartner = await _partnerRepository.GetByIdAsync(payment.PartnerId, cancellationToken);
                    partnerName = paymentPartner?.Name ?? "Parceiro não encontrado";
                }
                
                var paymentDto = new BusinessPaymentDto
                {
                    Id = payment.Id,
                    PartnerId = payment.PartnerId,
                    PartnerName = partnerName,
                    TipoPagamento = payment.TipoPagamento.ToLegacyString(),
                    Value = payment.Value,
                    Status = payment.Status.ToLegacyString(),
                    PaidOn = payment.PaidOn,
                    CreatedAt = commission.CreatedAt
                };

                paymentDtos.Add(paymentDto);
            }

            // Calcular resumo
            var summary = new PaymentSummaryDto
            {
                TotalPago = paymentDtos.Where(p => p.Status == PaymentStatus.Pago.ToLegacyString()).Sum(p => p.Value),
                TotalPendente = paymentDtos.Where(p => p.Status == PaymentStatus.APagar.ToLegacyString()).Sum(p => p.Value),
                TotalCancelado = paymentDtos.Where(p => p.Status == PaymentStatus.Cancelado.ToLegacyString()).Sum(p => p.Value),
                QuantidadePagos = paymentDtos.Count(p => p.Status == PaymentStatus.Pago.ToLegacyString()),
                QuantidadePendentes = paymentDtos.Count(p => p.Status == PaymentStatus.APagar.ToLegacyString()),
                QuantidadeCancelados = paymentDtos.Count(p => p.Status == PaymentStatus.Cancelado.ToLegacyString())
            };

            // Criar DTO principal
            var businessPaymentsDto = new BusinessPaymentsDto
            {
                BusinessId = business.Id,
                BusinessDescription = business.Observations,
                BusinessValue = business.Value,
                BusinessDate = business.CreatedAt,
                BusinessStatus = business.Status.ToLegacyString(),
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