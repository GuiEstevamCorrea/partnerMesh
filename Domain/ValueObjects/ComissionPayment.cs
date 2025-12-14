using Domain.Entities;
using Domain.ValueTypes;

namespace Domain.ValueObjects;

public class ComissionPayment
{
    // Constantes para Tipos de Pagamento
    public static readonly string VetorPagamento = "vetor";
    public static readonly string RecomendadorPagamento = "recomendador";
    public static readonly string ParticipantePagamento = "participante";
    public static readonly string IntermediarioPagamento = "intermediario";

    public Guid Id { get; private set; }
    public Guid ComissionId { get; private set; }
    public Comission Comission { get; private set; }

    public Guid PartnerId { get; private set; }
    public string TipoPagamento { get; private set; }
    public decimal Value { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime? PaidOn { get; private set; }

    protected ComissionPayment() { }

    public ComissionPayment(Guid comissionId, Guid partnerId, decimal value, string tipoPagamento, PaymentStatus status = PaymentStatus.APagar)
    {
        Id = Guid.NewGuid();
        ComissionId = comissionId;
        PartnerId = partnerId;
        Value = value;
        TipoPagamento = tipoPagamento;
        Status = status;
    }

    public void UpdateStatusToPaid()
    {
        Status = PaymentStatus.Pago;
        PaidOn = DateTime.UtcNow;
    }

    public void CancelPayment()
    {
        if (Status == PaymentStatus.APagar)
        {
            Status = PaymentStatus.Cancelado;
        }
    }
    
    public bool IsPaid() => Status == PaymentStatus.Pago;
    public bool IsPending() => Status == PaymentStatus.APagar;
    public bool IsCanceled() => Status == PaymentStatus.Cancelado;
}
