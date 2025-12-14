using Application.Interfaces.Repositories;
using Application.UseCases.ListBusinessTypes.DTO;
using Domain.Entities;

namespace Infraestructure.Repositories;

public class BusinessTypeRepository : IBusinessTypeRepository
{
    private readonly List<BusinessType> _businessTypes = new();

    public Task<BusinessType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var businessType = _businessTypes.FirstOrDefault(bt => bt.Id == id);
        return Task.FromResult(businessType);
    }

    public Task<IEnumerable<BusinessType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<BusinessType>>(_businessTypes.ToList());
    }

    public Task<IEnumerable<BusinessType>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var activeTypes = _businessTypes.Where(bt => bt.Active).ToList();
        return Task.FromResult<IEnumerable<BusinessType>>(activeTypes);
    }

    public Task<BusinessType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var businessType = _businessTypes.FirstOrDefault(bt => bt.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(businessType);
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        var exists = _businessTypes.Any(bt => bt.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task<bool> NameExistsAsync(string name, Guid excludeId, CancellationToken cancellationToken = default)
    {
        var exists = _businessTypes.Any(bt => bt.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && bt.Id != excludeId);
        return Task.FromResult(exists);
    }

    public Task<(IEnumerable<BusinessType> BusinessTypes, int TotalCount)> GetFilteredAsync(ListBusinessTypesRequest request, CancellationToken cancellationToken = default)
    {
        var query = _businessTypes.AsEnumerable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(bt => bt.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            query = query.Where(bt => bt.Description.Contains(request.Description, StringComparison.OrdinalIgnoreCase));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(bt => bt.Active == request.IsActive.Value);
        }

        // Aplicar ordenação
        query = ApplyOrdering(query, request.OrderBy, request.OrderDirection);

        var totalCount = query.Count();

        // Aplicar paginação
        var pagedResults = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Task.FromResult<(IEnumerable<BusinessType>, int)>((pagedResults, totalCount));
    }

    private static IEnumerable<BusinessType> ApplyOrdering(IEnumerable<BusinessType> query, string? orderBy, string? orderDirection)
    {
        var isDescending = string.Equals(orderDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return orderBy?.ToLowerInvariant() switch
        {
            "name" => isDescending ? query.OrderByDescending(bt => bt.Name) : query.OrderBy(bt => bt.Name),
            "description" => isDescending ? query.OrderByDescending(bt => bt.Description) : query.OrderBy(bt => bt.Description),
            "createdat" => isDescending ? query.OrderByDescending(bt => bt.CreatedAt) : query.OrderBy(bt => bt.CreatedAt),
            "lastmodified" => isDescending ? query.OrderByDescending(bt => bt.LastModified) : query.OrderBy(bt => bt.LastModified),
            "active" => isDescending ? query.OrderByDescending(bt => bt.Active) : query.OrderBy(bt => bt.Active),
            _ => query.OrderBy(bt => bt.Name) // Default ordering
        };
    }

    public Task AddAsync(BusinessType businessType, CancellationToken cancellationToken = default)
    {
        _businessTypes.Add(businessType);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(BusinessType businessType, CancellationToken cancellationToken = default)
    {
        var existing = _businessTypes.FirstOrDefault(bt => bt.Id == businessType.Id);
        if (existing != null)
        {
            _businessTypes.Remove(existing);
            _businessTypes.Add(businessType);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(BusinessType businessType, CancellationToken cancellationToken = default)
    {
        _businessTypes.Remove(businessType);
        return Task.CompletedTask;
    }
}