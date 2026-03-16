using Microsoft.EntityFrameworkCore;
using Ardalis.Specification;
using Shouldly;
using ShoppingList.Infrastructure.Db;

namespace ShoppingList.Infrastructure.Db.Tests;

public class CopyFlowPersistenceTests(SqlServerFixture fixture) : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture = fixture;

    [Fact]
    public async Task CopiedListAndItemIds_AreStableAfterRoundTrip()
    {
        DbContextOptions<ApplicationContext> options = await _fixture.CreateFreshOptionsAsync();

        Guid copiedListId;
        List<Guid> copiedItemIds;

        await using (ApplicationContext context = new(options))
        {
            Domain.Entities.ShoppingList source =
                Domain.Entities.ShoppingList.Create("owner-1");
            _ = source.AddItem("Milk", 2);
            _ = source.AddItem("Bread", 1);

            Domain.Entities.ShoppingList copied = source.Copy("owner-2");
            IRepositoryBase<Domain.Entities.ShoppingList> repository = new ShoppingListTestRepository(context);

            copiedListId = copied.Id;
            copiedItemIds = copied.Items.Select(x => x.Id).OrderBy(x => x).ToList();

            _ = await repository.AddAsync(copied);
        }

        await using (ApplicationContext context = new(options))
        {
            IReadRepositoryBase<Domain.Entities.ShoppingList> repository = new ShoppingListTestRepository(context);
            ShoppingListByIdWithItemsSpec specification = new(copiedListId);
            Domain.Entities.ShoppingList? reloaded = await repository.FirstOrDefaultAsync(specification);
            reloaded.ShouldNotBeNull();

            List<Guid> reloadedItemIds = reloaded.Items.Select(x => x.Id).OrderBy(x => x).ToList();

            reloaded.Id.ShouldBe(copiedListId);
            reloadedItemIds.ShouldBe(copiedItemIds);
        }
    }

    [Fact]
    public async Task ListItemMutations_PersistAfterRoundTrip()
    {
        DbContextOptions<ApplicationContext> options = await _fixture.CreateFreshOptionsAsync();

        Guid listId;
        Guid purchasedItemId;
        Guid removedItemId;

        await using (ApplicationContext context = new(options))
        {
            Domain.Entities.ShoppingList list = Domain.Entities.ShoppingList.Create("owner-3");
            Domain.Entities.ShoppingListItem purchasedItem = list.AddItem("Milk", 2);
            Domain.Entities.ShoppingListItem removedItem = list.AddItem("Bread", 1);

            list.MarkItemPurchased(purchasedItem.Id);
            list.RemoveItem(removedItem.Id);
            list.UpdateItemQuantity(purchasedItem.Id, 5);

            IRepositoryBase<Domain.Entities.ShoppingList> repository = new ShoppingListTestRepository(context);
            _ = await repository.AddAsync(list);

            listId = list.Id;
            purchasedItemId = purchasedItem.Id;
            removedItemId = removedItem.Id;
        }

        await using (ApplicationContext context = new(options))
        {
            IReadRepositoryBase<Domain.Entities.ShoppingList> repository = new ShoppingListTestRepository(context);
            ShoppingListByIdWithItemsSpec specification = new(listId);
            Domain.Entities.ShoppingList? reloaded = await repository.FirstOrDefaultAsync(specification);

            reloaded.ShouldNotBeNull();
            reloaded.Items.Count.ShouldBe(2);

            Domain.Entities.ShoppingListItem purchased = reloaded.Items.Single(i => i.Id == purchasedItemId);
            Domain.Entities.ShoppingListItem removed = reloaded.Items.Single(i => i.Id == removedItemId);

            purchased.Purchased.ShouldBeTrue();
            purchased.Quantity.ShouldBe(5);
            removed.Removed.ShouldBeTrue();
            reloaded.Finished.ShouldBeTrue();
        }
    }

    private sealed class ShoppingListByIdWithItemsSpec : Specification<Domain.Entities.ShoppingList>, ISingleResultSpecification<Domain.Entities.ShoppingList>
    {
        public ShoppingListByIdWithItemsSpec(Guid listId)
        {
            Query
                .Where(x => x.Id == listId)
                .Include(x => x.Items);
        }
    }
}
