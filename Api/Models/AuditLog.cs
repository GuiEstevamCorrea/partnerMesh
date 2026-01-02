using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class AuditLog
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int Action { get; set; }

    public int Entity { get; set; }

    public Guid EntityId { get; set; }

    public string Datas { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
