namespace Todo.Models;

public class BaseEntity<TId>
	where TId : IEquatable<TId>
{
	public BaseEntity()
	{
		CreatedAt = DateTime.UtcNow;
	}
	public TId Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime DeletedAt { get; set; }
}