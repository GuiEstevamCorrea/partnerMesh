using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

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