using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueTypes;

namespace Infrastructure.Repositories;

/// <summary>
/// Implementação temporária em memória do repositório de usuários.
/// Esta implementação será substituída por Entity Framework + PostgreSQL.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private static readonly List<User> _users = new();

    static UserRepository()
    {
        // Dados de teste - Admin Global
        var adminGlobal = new User(
            "Admin Global",
            "admin@partnermesh.com",
            BCrypt.Net.BCrypt.HashPassword("123456"),
            PermissionEnum.AdminGlobal);

        _users.Add(adminGlobal);

        // Obter ID do vetor padrão
        var defaultVetorId = VetorRepository.GetDefaultVetorId();

        // Dados de teste - Admin Vetor
        var adminVetor = new User(
            "Admin Vetor",
            "adminvetor@partnermesh.com", 
            BCrypt.Net.BCrypt.HashPassword("123456"),
            PermissionEnum.AdminVetor);
        
        adminVetor.AddVetor(defaultVetorId);
        _users.Add(adminVetor);

        // Dados de teste - Operador
        var operador = new User(
            "Operador Sistema",
            "operador@partnermesh.com",
            BCrypt.Net.BCrypt.HashPassword("123456"),
            PermissionEnum.Operador);
        
        operador.AddVetor(defaultVetorId);
        _users.Add(operador);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task<bool> EmailExistsExcludingUserAsync(string email, Guid excludeUserId, CancellationToken cancellationToken = default)
    {
        var exists = _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Id != excludeUserId);
        return Task.FromResult(exists);
    }

    public Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            _users.Remove(existingUser);
        }
        
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_users.AsEnumerable());
    }
}