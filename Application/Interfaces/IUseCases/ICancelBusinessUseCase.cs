using Application.UseCases.CancelBusiness.DTO;

namespace Application.Interfaces.IUseCases;

public interface ICancelBusinessUseCase
{
    Task<CancelBusinessResult> ExecuteAsync(Guid businessId, CancelBusinessRequest request, Guid userId);
}