using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class Business
{
    public Guid Id { get; set; }

    public Guid PartnerId { get; set; }

    public Guid BussinessTypeId { get; set; }

    public decimal Value { get; set; }

    public int Status { get; set; }

    public DateTime Date { get; set; }

    public string Observations { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual BussinessType BussinessType { get; set; } = null!;

    public virtual Comission? Comission { get; set; }

    public virtual Partner Partner { get; set; } = null!;
}
