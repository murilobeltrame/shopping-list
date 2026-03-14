using Microsoft.EntityFrameworkCore;

namespace ShoppingList.Infrastructure.Db;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
	public DbSet<global::ShoppingList.Domain.Entities.ShoppingList> ShoppingLists => Set<global::ShoppingList.Domain.Entities.ShoppingList>();

	public DbSet<global::ShoppingList.Domain.Entities.ShoppingListItem> ShoppingListItems => Set<global::ShoppingList.Domain.Entities.ShoppingListItem>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
	}
}
