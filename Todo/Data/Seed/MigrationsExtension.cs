using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Models.Identity;

namespace Todo.Data.Seed;

public static class MigrationsExtension
{
	public static async Task ApplyMigrations(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		try
		{
			var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDataContext>();
			await applicationContext.Database.MigrateAsync();
			
			var identityDataContext = scope.ServiceProvider.GetRequiredService<IdentityDataContext>();
			await identityDataContext.Database.MigrateAsync();
			
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

			await IdentitySeed.SeedDataAsync(userManager, roleManager);
		}
		catch (Exception e)
		{
			var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
			var logger = loggerFactory.CreateLogger(typeof(MigrationsExtension));
			
			logger.LogError(e, "An error occured during seed migrations");
		}
	}
}