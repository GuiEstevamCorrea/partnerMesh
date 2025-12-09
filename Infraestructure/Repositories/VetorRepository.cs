using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

/// <summary>
/// Implementação temporária em memória do repositório de vetores.
/// Esta implementação será substituída por Entity Framework + PostgreSQL.
/// </summary>
public sealed class VetorRepository : IVetorRepository
{
    private static readonly List<Vetor> _vetores = new();

    static VetorRepository()
    {
        // Criar um vetor padrão para testes
        var vetorPadrao = new Vetor("Vetor Principal", "vetor@partnermesh.com");
        _vetores.Add(vetorPadrao);
        
        // Criar mais um vetor para testes
        var vetorSecundario = new Vetor("Vetor Secundário", "vetor2@partnermesh.com");
        _vetores.Add(vetorSecundario);
    }

    public Task<Vetor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vetor = _vetores.FirstOrDefault(v => v.Id == id);
        return Task.FromResult(vetor);
    }

    public Task<IEnumerable<Vetor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_vetores.AsEnumerable());
    }

    public Task SaveAsync(Vetor vetor, CancellationToken cancellationToken = default)
    {
        var existingVetor = _vetores.FirstOrDefault(v => v.Id == vetor.Id);
        if (existingVetor != null)
        {
            _vetores.Remove(existingVetor);
        }
        
        _vetores.Add(vetor);
        return Task.CompletedTask;
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        var exists = _vetores.Any(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = _vetores.Any(v => v.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    // Método auxiliar para obter o primeiro vetor (para testes)
    public static Guid GetDefaultVetorId()
    {
        return _vetores.First().Id;
    }
}