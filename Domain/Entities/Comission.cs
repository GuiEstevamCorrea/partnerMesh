using Domain.ValueObjects;

namespace Domain.Entities;

public class Comission
{
    public Guid Id { get; private set; }
    public Guid BussinessId { get; private set; }
    public Bussiness Bussiness { get; private set; }

    public decimal TotalValue { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<ComissionPayment> Payments => _payments.AsReadOnly();
    private readonly List<ComissionPayment> _payments = new();

    protected Comission() { }

    public Comission(Guid bussinessId, decimal totalValue)
    {
        Id = Guid.NewGuid();
        BussinessId = bussinessId;
        TotalValue = totalValue;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddPagamento(ComissionPayment pagamento)
    {
        _payments.Add(pagamento);
    }
}
