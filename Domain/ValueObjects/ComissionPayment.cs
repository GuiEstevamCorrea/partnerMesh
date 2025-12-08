using Domain.Entities;

namespace Domain.ValueObjects;

public class ComissionPayment
{
    public Guid Id { get; private set; }
    public Guid ComissionId { get; private set; }
    public Comission Comission { get; private set; }

    public Guid ReceiverId { get; private set; }
    public string ReceiverType { get; private set; } // "vetor" ou "parceiro"  // TO-DO: Implementar tipo tipado

    public int Level { get; private set; }
    public decimal Value { get; private set; }

    public string Status { get; private set; } // "a_pagar" / "pago"    // TO-DO: Implementar status tipado
    public DateTime? PaidOn { get; private set; }

    protected ComissionPayment() { }

    public ComissionPayment(Guid comissionId, Guid receiverId, string receiverType, int level, decimal value)
    {
        Id = Guid.NewGuid();
        ComissionId = comissionId;
        ReceiverId = receiverId;
        ReceiverType = receiverType;
        Level = level;
        Value = value;
        Status = "a_pagar";
    }

    public void UpdateStatusToPaid()
    {
        Status = "pago";
        PaidOn = DateTime.UtcNow;
    }
}
