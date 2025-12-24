using Application.Interfaces.Repositories;
using Domain.Entities;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public sealed class VetorRepository : IVetorRepository
{
    private readonly PartnerMeshDbContext _context;

    public VetorRepository(PartnerMeshDbContext context)
    {
        _context = context;
    }

    public async Task<Vetor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vetores
            .Include(v => v.UserVetores.Where(uv => uv.Active))
            .Include(v => v.Partners)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Vetor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vetores
            .Include(v => v.UserVetores.Where(uv => uv.Active))
            .Include(v => v.Partners)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveAsync(Vetor vetor, CancellationToken cancellationToken = default)
    {
        var existingVetor = await _context.Vetores.FindAsync(new object[] { vetor.Id }, cancellationToken);

        if (existingVetor != null)
        {
            _context.Entry(existingVetor).CurrentValues.SetValues(vetor);
        }
        else
        {
            await _context.Vetores.AddAsync(vetor, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Vetores
            .AnyAsync(v => v.Name == name, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Vetores
            .AnyAsync(v => v.Email == email, cancellationToken);
    }

    public async Task<bool> NameExistsExcludingVetorAsync(string name, Guid excludeVetorId, CancellationToken cancellationToken = default)
    {
        return await _context.Vetores
            .AnyAsync(v => v.Name == name && v.Id != excludeVetorId, cancellationToken);
    }

    public async Task<bool> EmailExistsExcludingVetorAsync(string email, Guid excludeVetorId, CancellationToken cancellationToken = default)
    {
        return await _context.Vetores
            .AnyAsync(v => v.Email == email && v.Id != excludeVetorId, cancellationToken);
    }
}