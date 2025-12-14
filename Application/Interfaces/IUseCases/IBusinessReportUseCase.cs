using Application.UseCases.BusinessReport.DTO;

namespace Application.Interfaces.IUseCases;

public interface IBusinessReportUseCase
{
    Task<BusinessReportResult> ExecuteAsync(BusinessReportRequest request, Guid userId, CancellationToken cancellationToken = default);
}