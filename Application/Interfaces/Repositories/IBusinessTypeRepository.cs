using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IBusinessTypeRepository
{
    Task<BusinessType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BusinessType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BusinessType>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<BusinessType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(BusinessType businessType, CancellationToken cancellationToken = default);
    Task UpdateAsync(BusinessType businessType, CancellationToken cancellationToken = default);
    Task DeleteAsync(BusinessType businessType, CancellationToken cancellationToken = default);
}