using Domain.Entities;
using Domain.ValueObjects;
using Domain.ValueTypes;

namespace Application.Interfaces.Repositories;

public interface ICommissionRepository
{
    Task<Comission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Comission?> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Comission commission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Comission commission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Comission commission, CancellationToken cancellationToken = default);

    Task<(IEnumerable<ComissionPayment> payments, int totalCount)> GetPaymentsWithFiltersAsync(
        Guid? vetorId = null,
        Guid? partnerId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? status = null,
        string? tipoPagamento = null,
        PaymentSortField sortBy = PaymentSortField.CreatedAt,
        SortDirection sortDirection = SortDirection.Descending,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}