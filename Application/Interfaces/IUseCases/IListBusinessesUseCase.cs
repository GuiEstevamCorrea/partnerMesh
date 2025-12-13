using Application.UseCases.ListBusinesses.DTO;

namespace Application.Interfaces.IUseCases;

public interface IListBusinessesUseCase
{
    Task<ListBusinessesResult> ExecuteAsync(ListBusinessesRequest request, Guid userId);
}