using Application.UseCases.FinancialReport.DTO;

namespace Application.Interfaces.IUseCases;

public interface IFinancialReportUseCase
{
    Task<FinancialReportResult> ExecuteAsync(FinancialReportRequest request, Guid userId, CancellationToken cancellationToken = default);
}