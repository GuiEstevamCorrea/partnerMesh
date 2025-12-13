using Application.UseCases.CreateBusiness.DTO;

namespace Application.Interfaces.IUseCases;

public interface ICreateBusinessUseCase
{
    Task<CreateBusinessResult> ExecuteAsync(CreateBusinessRequest request, Guid userId);
}