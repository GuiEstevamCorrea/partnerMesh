using Application.UseCases.ListBusinessTypes.DTO;

namespace Application.Interfaces.IUseCases;

public interface IListBusinessTypesUseCase
{
    Task<ListBusinessTypesResult> ExecuteAsync(ListBusinessTypesRequest request, Guid userId);
}