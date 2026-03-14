using Microsoft.EntityFrameworkCore;
using Ardalis.Specification;
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
