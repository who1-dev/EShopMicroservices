namespace Ordering.Domain.Abstractions;

public abstract class Entity<T> : IEntity<T>
{
    public T Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreadtedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public DateTime? LastModifiedBy { get; set; }
}
