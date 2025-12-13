using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class CommissionRepository : ICommissionRepository
{
    private readonly List<Comission> _commissions = new();

    public Task<Comission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var commission = _commissions.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(commission);
    }

    public Task<Comission?> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        var commission = _commissions.FirstOrDefault(c => c.BussinessId == businessId);
        return Task.FromResult(commission);
    }

    public Task<IEnumerable<Comission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Comission>>(_commissions.ToList());
    }

    public Task AddAsync(Comission commission, CancellationToken cancellationToken = default)
    {
        _commissions.Add(commission);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Comission commission, CancellationToken cancellationToken = default)
    {
        var existing = _commissions.FirstOrDefault(c => c.Id == commission.Id);
        if (existing != null)
        {
            _commissions.Remove(existing);
            _commissions.Add(commission);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Comission commission, CancellationToken cancellationToken = default)
    {
        _commissions.Remove(commission);
        return Task.CompletedTask;
    }
}