namespace Core.Common;

public abstract class AuditableEntity
{
    public DateTime Created { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? LastModified { get; set; }

    public string LastModifiedBy { get; set; } = string.Empty;
}
