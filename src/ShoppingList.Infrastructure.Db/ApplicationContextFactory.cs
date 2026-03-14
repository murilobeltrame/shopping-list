using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShoppingList.Infrastructure.Db;

public sealed class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationContext> optionsBuilder = new();
        string connectionString = Environment.GetEnvironmentVariable("SHOPPINGLIST_DB_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=shoppinglist;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);
        return new ApplicationContext(optionsBuilder.Options);
    }
}