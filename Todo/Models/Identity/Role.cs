using Microsoft.AspNetCore.Identity;

namespace Todo.Models.Identity;

public class Role : IdentityRole<Guid>
{
	public ICollection<UserRole> UserRoles { get; set; }
}