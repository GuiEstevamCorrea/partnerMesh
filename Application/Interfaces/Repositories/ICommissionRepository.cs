using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICommissionRepository
{
    Task<Comission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Comission?> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Comission commission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Comission commission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Comission commission, CancellationToken cancellationToken = default);
}