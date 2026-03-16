using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ShoppingList.Infrastructure.Db;
using Testcontainers.MsSql;

namespace ShoppingList.Infrastructure.Db.Tests;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Your_strong_password123!")
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
        return await CreateFreshOptionsAsync(ensureCreated: true);
    }

    public async Task<DbContextOptions<ApplicationContext>> CreateMigrationOptionsAsync()
    {
        return await CreateFreshOptionsAsync(ensureCreated: false);
    }

    private async Task<DbContextOptions<ApplicationContext>> CreateFreshOptionsAsync(bool ensureCreated)
    {
        SqlConnectionStringBuilder connectionStringBuilder = new(_container.GetConnectionString())
        {
            InitialCatalog = "shoppinglist_tests",
            TrustServerCertificate = true
        };

        DbContextOptions<ApplicationContext> options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlServer(connectionStringBuilder.ConnectionString)
            .Options;

        await using ApplicationContext context = new(options);
        await context.Database.EnsureDeletedAsync();
        if (ensureCreated)
        {
            await context.Database.EnsureCreatedAsync();
        }

        return options;
    }
}
