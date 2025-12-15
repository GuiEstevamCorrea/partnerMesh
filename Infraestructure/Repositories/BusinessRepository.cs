using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueTypes;
using Domain.Extensions;

namespace Infraestructure.Repositories;

public class BusinessRepository : IBusinessRepository
{
    private readonly List<Bussiness> _businesses = new();

    public Task<Bussiness?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var business = _businesses.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(business);
    }

    public Task<IEnumerable<Bussiness>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Bussiness>>(_businesses.ToList());
    }

    public Task<IEnumerable<Bussiness>> GetByPartnerIdAsync(Guid partnerId, CancellationToken cancellationToken = default)
    {
        var businesses = _businesses.Where(b => b.PartnerId == partnerId).ToList();
        return Task.FromResult<IEnumerable<Bussiness>>(businesses);
    }

    public Task<IEnumerable<Bussiness>> GetByVetorIdAsync(Guid vetorId, CancellationToken cancellationToken = default)
    {
        // Para implementar isso, precisaria ter acesso ao PartnerRepository para filtrar por vetor
        // Por agora, retorno lista vazia - implementar quando necessário
        return Task.FromResult<IEnumerable<Bussiness>>(new List<Bussiness>());
    }

    public Task<(IEnumerable<Bussiness> businesses, int totalCount)> GetWithFiltersAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _businesses.AsQueryable();

        // Aplicar filtros
        if (partnerId.HasValue)
            query = query.Where(b => b.PartnerId == partnerId.Value);

        if (businessTypeId.HasValue)
            query = query.Where(b => b.BussinessTypeId == businessTypeId.Value);

        if (!string.IsNullOrEmpty(status))
        {
            if (BusinessStatusExtensions.TryParse(status, out var statusEnum))
            {
                query = query.Where(b => b.Status == statusEnum);
            }
        }

        if (minValue.HasValue)
            query = query.Where(b => b.Value >= minValue.Value);

        if (maxValue.HasValue)
            query = query.Where(b => b.Value <= maxValue.Value);

        if (startDate.HasValue)
            query = query.Where(b => b.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(b => b.Date <= endDate.Value);

        if (!string.IsNullOrEmpty(searchText))
            query = query.Where(b => b.Observations.ToLower().Contains(searchText.ToLower()));

        // Contar total antes da paginação
        var totalCount = query.Count();

        // Aplicar ordenação
        var isAscending = sortDirection == SortDirection.Ascending;
        
        query = sortBy switch
        {
            BusinessSortField.Value => isAscending
                ? query.OrderBy(b => b.Value) 
                : query.OrderByDescending(b => b.Value),
            BusinessSortField.Partner => isAscending
                ? query.OrderBy(b => b.PartnerId) 
                : query.OrderByDescending(b => b.PartnerId),
            BusinessSortField.BusinessType => isAscending
                ? query.OrderBy(b => b.BussinessTypeId) 
                : query.OrderByDescending(b => b.BussinessTypeId),
            BusinessSortField.Status => isAscending
                ? query.OrderBy(b => b.Status) 
                : query.OrderByDescending(b => b.Status),
            _ => isAscending
                ? query.OrderBy(b => b.Date) 
                : query.OrderByDescending(b => b.Date)
        };

        // Aplicar paginação
        var businesses = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<(IEnumerable<Bussiness>, int)>((businesses, totalCount));
    }

    public Task AddAsync(Bussiness business, CancellationToken cancellationToken = default)
    {
        _businesses.Add(business);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Bussiness business, CancellationToken cancellationToken = default)
    {
        var existing = _businesses.FirstOrDefault(b => b.Id == business.Id);
        if (existing != null)
        {
            _businesses.Remove(existing);
            _businesses.Add(business);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Bussiness business, CancellationToken cancellationToken = default)
    {
        _businesses.Remove(business);
        return Task.CompletedTask;
    }
}