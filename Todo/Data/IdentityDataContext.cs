using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Todo.Data.Configurations.Identity;
using Todo.Models.Identity;

namespace Todo.Data;

public class IdentityDataContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>,
	UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
	public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options)
	{
	}
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

		modelBuilder.Entity<Role>().ToTable("Roles");
		modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
		modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
		modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
		modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
	}
}