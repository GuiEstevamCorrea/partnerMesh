using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class BussinessType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Business> Businesses { get; set; } = new List<Business>();
}
