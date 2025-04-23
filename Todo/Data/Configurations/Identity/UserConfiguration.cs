using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Models.Identity;

namespace Todo.Data.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");
		builder.HasMany(x => x.UserRoles)
			.WithOne()
			.HasForeignKey(x => x.UserId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);
	}
}