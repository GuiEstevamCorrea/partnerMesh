using Application.UseCases.GetPartnerTree.DTO;

namespace Application.Interfaces.IUseCases;

public interface IGetPartnerTreeUseCase
{
    Task<GetPartnerTreeResult> GetTreeAsync(GetPartnerTreeRequest request, Guid currentUserId, CancellationToken cancellationToken = default);
}