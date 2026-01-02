using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class ComissionPayment
{
    public Guid Id { get; set; }

    public Guid ComissionId { get; set; }

    public Guid PartnerId { get; set; }

    public int TipoPagamento { get; set; }

    public decimal Value { get; set; }

    public int Status { get; set; }

    public DateTime? PaidOn { get; set; }

    public virtual Comission Comission { get; set; } = null!;
}
