using Domain.ValueObjects;

namespace Domain.Entities;

public class Comission
{
    public Guid Id { get; private set; }
    public Guid BussinessId { get; private set; }
    public Bussiness Bussiness { get; private set; }

    public decimal TotalValue { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<ComissionPayment> Pagamentos => _payments.AsReadOnly();
    private readonly List<ComissionPayment> _payments = new();

    protected Comission() { }

    public Comission(Guid bussinessId, decimal totalValue)
    {
        Id = Guid.NewGuid();
        BussinessId = bussinessId;
        TotalValue = totalValue;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddPagamento(Guid partnerId, decimal value, string tipoPagamento, string status)
    {
        var pagamento = new ComissionPayment(Id, partnerId, value, tipoPagamento, status);
        _payments.Add(pagamento);
    }

    public void AddPagamento(ComissionPayment pagamento)
    {
        _payments.Add(pagamento);
    }

    public void CancelPendingPayments()
    {
        foreach (var payment in _payments.Where(p => p.Status == ComissionPayment.APagar))
        {
            payment.CancelPayment();
        }
    }

    public int GetPendingPaymentsCount()
    {
        return _payments.Count(p => p.Status == ComissionPayment.APagar);
    }

    public int GetPaidPaymentsCount()
    {
        return _payments.Count(p => p.Status == ComissionPayment.Pago);
    }

    public decimal GetPendingPaymentsValue()
    {
        return _payments.Where(p => p.Status == ComissionPayment.APagar).Sum(p => p.Value);
    }

    public decimal GetPaidPaymentsValue()
    {
        return _payments.Where(p => p.Status == ComissionPayment.Pago).Sum(p => p.Value);
    }
}
