using Domain.ValueObjects;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Bussiness
{
    public Guid Id { get; private set; }
    public Guid PartnerId { get; private set; }
    public Partner Partner { get; private set; }

    public Guid BussinessTypeId { get; private set; }
    public BusinessType BussinessType { get; private set; }

    public decimal Value { get; private set; }
    public BusinessStatus Status { get; private set; }
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
        Status = BusinessStatus.Ativo;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateValue(decimal newValue)
    {
        if (newValue <= 0)
            throw new ArgumentException("Valor deve ser positivo", nameof(newValue));
        
        Value = newValue;
    }

    public void UpdateObservations(string newObservations)
    {
        Observations = newObservations ?? string.Empty;
    }

    public void CancelBusiness()
    {
        Status = BusinessStatus.Cancelado;
    }

    /// <summary>
    /// Atualiza o status do negócio baseado nos pagamentos da comissão
    /// </summary>
    public void UpdateStatusBasedOnPayments()
    {
        // Se já está cancelado, não muda
        if (Status == BusinessStatus.Cancelado)
            return;

        // Se não tem comissão, mantém como ativo
        if (Comissao == null)
        {
            Status = BusinessStatus.Ativo;
            return;
        }

        var totalPayments = Comissao.Pagamentos.Count;
        var paidPayments = Comissao.GetPaidPaymentsCount();

        if (totalPayments == 0)
        {
            Status = BusinessStatus.Ativo;
        }
        else if (paidPayments == 0)
        {
            Status = BusinessStatus.Ativo;
        }
        else if (paidPayments == totalPayments)
        {
            Status = BusinessStatus.TotalmentePago;
        }
        else
        {
            Status = BusinessStatus.ParcialmentePago;
        }
    }
    
    public bool IsActive() => Status == BusinessStatus.Ativo;
    public bool IsCanceled() => Status == BusinessStatus.Cancelado;
    public bool IsPartiallyPaid() => Status == BusinessStatus.ParcialmentePago;
    public bool IsFullyPaid() => Status == BusinessStatus.TotalmentePago;
}
