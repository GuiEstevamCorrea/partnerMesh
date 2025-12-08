namespace Domain.ValueObjects;

public class BussinessType
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    protected BussinessType() { }

    public BussinessType(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
    }
}
