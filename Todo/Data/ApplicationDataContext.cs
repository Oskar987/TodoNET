using Microsoft.EntityFrameworkCore;
using Todo.Data.Configurations;
using Todo.Models;

namespace Todo.Data;

public class ApplicationDataContext : DbContext
{
	public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options)
	{
	}

	public DbSet<TodoItem> TodoItems { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new TodoItemConfiguration());
	}
}