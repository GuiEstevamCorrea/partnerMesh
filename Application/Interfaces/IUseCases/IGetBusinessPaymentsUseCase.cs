using Application.UseCases.GetBusinessPayments.DTO;

namespace Application.Interfaces.IUseCases;

public interface IGetBusinessPaymentsUseCase
{
    Task<GetBusinessPaymentsResult> ExecuteAsync(
        Guid businessId,
        Guid userId,
        CancellationToken cancellationToken = default);
}