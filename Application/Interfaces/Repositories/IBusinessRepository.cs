using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IBusinessRepository
{
    Task<Bussiness?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bussiness>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Bussiness>> GetByPartnerIdAsync(Guid partnerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bussiness>> GetByVetorIdAsync(Guid vetorId, CancellationToken cancellationToken = default);
    Task AddAsync(Bussiness business, CancellationToken cancellationToken = default);
    Task UpdateAsync(Bussiness business, CancellationToken cancellationToken = default);
    Task DeleteAsync(Bussiness business, CancellationToken cancellationToken = default);
}