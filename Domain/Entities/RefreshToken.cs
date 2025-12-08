namespace Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public bool IsUsed { get; private set; }

    protected RefreshToken() { }

    public RefreshToken(string token, Guid userId, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
        IsUsed = false;
    }

    public bool IsValid()
    {
        return !IsRevoked && !IsUsed && ExpiresAt > DateTime.UtcNow;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
}