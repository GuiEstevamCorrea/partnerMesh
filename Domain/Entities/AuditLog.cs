using Domain.ValueTypes;

namespace Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public AuditAction Action { get; private set; }
    public AuditEntityType Entity { get; private set; }
    public Guid EntityId { get; private set; }
    public string Datas { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected AuditLog() { }

    public AuditLog(Guid userId, AuditAction action, AuditEntityType entity, Guid entityId, string datas)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Action = action;
        Entity = entity;
        EntityId = entityId;
        Datas = datas;
        CreatedAt = DateTime.UtcNow;
    }
}
