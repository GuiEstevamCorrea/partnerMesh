using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class PartnerRepository : IPartnerRepository
{
    private readonly List<Partner> _partners = new();

    public Task<Partner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var partner = _partners.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(partner);
    }

    public Task<IEnumerable<Partner>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Partner>>(_partners.ToList());
    }

    public Task<IEnumerable<Partner>> GetByVetorIdAsync(Guid vetorId, CancellationToken cancellationToken = default)
    {
        var partners = _partners.Where(p => p.VetorId == vetorId).ToList();
        return Task.FromResult<IEnumerable<Partner>>(partners);
    }

    public Task<Partner?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var partner = _partners.FirstOrDefault(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(partner);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = _partners.Any(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task<bool> EmailExistsAsync(string email, Guid excludeId, CancellationToken cancellationToken = default)
    {
        var exists = _partners.Any(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && p.Id != excludeId);
        return Task.FromResult(exists);
    }

    public async Task<IEnumerable<Partner>> GetRecommendationChainAsync(Guid partnerId, CancellationToken cancellationToken = default)
    {
        var chain = new List<Partner>();
        var currentPartnerId = partnerId;

        while (currentPartnerId != Guid.Empty)
        {
            var partner = await GetByIdAsync(currentPartnerId, cancellationToken);
            if (partner == null) break;

            chain.Add(partner);
            currentPartnerId = partner.RecommenderId ?? Guid.Empty;
        }

        return chain;
    }

    public async Task<bool> WouldCreateCycleAsync(Guid partnerId, Guid recommenderId, CancellationToken cancellationToken = default)
    {
        // Se o parceiro vai ser recomendado por alguém que está na sua cadeia de recomendados, seria um ciclo
        var chain = await GetRecommendationChainAsync(recommenderId, cancellationToken);
        return chain.Any(p => p.Id == partnerId);
    }

    public Task AddAsync(Partner partner, CancellationToken cancellationToken = default)
    {
        _partners.Add(partner);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Partner partner, CancellationToken cancellationToken = default)
    {
        var existing = _partners.FirstOrDefault(p => p.Id == partner.Id);
        if (existing != null)
        {
            _partners.Remove(existing);
            _partners.Add(partner);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Partner partner, CancellationToken cancellationToken = default)
    {
        _partners.Remove(partner);
        return Task.CompletedTask;
    }
}