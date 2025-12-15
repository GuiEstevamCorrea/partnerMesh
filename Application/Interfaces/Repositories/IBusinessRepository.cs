using Domain.Entities;
using Domain.ValueTypes;

namespace Application.Interfaces.Repositories;

public interface IBusinessRepository
{
    Task<Bussiness?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bussiness>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Bussiness>> GetByPartnerIdAsync(Guid partnerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bussiness>> GetByVetorIdAsync(Guid vetorId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Bussiness> businesses, int totalCount)> GetWithFiltersAsync(
        Guid? partnerId = null,
        Guid? businessTypeId = null,
        string? status = null,
        decimal? minValue = null,
        decimal? maxValue = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? searchText = null,
        BusinessSortField sortBy = BusinessSortField.Date,
        SortDirection sortDirection = SortDirection.Descending,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    Task AddAsync(Bussiness business, CancellationToken cancellationToken = default);
    Task UpdateAsync(Bussiness business, CancellationToken cancellationToken = default);
    Task DeleteAsync(Bussiness business, CancellationToken cancellationToken = default);
}