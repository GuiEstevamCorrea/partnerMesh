namespace Api.Models;

public class ProcessMultiplePaymentsRequest
{
    public List<Guid> PaymentIds { get; set; } = new();
    public string? Observations { get; set; }
}
