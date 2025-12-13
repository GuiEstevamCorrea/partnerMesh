using Application.UseCases.UpdateBusiness.DTO;

namespace Application.Interfaces.IUseCases;

public interface IUpdateBusinessUseCase
{
    Task<UpdateBusinessResult> ExecuteAsync(Guid businessId, UpdateBusinessRequest request, Guid userId);
}