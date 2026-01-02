namespace Domain.Entities;

public class Partner
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Guid VetorId { get; private set; }
    public Vetor Vetor { get; private set; }

    public Guid? RecommenderId { get; private set; }
    public Partner? Recommender { get; private set; }

    public IReadOnlyCollection<Partner> Recommended => _recommended.AsReadOnly();
    private List<Partner> _recommended = new();

    protected Partner() { }

    public Partner(string name, string phoneNumber, string email, Guid vetorId, Guid? recommenderId)
    {
        Id = Guid.NewGuid();
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        VetorId = vetorId;
        RecommenderId = recommenderId;
        Active = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string name, string phoneNumber, string email)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public void UpdateRecommender(Guid? recommenderId)
    {
        RecommenderId = recommenderId;
    }

    public void Activate()
    {
        Active = true;
    }

    public void Deactivate()
    {
        Active = false;
    }

    public bool CanRecommend()
    {
        return Active;
    }
}
