using Application.UseCases.CreatePartner.DTO;

namespace Application.Interfaces.IUseCases;

public interface ICreatePartnerUseCase
{
    Task<CreatePartnerResult> CreateAsync(CreatePartnerRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}