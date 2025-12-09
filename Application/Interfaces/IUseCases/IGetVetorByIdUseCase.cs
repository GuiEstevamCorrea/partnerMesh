using Application.UseCases.GetVetorById.DTO;

namespace Application.Interfaces.IUseCases;

public interface IGetVetorByIdUseCase
{
    Task<GetVetorByIdResult> GetByIdAsync(GetVetorByIdRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}