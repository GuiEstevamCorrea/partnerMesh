using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class UserVetor
{
    public Guid UserId { get; set; }

    public Guid VetorId { get; set; }

    public bool Active { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Vetore Vetor { get; set; } = null!;
}
