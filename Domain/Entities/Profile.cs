namespace Domain.Entities;

public class Profile
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    protected Profile() { }

    public Profile(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}
