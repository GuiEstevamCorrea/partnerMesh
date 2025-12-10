using Application.UseCases.DeactivateBusinessType.DTO;

namespace Application.Interfaces.IUseCases;

public interface IDeactivateBusinessTypeUseCase
{
    Task<DeactivateBusinessTypeResult> ExecuteAsync(Guid businessTypeId, Guid userId);
}