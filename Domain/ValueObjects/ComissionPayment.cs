using Domain.Entities;
using Domain.ValueTypes;

namespace Domain.ValueObjects;

public class ComissionPayment
{
    // Constantes para Tipos de Pagamento (mantidas para compatibilidade legada)
    [Obsolete("Use PaymentType.Vetor com extensão ToLegacyString() em vez disso")]
    public static readonly string VetorPagamento = "vetor";
    [Obsolete("Use PaymentType.Recomendador com extensão ToLegacyString() em vez disso")]
    public static readonly string RecomendadorPagamento = "recomendador";
    [Obsolete("Use PaymentType.Participante com extensão ToLegacyString() em vez disso")]
    public static readonly string ParticipantePagamento = "participante";
    [Obsolete("Use PaymentType.Intermediario com extensão ToLegacyString() em vez disso")]
    public static readonly string IntermediarioPagamento = "intermediario";

    public Guid Id { get; private set; }
    public Guid ComissionId { get; private set; }
    public Comission Comission { get; private set; }

    public Guid PartnerId { get; private set; }
    public PaymentType TipoPagamento { get; private set; }
    public decimal Value { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime? PaidOn { get; private set; }

    protected ComissionPayment() { }

    public ComissionPayment(Guid comissionId, Guid partnerId, decimal value, PaymentType tipoPagamento, PaymentStatus status = PaymentStatus.APagar)
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
    
    public bool IsVetorPayment() => TipoPagamento == PaymentType.Vetor;
    public bool IsRecomendadorPayment() => TipoPagamento == PaymentType.Recomendador;
    public bool IsParticipantePayment() => TipoPagamento == PaymentType.Participante;
    public bool IsIntermediarioPayment() => TipoPagamento == PaymentType.Intermediario;
}
