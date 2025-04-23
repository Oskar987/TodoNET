using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Models.Identity;

namespace Todo.Data.Configurations.Identity;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
	public void Configure(EntityTypeBuilder<UserRole> builder)
	{
		builder.ToTable("UserRoles");
		builder.HasKey(ur => new { ur.UserId, ur.RoleId });
		builder.HasOne(x => x.Role)
			.WithMany(x => x.UserRoles)
			.HasForeignKey(x => x.RoleId)
			.IsRequired();
		builder.HasOne(x => x.User)
			.WithMany(x => x.UserRoles)
			.HasForeignKey(x => x.UserId)
			.IsRequired();
	}
}