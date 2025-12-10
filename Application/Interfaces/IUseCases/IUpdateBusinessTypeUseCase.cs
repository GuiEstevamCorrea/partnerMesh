using Application.UseCases.UpdateBusinessType.DTO;

namespace Application.Interfaces.IUseCases;

public interface IUpdateBusinessTypeUseCase
{
    Task<UpdateBusinessTypeResult> ExecuteAsync(Guid businessTypeId, UpdateBusinessTypeRequest request, Guid userId);
}