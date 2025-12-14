using Application.UseCases.PartnersReport.DTO;

namespace Application.Interfaces.IUseCases;

public interface IPartnersReportUseCase
{
    Task<PartnersReportResult> ExecuteAsync(
        PartnersReportRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);
}