using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Models.Identity;

namespace Todo.Data.Seed;

public class IdentitySeed
{
	public static async Task SeedDataAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
	{
		foreach (var roleName in Enum.GetNames(typeof(Roles)))
		{
			if (!await roleManager.RoleExistsAsync(roleName))
			{
				await roleManager.CreateAsync(new Role { Name = roleName });
			}
		}

		if (!await userManager.Users.AnyAsync())
		{
			var user = new User
			{
				UserName = "User",
				Email = "user@todo.ru"
			};
			
			var admin = new User
			{
				UserName = "Admin",
				Email = "admin@todo.ru"
			};

			await userManager.CreateAsync(user, "qwertyX123!");
			await userManager.CreateAsync(admin, "qwertyX123!");

			await userManager.AddToRoleAsync(user, Roles.User.ToString());
			await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
			await userManager.AddToRoleAsync(admin, Roles.User.ToString());
		}
	}
}