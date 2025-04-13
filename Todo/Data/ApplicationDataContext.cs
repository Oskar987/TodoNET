using Microsoft.EntityFrameworkCore;
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
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDataContext).Assembly);
	}
}