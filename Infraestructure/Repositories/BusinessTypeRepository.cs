using Application.Interfaces.Repositories;
using Application.UseCases.ListBusinessTypes.DTO;
using Domain.Entities;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class BusinessTypeRepository : IBusinessTypeRepository
{
    private readonly PartnerMeshDbContext _context;

    public BusinessTypeRepository(PartnerMeshDbContext context)
    {
        _context = context;
    }

    public async Task<BusinessType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes
            .FirstOrDefaultAsync(bt => bt.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<BusinessType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BusinessType>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes
            .Where(bt => bt.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<BusinessType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes
            .FirstOrDefaultAsync(bt => bt.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes
            .AnyAsync(bt => bt.Name == name, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes
            .AnyAsync(bt => bt.Name == name && bt.Id != excludeId, cancellationToken);
    }

    public async Task<(IEnumerable<BusinessType> BusinessTypes, int TotalCount)> GetFilteredAsync(ListBusinessTypesRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.BusinessTypes.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(bt => bt.Name.Contains(request.Name));
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            query = query.Where(bt => bt.Description.Contains(request.Description));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(bt => bt.Active == request.IsActive.Value);
        }

        // Aplicar ordenação
        query = ApplyOrdering(query, request.OrderBy, request.OrderDirection);

        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginação
        var pagedResults = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (pagedResults, totalCount);
    }

    private static IQueryable<BusinessType> ApplyOrdering(IQueryable<BusinessType> query, string? orderBy, string? orderDirection)
    {
        var isDescending = string.Equals(orderDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return orderBy?.ToLowerInvariant() switch
        {
            "name" => isDescending ? query.OrderByDescending(bt => bt.Name) : query.OrderBy(bt => bt.Name),
            "description" => isDescending ? query.OrderByDescending(bt => bt.Description) : query.OrderBy(bt => bt.Description),
            "createdat" => isDescending ? query.OrderByDescending(bt => bt.CreatedAt) : query.OrderBy(bt => bt.CreatedAt),
            "lastmodified" => isDescending ? query.OrderByDescending(bt => bt.LastModified) : query.OrderBy(bt => bt.LastModified),
            "active" => isDescending ? query.OrderByDescending(bt => bt.Active) : query.OrderBy(bt => bt.Active),
            _ => query.OrderBy(bt => bt.Name) // Default ordering
        };
    }

    public async Task AddAsync(BusinessType businessType, CancellationToken cancellationToken = default)
    {
        await _context.BusinessTypes.AddAsync(businessType, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(BusinessType businessType, CancellationToken cancellationToken = default)
    {
        _context.BusinessTypes.Update(businessType);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(BusinessType businessType, CancellationToken cancellationToken = default)
    {
        _context.BusinessTypes.Remove(businessType);
        await _context.SaveChangesAsync(cancellationToken);
    }
}