using Microsoft.AspNetCore.Identity;

namespace Todo.Models.Identity;

public class UserRole : IdentityUserRole<Guid>
{
	public virtual User User { get; set; }
	public virtual Role Role { get; set; }
}