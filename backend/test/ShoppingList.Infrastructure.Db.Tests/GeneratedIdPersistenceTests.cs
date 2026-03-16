using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using ShoppingList.Infrastructure.Db;

namespace ShoppingList.Infrastructure.Db.Tests;

public class GeneratedIdPersistenceTests(SqlServerFixture fixture) : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture = fixture;

    [Fact]
    public async Task PersistedListAndItemIds_AreStableAfterRoundTrip()
    {
        DbContextOptions<ApplicationContext> options = await _fixture.CreateFreshOptionsAsync();

        Guid listId;
        Guid itemId;

        await using (ApplicationContext context = new(options))
        {
            Domain.Entities.ShoppingList list =
                Domain.Entities.ShoppingList.Create("owner-1");
            Domain.Entities.ShoppingListItem item = list.AddItem("Milk", 2);

            IRepositoryBase<Domain.Entities.ShoppingList> repository =
                new ShoppingListTestRepository(context);

            listId = list.Id;
            itemId = item.Id;

            _ = await repository.AddAsync(list);
        }

        await using (ApplicationContext context = new(options))
        {
            IReadRepositoryBase<Domain.Entities.ShoppingList> repository =
                new ShoppingListTestRepository(context);
            ShoppingListByIdWithItemsSpec specification = new(listId);

            Domain.Entities.ShoppingList? reloaded =
                await repository.FirstOrDefaultAsync(specification);
            reloaded.ShouldNotBeNull();

            Domain.Entities.ShoppingListItem reloadedItem = reloaded.Items.Single();
            reloaded.Id.ShouldBe(listId);
            reloadedItem.Id.ShouldBe(itemId);
        }
    }

    [Fact]
    public async Task SqlServerContainer_CanConnectWithFreshOptions()
    {
        DbContextOptions<ApplicationContext> options = await _fixture.CreateFreshOptionsAsync();

        await using ApplicationContext context = new(options);
        bool canConnect = await context.Database.CanConnectAsync();

        canConnect.ShouldBeTrue();
    }

    [Fact]
    public async Task SchemaMigration_CreatesExpectedTablesOnSqlServer()
    {
        DbContextOptions<ApplicationContext> options = await _fixture.CreateMigrationOptionsAsync();

        await using ApplicationContext context = new(options);
        await context.Database.MigrateAsync();

        int shoppingListsCount = await context.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) AS Value FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ShoppingLists'")
            .SingleAsync();
        int shoppingListItemsCount = await context.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) AS Value FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ShoppingListItems'")
            .SingleAsync();

        shoppingListsCount.ShouldBe(1);
        shoppingListItemsCount.ShouldBe(1);
    }

    private sealed class ShoppingListByIdWithItemsSpec
        : Specification<Domain.Entities.ShoppingList>,
            ISingleResultSpecification<Domain.Entities.ShoppingList>
    {
        public ShoppingListByIdWithItemsSpec(Guid listId)
        {
            Query
                .Where(x => x.Id == listId)
                .Include(x => x.Items);
        }
    }
}
