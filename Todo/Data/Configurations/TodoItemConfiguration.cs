using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Models;

namespace Todo.Data.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
	public void Configure(EntityTypeBuilder<TodoItem> builder)
	{
		builder.ToTable("TodoItems");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
		builder.Property(x => x.Description).HasMaxLength(500);
		
		builder.Property(x => x.IsDone);
		builder.Property(x => x.CreatedAt).IsRequired();
		builder.Property(x => x.DeletedAt);

		builder.HasIndex(x => x.Title);
	}
}