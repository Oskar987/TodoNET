using Microsoft.AspNetCore.Identity;

namespace Todo.Models.Identity;

public class User : IdentityUser<Guid>
{
	public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
}