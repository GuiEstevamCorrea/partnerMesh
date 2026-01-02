using Domain.ValueObjects;

namespace Domain.Entities;

public class Vetor
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<UserVetor> UserVetores => _userVetores.AsReadOnly();
    private List<UserVetor> _userVetores = new();

    public IReadOnlyCollection<Partner> Partners => _partners.AsReadOnly();
    private List<Partner> _partners = new();

    protected Vetor() { }

    public Vetor(string name, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Active = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }

    public void UpdateEmail(string newEmail)
    {
        Email = newEmail;
    }

    public void Activate()
    {
        Active = true;
    }

    public void Deactivate()
    {
        Active = false;
    }
}
