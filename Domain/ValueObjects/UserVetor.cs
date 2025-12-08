using Domain.Entities;

namespace Domain.ValueObjects;

public class UserVetor
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid VetorId { get; private set; }
    public Vetor Vetor { get; private set; }

    public bool Active { get; private set; }

    protected UserVetor() { }

    public UserVetor(Guid userId, Guid vetorId)
    {
        UserId = userId;
        VetorId = vetorId;
        Active = true; // Por padrão, a relação usuário-vetor é ativa
    }

    public void Deactivate() => Active = false;
    public void Activate() => Active = true;
}
