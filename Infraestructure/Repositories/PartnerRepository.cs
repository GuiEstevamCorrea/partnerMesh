using Application.Interfaces.Repositories;
using Application.UseCases.ListPartners.DTO;
using Domain.Entities;
using Domain.ValueTypes;
using Domain.Extensions;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class PartnerRepository : IPartnerRepository
{
    private readonly PartnerMeshDbContext _context;

    public PartnerRepository(PartnerMeshDbContext context)
    {
        _context = context;
    }

    public async Task<Partner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Partners
            .Include(p => p.Vetor)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Partner>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Partners
            .Include(p => p.Vetor)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Partner>> GetByVetorIdAsync(Guid vetorId, CancellationToken cancellationToken = default)
    {
        return await _context.Partners
            .Include(p => p.Vetor)
            .Where(p => p.VetorId == vetorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Partner?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Partners
            .Include(p => p.Vetor)
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Partners
            .AnyAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.Partners
            .AnyAsync(p => p.Email == email && p.Id != excludeId, cancellationToken);
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

    public async Task<IEnumerable<Partner>> GetRecommendedByPartnerAsync(Guid recommenderId, CancellationToken cancellationToken = default)
    {
        return await _context.Partners
            .Include(p => p.Vetor)
            .Where(p => p.RecommenderId == recommenderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> WouldCreateCycleAsync(Guid partnerId, Guid recommenderId, CancellationToken cancellationToken = default)
    {
        // Se o parceiro vai ser recomendado por alguém que está na sua cadeia de recomendados, seria um ciclo
        var chain = await GetRecommendationChainAsync(recommenderId, cancellationToken);
        return chain.Any(p => p.Id == partnerId);
    }

    public async Task<(IEnumerable<Partner> Partners, int TotalCount)> GetFilteredAsync(ListPartnersRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Partners
            .Include(p => p.Vetor)
            .AsQueryable();

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
        var sortField = PartnerSortField.Name;
        var sortDirection = SortDirection.Ascending;
        
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            PartnerSortFieldExtensions.TryParse(request.OrderBy, out sortField);
        }
        
        if (!string.IsNullOrWhiteSpace(request.OrderDirection))
        {
            SortDirectionExtensions.TryParse(request.OrderDirection, out sortDirection);
        }
        
        var isAscending = sortDirection == SortDirection.Ascending;
        
        query = sortField switch
        {
            PartnerSortField.Name => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
            PartnerSortField.Email => isAscending ? query.OrderBy(p => p.Email) : query.OrderByDescending(p => p.Email),
            PartnerSortField.CreatedAt => isAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
            PartnerSortField.Active => isAscending ? query.OrderBy(p => p.Active) : query.OrderByDescending(p => p.Active),
            _ => query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        // Paginação
        var pageSize = Math.Min(request.PageSize, 100); // Máximo 100 itens por página
        var skip = (request.Page - 1) * pageSize;

        var partners = await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

        return (partners, totalCount);
    }

    public async Task AddAsync(Partner partner, CancellationToken cancellationToken = default)
    {
        await _context.Partners.AddAsync(partner, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Partner partner, CancellationToken cancellationToken = default)
    {
        _context.Partners.Update(partner);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Partner partner, CancellationToken cancellationToken = default)
    {
        _context.Partners.Remove(partner);
        await _context.SaveChangesAsync(cancellationToken);
    }
}