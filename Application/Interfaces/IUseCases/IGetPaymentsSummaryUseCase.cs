using Application.UseCases.GetPaymentsSummary.DTO;

namespace Application.Interfaces.IUseCases;

public interface IGetPaymentsSummaryUseCase
{
    Task<GetPaymentsSummaryResult> ExecuteAsync(
        GetPaymentsSummaryRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);
}