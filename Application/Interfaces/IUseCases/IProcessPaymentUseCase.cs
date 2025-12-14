using Application.UseCases.ProcessPayment.DTO;

namespace Application.Interfaces.IUseCases;

public interface IProcessPaymentUseCase
{
    Task<ProcessPaymentResult> ExecuteAsync(
        ProcessPaymentRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);
}