using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int Permission { get; set; }

    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<UserVetor> UserVetors { get; set; } = new List<UserVetor>();
}
