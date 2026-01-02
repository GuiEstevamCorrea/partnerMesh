using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class Comission
{
    public Guid Id { get; set; }

    public Guid BussinessId { get; set; }

    public decimal TotalValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Business Bussiness { get; set; } = null!;

    public virtual ICollection<ComissionPayment> ComissionPayments { get; set; } = new List<ComissionPayment>();
}
