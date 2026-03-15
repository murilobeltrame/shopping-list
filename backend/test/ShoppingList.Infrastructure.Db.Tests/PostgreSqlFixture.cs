using Microsoft.EntityFrameworkCore;
using ShoppingList.Infrastructure.Db;
using Testcontainers.PostgreSql;

namespace ShoppingList.Infrastructure.Db.Tests;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("shoppinglist_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public async Task<DbContextOptions<ApplicationContext>> CreateFreshOptionsAsync()
    {
        DbContextOptions<ApplicationContext> options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        await using ApplicationContext context = new(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        return options;
    }
}
