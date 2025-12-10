namespace Domain.Entities;

public class BusinessType
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModified { get; private set; }
    public Guid CreatedBy { get; private set; }
    public Guid? ModifiedBy { get; private set; }

    protected BusinessType() { }

    public BusinessType(string name, string description, Guid createdBy)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Active = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void UpdateInfo(string name, string description, Guid modifiedBy)
    {
        Name = name;
        Description = description;
        LastModified = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Activate(Guid modifiedBy)
    {
        Active = true;
        LastModified = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Deactivate(Guid modifiedBy)
    {
        Active = false;
        LastModified = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}