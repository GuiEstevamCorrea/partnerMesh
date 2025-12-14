using Application.UseCases.LogAudit.DTO;

namespace Application.Interfaces.IUseCases;

public interface ILogAuditUseCase
{
    Task<LogAuditResult> ExecuteAsync(LogAuditRequest request, CancellationToken cancellationToken = default);
}