using Application.UseCases.GetCancelledBusinessSummary.DTO;

namespace Application.Interfaces.IUseCases;

public interface ICancelledBusinessSummaryUseCase
{
    Task<CancelledBusinessSummaryResult> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}