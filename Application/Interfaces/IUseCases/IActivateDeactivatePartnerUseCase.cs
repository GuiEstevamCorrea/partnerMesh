using Application.UseCases.ActivateDeactivatePartner.DTO;

namespace Application.Interfaces.IUseCases;

public interface IActivateDeactivatePartnerUseCase
{
    Task<ActivateDeactivatePartnerResult> ActivateDeactivateAsync(Guid partnerId, ActivateDeactivatePartnerRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}