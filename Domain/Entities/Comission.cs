using Domain.ValueObjects;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Comission
{
    public Guid Id { get; private set; }
    public Guid BussinessId { get; private set; }
    public Bussiness Bussiness { get; private set; }

    public decimal TotalValue { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<ComissionPayment> Pagamentos => _payments.AsReadOnly();
    private List<ComissionPayment> _payments = new();

    protected Comission() { }

    public Comission(Guid bussinessId, decimal totalValue)
    {
        Id = Guid.NewGuid();
        BussinessId = bussinessId;
        TotalValue = totalValue;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddPagamento(Guid partnerId, decimal value, PaymentType tipoPagamento, PaymentStatus status = PaymentStatus.APagar)
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
        foreach (var payment in _payments.Where(p => p.Status == ValueTypes.PaymentStatus.APagar))
        {
            payment.CancelPayment();
        }
    }

    /// <summary>
    /// Cancela TODOS os pagamentos da comissão (pendentes e pagos)
    /// Usado quando um negócio é cancelado
    /// </summary>
    public void CancelAllPayments()
    {
        foreach (var payment in _payments)
        {
            payment.CancelPayment();
        }
    }

    public int GetPendingPaymentsCount()
    {
        return _payments.Count(p => p.Status == ValueTypes.PaymentStatus.APagar);
    }

    public int GetPaidPaymentsCount()
    {
        return _payments.Count(p => p.Status == ValueTypes.PaymentStatus.Pago);
    }

    public decimal GetPendingPaymentsValue()
    {
        return _payments.Where(p => p.Status == ValueTypes.PaymentStatus.APagar).Sum(p => p.Value);
    }

    public decimal GetPaidPaymentsValue()
    {
        return _payments.Where(p => p.Status == ValueTypes.PaymentStatus.Pago).Sum(p => p.Value);
    }
}
