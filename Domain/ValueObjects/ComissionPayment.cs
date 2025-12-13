using Domain.Entities;

namespace Domain.ValueObjects;

public class ComissionPayment
{
    // Constantes para Tipos de Pagamento
    public static readonly string VetorPagamento = "vetor";
    public static readonly string RecomendadorPagamento = "recomendador";
    public static readonly string ParticipantePagamento = "participante";
    public static readonly string IntermediarioPagamento = "intermediario";

    // Constantes para Status
    public static readonly string APagar = "a_pagar";
    public static readonly string Pago = "pago";
    public static readonly string Cancelado = "cancelado";

    public Guid Id { get; private set; }
    public Guid ComissionId { get; private set; }
    public Comission Comission { get; private set; }

    public Guid PartnerId { get; private set; } // Mudei de ReceiverId para PartnerId
    public string TipoPagamento { get; private set; } // Mudei de ReceiverType para TipoPagamento
    public decimal Value { get; private set; }
    public string Status { get; private set; }
    public DateTime? PaidOn { get; private set; }

    protected ComissionPayment() { }

    public ComissionPayment(Guid comissionId, Guid partnerId, decimal value, string tipoPagamento, string status)
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
        Status = Pago;
        PaidOn = DateTime.UtcNow;
    }

    public void CancelPayment()
    {
        if (Status == APagar)
        {
            Status = Cancelado;
        }
    }
}
