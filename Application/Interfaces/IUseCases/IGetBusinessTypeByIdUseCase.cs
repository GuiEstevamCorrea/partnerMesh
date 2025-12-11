using Application.UseCases.GetBusinessTypeById.DTO;

namespace Application.Interfaces.IUseCases;

public interface IGetBusinessTypeByIdUseCase
{
    Task<GetBusinessTypeByIdResult> ExecuteAsync(Guid businessTypeId, Guid userId);
}