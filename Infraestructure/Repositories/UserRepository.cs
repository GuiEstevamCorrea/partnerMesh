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

        // Dados de teste - Admin Vetor
        var adminVetor = new User(
            "Admin Vetor",
            "adminvetor@partnermesh.com", 
            BCrypt.Net.BCrypt.HashPassword("123456"),
            PermissionEnum.AdminVetor);
        
        adminVetor.AddVetor(Guid.NewGuid());
        _users.Add(adminVetor);

        // Dados de teste - Operador
        var operador = new User(
            "Operador Sistema",
            "operador@partnermesh.com",
            BCrypt.Net.BCrypt.HashPassword("123456"),
            PermissionEnum.Operador);
        
        operador.AddVetor(Guid.NewGuid());
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
}