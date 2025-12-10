using Application.UseCases.GetPartnerById.DTO;

namespace Application.Interfaces.IUseCases;

public interface IGetPartnerByIdUseCase
{
    Task<GetPartnerByIdResult> GetByIdAsync(Guid partnerId, Guid currentUserId, CancellationToken cancellationToken = default);
}