using Application.UseCases.CreateBusinessType.DTO;

namespace Application.Interfaces.IUseCases;

public interface ICreateBusinessTypeUseCase
{
    Task<CreateBusinessTypeResult> CreateAsync(CreateBusinessTypeRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}