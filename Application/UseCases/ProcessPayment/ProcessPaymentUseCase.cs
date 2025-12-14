using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ProcessPayment.DTO;
using Domain.ValueObjects;

namespace Application.UseCases.ProcessPayment;

public class ProcessPaymentUseCase : IProcessPaymentUseCase
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IUserRepository _userRepository;

    public ProcessPaymentUseCase(
        ICommissionRepository commissionRepository,
        IPartnerRepository partnerRepository,
        IUserRepository userRepository)
    {
        _commissionRepository = commissionRepository;
        _partnerRepository = partnerRepository;
        _userRepository = userRepository;
    }

    public async Task<ProcessPaymentResult> ExecuteAsync(
        ProcessPaymentRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar entrada
            if (!request.IsValid())
            {
                return ProcessPaymentResult.Failure("Dados de entrada inválidos.");
            }

            // Verificar se o usuário existe
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return ProcessPaymentResult.Failure("Usuário não encontrado.");
            }

            // Buscar todas as comissões para encontrar o pagamento
            var commissions = await _commissionRepository.GetAllAsync(cancellationToken);
            ComissionPayment? payment = null;
            Domain.Entities.Comission? commission = null;

            foreach (var comm in commissions)
            {
                payment = comm.Pagamentos.FirstOrDefault(p => p.Id == request.PaymentId);
                if (payment != null)
                {
                    commission = comm;
                    break;
                }
            }

            if (payment == null)
            {
                return ProcessPaymentResult.Failure("Pagamento não encontrado.");
            }

            // Verificar se o pagamento já foi pago
            if (payment.Status == Domain.ValueTypes.PaymentStatus.Pago)
            {
                return ProcessPaymentResult.Failure("Este pagamento já foi efetuado.");
            }

            // Verificar se o pagamento não está cancelado
            if (payment.Status == Domain.ValueTypes.PaymentStatus.Cancelado)
            {
                return ProcessPaymentResult.Failure("Não é possível efetuar um pagamento cancelado.");
            }

            // Efetuar o pagamento
            payment.UpdateStatusToPaid();

            // Atualizar no repositório
            await _commissionRepository.UpdateAsync(commission, cancellationToken);

            // Buscar dados do parceiro
            var partner = await _partnerRepository.GetByIdAsync(payment.PartnerId, cancellationToken);

            // Criar DTO de resposta
            var paymentDto = new PaymentProcessedDto
            {
                Id = payment.Id,
                ComissionId = payment.ComissionId,
                PartnerId = payment.PartnerId,
                PartnerName = partner?.Name ?? "Parceiro não encontrado",
                TipoPagamento = payment.TipoPagamento,
                Value = payment.Value,
                Status = payment.Status.ToLegacyString(),
                PaidOn = payment.PaidOn,
                ProcessedBy = userId,
                ProcessedByName = user.Name
            };

            return ProcessPaymentResult.Success(paymentDto);
        }
        catch (Exception ex)
        {
            return ProcessPaymentResult.Failure($"Erro interno: {ex.Message}");
        }
    }
}