using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class Vetore
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();

    public virtual ICollection<UserVetor> UserVetors { get; set; } = new List<UserVetor>();
}
