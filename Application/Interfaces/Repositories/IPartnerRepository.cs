using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPartnerRepository
{
    Task<Partner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Partner>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Partner>> GetByVetorIdAsync(Guid vetorId, CancellationToken cancellationToken = default);
    Task<Partner?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid excludeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Partner>> GetRecommendationChainAsync(Guid partnerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Partner>> GetRecommendedByPartnerAsync(Guid recommenderId, CancellationToken cancellationToken = default);
    Task<bool> WouldCreateCycleAsync(Guid partnerId, Guid recommenderId, CancellationToken cancellationToken = default);
    Task AddAsync(Partner partner, CancellationToken cancellationToken = default);
    Task UpdateAsync(Partner partner, CancellationToken cancellationToken = default);
    Task DeleteAsync(Partner partner, CancellationToken cancellationToken = default);
}