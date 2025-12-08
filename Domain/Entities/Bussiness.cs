using Domain.ValueObjects;

namespace Domain.Entities;

public class Bussiness
{
    public Guid Id { get; private set; }
    public Guid PartnerId { get; private set; }
    public Partner Partner { get; private set; }

    public Guid BussinessTypeId { get; private set; }
    public BussinessType BussinessType { get; private set; }

    public decimal Value { get; private set; }
    public string Status { get; private set; } // TO-DO: Implementar status tipado
    public DateTime Date { get; private set; }
    public string Observations { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Comission Comissao { get; private set; }

    protected Bussiness() { }

    public Bussiness(Guid partnerId, Guid bussinessTypeId, decimal value, string observations)
    {
        Id = Guid.NewGuid();
        PartnerId = partnerId;
        BussinessTypeId = bussinessTypeId;
        Value = value;
        Date = DateTime.UtcNow;
        Observations = observations;
        Status = "ativo";
        CreatedAt = DateTime.UtcNow;
    }
}
