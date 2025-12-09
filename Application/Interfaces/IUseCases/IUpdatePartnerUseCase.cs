using Application.UseCases.UpdatePartner.DTO;

namespace Application.Interfaces.IUseCases;

public interface IUpdatePartnerUseCase
{
    Task<UpdatePartnerResult> UpdateAsync(Guid partnerId, UpdatePartnerRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}