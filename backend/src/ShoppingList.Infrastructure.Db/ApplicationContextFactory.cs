using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShoppingList.Infrastructure.Db;

public sealed class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationContext> optionsBuilder = new();
        string connectionString = Environment.GetEnvironmentVariable("SHOPPINGLIST_DB_CONNECTION_STRING")
            ?? "Server=localhost,1433;Database=shoppinglist;User Id=sa;Password=Your_strong_password123!;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure();
        });
        return new ApplicationContext(optionsBuilder.Options);
    }
}