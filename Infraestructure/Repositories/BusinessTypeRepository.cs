using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

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