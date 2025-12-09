using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IVetorRepository
{
    Task<Vetor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Vetor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(Vetor vetor, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}