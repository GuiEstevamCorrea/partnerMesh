using Application.Interfaces.Repositories;
using Domain.Entities;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly PartnerMeshDbContext _context;

    public UserRepository(PartnerMeshDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserVetores.Where(uv => uv.Active))
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserVetores.Where(uv => uv.Active))
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsExcludingUserAsync(string email, Guid excludeUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email && u.Id != excludeUserId, cancellationToken);
    }

    public async Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        var existingUser = await _context.Users
            .Include(u => u.UserVetores)
            .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        if (existingUser != null)
        {
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            
            // Atualizar UserVetores
            var existingVetores = existingUser.UserVetores.ToList();
            var newVetores = user.UserVetores.ToList();
            
            // Remover os que não existem mais
            foreach (var existingVetor in existingVetores)
            {
                if (!newVetores.Any(nv => nv.UserId == existingVetor.UserId && nv.VetorId == existingVetor.VetorId))
                {
                    _context.Entry(existingVetor).State = EntityState.Deleted;
                }
            }
            
            // Adicionar ou atualizar os novos
            foreach (var newVetor in newVetores)
            {
                var existing = existingVetores.FirstOrDefault(ev => ev.UserId == newVetor.UserId && ev.VetorId == newVetor.VetorId);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(newVetor);
                }
                else
                {
                    _context.Entry(newVetor).State = EntityState.Added;
                }
            }
        }
        else
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserVetores.Where(uv => uv.Active))
                .ThenInclude(uv => uv.Vetor)
            .ToListAsync(cancellationToken);
    }
}