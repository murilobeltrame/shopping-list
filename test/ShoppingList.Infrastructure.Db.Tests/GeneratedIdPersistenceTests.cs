using Microsoft.EntityFrameworkCore;
using Shouldly;
using ShoppingList.Infrastructure.Db;

namespace ShoppingList.Infrastructure.Db.Tests;

public class GeneratedIdPersistenceTests(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture = fixture;

    [Fact]
    public async Task PersistedListAndItemIds_AreStableAfterRoundTrip()
    {
        DbContextOptions<ApplicationContext> options = await _fixture.CreateFreshOptionsAsync();

        Guid listId;
        Guid itemId;

        await using (ApplicationContext context = new(options))
        {
            await context.Database.EnsureCreatedAsync();

            global::ShoppingList.Domain.Entities.ShoppingList list =
                global::ShoppingList.Domain.Entities.ShoppingList.Create("owner-1");
            global::ShoppingList.Domain.Entities.ShoppingListItem item = list.AddItem("Milk", 2);

            listId = list.Id;
            itemId = item.Id;

            context.ShoppingLists.Add(list);
            await context.SaveChangesAsync();
        }

        await using (ApplicationContext context = new(options))
        {
            global::ShoppingList.Domain.Entities.ShoppingList reloaded = await context.ShoppingLists
                .Include(x => x.Items)
                .SingleAsync(x => x.Id == listId);

            global::ShoppingList.Domain.Entities.ShoppingListItem reloadedItem = reloaded.Items.Single();

            reloaded.Id.ShouldBe(listId);
            reloadedItem.Id.ShouldBe(itemId);
        }
    }
}
