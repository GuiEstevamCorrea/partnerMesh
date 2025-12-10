using Application.Interfaces.Repositories;
using Application.UseCases.ListPartners.DTO;
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

    public Task<IEnumerable<Partner>> GetRecommendedByPartnerAsync(Guid recommenderId, CancellationToken cancellationToken = default)
    {
        var recommended = _partners.Where(p => p.RecommenderId == recommenderId).ToList();
        return Task.FromResult<IEnumerable<Partner>>(recommended);
    }

    public async Task<bool> WouldCreateCycleAsync(Guid partnerId, Guid recommenderId, CancellationToken cancellationToken = default)
    {
        // Se o parceiro vai ser recomendado por alguém que está na sua cadeia de recomendados, seria um ciclo
        var chain = await GetRecommendationChainAsync(recommenderId, cancellationToken);
        return chain.Any(p => p.Id == partnerId);
    }

    public Task<(IEnumerable<Partner> Partners, int TotalCount)> GetFilteredAsync(ListPartnersRequest request, CancellationToken cancellationToken = default)
    {
        var query = _partners.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(p => p.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(p => p.Email.Contains(request.Email, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            query = query.Where(p => p.PhoneNumber.Contains(request.PhoneNumber, StringComparison.OrdinalIgnoreCase));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.Active == request.IsActive.Value);
        }

        if (request.VetorId.HasValue)
        {
            query = query.Where(p => p.VetorId == request.VetorId.Value);
        }

        if (request.RecommenderId.HasValue)
        {
            query = query.Where(p => p.RecommenderId == request.RecommenderId.Value);
        }

        // Ordenação
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            var orderBy = request.OrderBy.ToLower();
            var orderDirection = request.OrderDirection?.ToLower() == "desc";

            query = orderBy switch
            {
                "name" => orderDirection ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "email" => orderDirection ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
                "createdat" => orderDirection ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };
        }
        else
        {
            query = query.OrderBy(p => p.Name);
        }

        var totalCount = query.Count();

        // Paginação
        var pageSize = Math.Min(request.PageSize, 100); // Máximo 100 itens por página
        var skip = (request.Page - 1) * pageSize;

        var partners = query.Skip(skip).Take(pageSize).ToList();

        return Task.FromResult<(IEnumerable<Partner>, int)>((partners, totalCount));
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