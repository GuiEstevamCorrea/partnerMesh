using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueTypes;
using Domain.Extensions;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class BusinessRepository : IBusinessRepository
{
    private readonly PartnerMeshDbContext _context;

    public BusinessRepository(PartnerMeshDbContext context)
    {
        _context = context;
    }

    public async Task<Bussiness?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Businesses
            .Include(b => b.Partner)
            .Include(b => b.BussinessType)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Bussiness>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Businesses
            .Include(b => b.Partner)
            .Include(b => b.BussinessType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bussiness>> GetByPartnerIdAsync(Guid partnerId, CancellationToken cancellationToken = default)
    {
        return await _context.Businesses
            .Include(b => b.Partner)
            .Include(b => b.BussinessType)
            .Where(b => b.PartnerId == partnerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bussiness>> GetByVetorIdAsync(Guid vetorId, CancellationToken cancellationToken = default)
    {
        return await _context.Businesses
            .Include(b => b.Partner)
            .Include(b => b.BussinessType)
            .Where(b => b.Partner.VetorId == vetorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Bussiness> businesses, int totalCount)> GetWithFiltersAsync(
        Guid? partnerId = null,
        Guid? businessTypeId = null,
        string? status = null,
        decimal? minValue = null,
        decimal? maxValue = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? searchText = null,
        BusinessSortField sortBy = BusinessSortField.Date,
        SortDirection sortDirection = SortDirection.Descending,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Businesses
            .Include(b => b.Partner)
            .Include(b => b.BussinessType)
            .AsQueryable();

        // Aplicar filtros
        if (partnerId.HasValue)
            query = query.Where(b => b.PartnerId == partnerId.Value);

        if (businessTypeId.HasValue)
            query = query.Where(b => b.BussinessTypeId == businessTypeId.Value);

        if (!string.IsNullOrEmpty(status))
        {
            if (BusinessStatusExtensions.TryParse(status, out var statusEnum))
            {
                query = query.Where(b => b.Status == statusEnum);
            }
        }

        if (minValue.HasValue)
            query = query.Where(b => b.Value >= minValue.Value);

        if (maxValue.HasValue)
            query = query.Where(b => b.Value <= maxValue.Value);

        if (startDate.HasValue)
            query = query.Where(b => b.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(b => b.Date <= endDate.Value);

        if (!string.IsNullOrEmpty(searchText))
            query = query.Where(b => b.Observations.ToLower().Contains(searchText.ToLower()));

        // Contar total antes da paginação
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar ordenação
        var isAscending = sortDirection == SortDirection.Ascending;
        
        query = sortBy switch
        {
            BusinessSortField.Value => isAscending
                ? query.OrderBy(b => b.Value) 
                : query.OrderByDescending(b => b.Value),
            BusinessSortField.Partner => isAscending
                ? query.OrderBy(b => b.PartnerId) 
                : query.OrderByDescending(b => b.PartnerId),
            BusinessSortField.BusinessType => isAscending
                ? query.OrderBy(b => b.BussinessTypeId) 
                : query.OrderByDescending(b => b.BussinessTypeId),
            BusinessSortField.Status => isAscending
                ? query.OrderBy(b => b.Status) 
                : query.OrderByDescending(b => b.Status),
            _ => isAscending
                ? query.OrderBy(b => b.Date) 
                : query.OrderByDescending(b => b.Date)
        };

        // Aplicar paginação
        var businesses = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (businesses, totalCount);
    }

    public async Task AddAsync(Bussiness business, CancellationToken cancellationToken = default)
    {
        await _context.Businesses.AddAsync(business, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Bussiness business, CancellationToken cancellationToken = default)
    {
        _context.Businesses.Update(business);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Bussiness business, CancellationToken cancellationToken = default)
    {
        _context.Businesses.Remove(business);
        await _context.SaveChangesAsync(cancellationToken);
    }
}