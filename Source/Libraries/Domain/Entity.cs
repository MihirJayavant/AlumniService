namespace Domain;

public interface IEntity
{
    public int Id { get; }
}

public interface IAuditableEntity : IEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
