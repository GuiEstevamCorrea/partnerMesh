using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetCancelledBusinessSummary.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.GetCancelledBusinessSummary;

public class CancelledBusinessSummaryUseCase : ICancelledBusinessSummaryUseCase
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICommissionRepository _commissionRepository;

    public CancelledBusinessSummaryUseCase(
        IBusinessRepository businessRepository,
        ICommissionRepository commissionRepository)
    {
        _businessRepository = businessRepository;
        _commissionRepository = commissionRepository;
    }

    public async Task<CancelledBusinessSummaryResult> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Buscar todos os negócios cancelados usando o filtro existente
            var (cancelledBusinesses, _) = await _businessRepository.GetWithFiltersAsync(
                status: "Cancelled", // Status no formato legado
                pageSize: int.MaxValue, // Buscar todos
                cancellationToken: cancellationToken);

            if (!cancelledBusinesses.Any())
            {
                return CancelledBusinessSummaryResult.Success(new CancelledBusinessSummaryDto
                {
                    TotalCancelledBusinesses = 0,
                    TotalCancelledValue = 0,
                    TotalCancelledCommissions = 0,
                    TotalCancelledPayments = 0,
                    CancelledPaymentsCount = 0,
                    PaidBeforeCancellation = 0,
                    PaidBeforeCancellationCount = 0
                });
            }

            decimal totalCancelledValue = 0;
            decimal totalCancelledCommissions = 0;
            decimal totalCancelledPayments = 0;
            int cancelledPaymentsCount = 0;
            decimal paidBeforeCancellation = 0;
            int paidBeforeCancellationCount = 0;

            foreach (var business in cancelledBusinesses)
            {
                totalCancelledValue += business.Value;

                // Buscar a comissão associada
                var commission = await _commissionRepository.GetByBusinessIdAsync(business.Id);
                if (commission != null)
                {
                    totalCancelledCommissions += commission.TotalValue;

                    // Calcular pagamentos cancelados e pagos antes do cancelamento
                    var cancelledPayments = commission.Pagamentos.Where(p => p.Status == PaymentStatus.Cancelado);
                    var paidPayments = commission.Pagamentos.Where(p => p.Status == PaymentStatus.Pago);

                    totalCancelledPayments += cancelledPayments.Sum(p => p.Value);
                    cancelledPaymentsCount += cancelledPayments.Count();

                    paidBeforeCancellation += paidPayments.Sum(p => p.Value);
                    paidBeforeCancellationCount += paidPayments.Count();
                }
            }

            var summary = new CancelledBusinessSummaryDto
            {
                TotalCancelledBusinesses = cancelledBusinesses.Count(),
                TotalCancelledValue = totalCancelledValue,
                TotalCancelledCommissions = totalCancelledCommissions,
                TotalCancelledPayments = totalCancelledPayments,
                CancelledPaymentsCount = cancelledPaymentsCount,
                PaidBeforeCancellation = paidBeforeCancellation,
                PaidBeforeCancellationCount = paidBeforeCancellationCount
            };

            return CancelledBusinessSummaryResult.Success(summary);
        }
        catch (Exception ex)
        {
            return CancelledBusinessSummaryResult.Failure($"Erro interno: {ex.Message}");
        }
    }
}