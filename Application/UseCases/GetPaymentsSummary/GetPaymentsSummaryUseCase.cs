using Application.Interfaces.Repositories;
using Application.Interfaces.IUseCases;
using Application.UseCases.GetPaymentsSummary.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.GetPaymentsSummary;

public class GetPaymentsSummaryUseCase : IGetPaymentsSummaryUseCase
{
    private readonly ICommissionRepository _commissionRepository;

    public GetPaymentsSummaryUseCase(ICommissionRepository commissionRepository)
    {
        _commissionRepository = commissionRepository;
    }

    public async Task<GetPaymentsSummaryResult> ExecuteAsync(
        GetPaymentsSummaryRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Obter resumo dos pagamentos
            var (totalPaid, totalPending, totalCancelled, countPaid, countPending, countCancelled) = 
                await _commissionRepository.GetPaymentsSummaryAsync(
                    request.VetorId,
                    request.PartnerId,
                    request.StartDate,
                    request.EndDate,
                    request.Status,
                    request.TipoPagamento,
                    cancellationToken);

            var summary = new PaymentsSummaryDto
            {
                TotalPaid = totalPaid,
                TotalPending = totalPending,
                TotalCancelled = totalCancelled,
                CountPaid = countPaid,
                CountPending = countPending,
                CountCancelled = countCancelled
            };

            return GetPaymentsSummaryResult.Success(summary);
        }
        catch (Exception ex)
        {
            return GetPaymentsSummaryResult.Failure($"Erro interno: {ex.Message}");
        }
    }
}