using Microsoft.EntityFrameworkCore;
using Shouldly;
using ShoppingList.Infrastructure.Db;

namespace ShoppingList.Infrastructure.Db.Tests;

public class CopyFlowPersistenceTests(PostgreSqlFixture fixture) : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture = fixture;

    [Fact]
    public async Task CopiedListAndItemIds_AreStableAfterRoundTrip()
    {
        DbContextOptions<ApplicationContext> options = await _fixture.CreateFreshOptionsAsync();

        Guid copiedListId;
        List<Guid> copiedItemIds;

        await using (ApplicationContext context = new(options))
        {
            await context.Database.EnsureCreatedAsync();

            global::ShoppingList.Domain.Entities.ShoppingList source =
                global::ShoppingList.Domain.Entities.ShoppingList.Create("owner-1");
            _ = source.AddItem("Milk", 2);
            _ = source.AddItem("Bread", 1);

            global::ShoppingList.Domain.Entities.ShoppingList copied = source.Copy("owner-2");
            copiedListId = copied.Id;
            copiedItemIds = copied.Items.Select(x => x.Id).OrderBy(x => x).ToList();

            context.ShoppingLists.Add(copied);
            await context.SaveChangesAsync();
        }

        await using (ApplicationContext context = new(options))
        {
            global::ShoppingList.Domain.Entities.ShoppingList reloaded = await context.ShoppingLists
                .Include(x => x.Items)
                .SingleAsync(x => x.Id == copiedListId);

            List<Guid> reloadedItemIds = reloaded.Items.Select(x => x.Id).OrderBy(x => x).ToList();

            reloaded.Id.ShouldBe(copiedListId);
            reloadedItemIds.ShouldBe(copiedItemIds);
        }
    }
}
