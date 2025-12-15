namespace Domain.ValueTypes;

/// <summary>
/// Define os campos disponíveis para ordenação de pagamentos
/// </summary>
public enum PaymentSortField
{
    CreatedAt = 1,
    Value = 2,
    Status = 3,
    PaidOn = 4
}
