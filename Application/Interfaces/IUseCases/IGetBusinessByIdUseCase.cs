using Application.UseCases.GetBusinessById.DTO;

namespace Application.Interfaces.IUseCases;

public interface IGetBusinessByIdUseCase
{
    Task<GetBusinessByIdResult> ExecuteAsync(Guid businessId, Guid userId);
}