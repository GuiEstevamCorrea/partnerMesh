using Application.UseCases.ListPartners.DTO;

namespace Application.Interfaces.IUseCases;

public interface IListPartnersUseCase
{
    Task<ListPartnersResult> ListAsync(ListPartnersRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}