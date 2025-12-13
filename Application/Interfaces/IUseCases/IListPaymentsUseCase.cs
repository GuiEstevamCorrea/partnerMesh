using Application.UseCases.ListPayments.DTO;

namespace Application.Interfaces.IUseCases;

public interface IListPaymentsUseCase
{
    Task<ListPaymentsResult> ExecuteAsync(
        ListPaymentsRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);
}