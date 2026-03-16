using NSubstitute;
using Shouldly;
using ShoppingList.Application.Commands;
using ShoppingList.Application.Handlers;
using Ardalis.Specification;

namespace ShoppingList.Application.Tests.Handlers;

public class CopyListHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CopiesListAndReturnsNewId()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Domain.Entities.ShoppingList source = Domain.Entities.ShoppingList.Create("owner-1");
        source.AddItem("Milk", 2);
        repository.GetByIdAsync(source.Id).Returns(source);

        CopyListHandler handler = new(repository);
        CopyListCommand command = new(source.Id, "owner-2", null);

        Guid copiedListId = await handler.Handle(command);

        copiedListId.ShouldNotBe(Guid.Empty);
        copiedListId.ShouldNotBe(source.Id);
        await repository.Received(1).AddAsync(Arg.Is<Domain.Entities.ShoppingList>(l => l.Id == copiedListId && l.Owner == "owner-2"));
    }

    [Fact]
    public async Task Handle_CopiedItemsResetToNotCompleted()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Domain.Entities.ShoppingList source = Domain.Entities.ShoppingList.Create("owner-1");
        Domain.Entities.ShoppingListItem sourceItem = source.AddItem("Milk", 2);
        source.MarkItemPurchased(sourceItem.Id);
        repository.GetByIdAsync(source.Id).Returns(source);

        CopyListHandler handler = new(repository);
        _ = await handler.Handle(new CopyListCommand(source.Id, "owner-2", null));

        await repository.Received(1).AddAsync(Arg.Is<Domain.Entities.ShoppingList>(l =>
            l.Owner == "owner-2" &&
            l.Items.Count == 1 &&
            l.Items.Single().Purchased == false &&
            l.Items.Single().Removed == false));
    }

    [Fact]
    public async Task Handle_SourceListNotFound_ThrowsInvalidOperationException()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Guid sourceListId = Guid.NewGuid();
        repository.GetByIdAsync(sourceListId).Returns((Domain.Entities.ShoppingList?)null);
        CopyListHandler handler = new(repository);

        await Should.ThrowAsync<InvalidOperationException>(async () => await handler.Handle(new CopyListCommand(sourceListId, "owner-2", null)));
    }

    [Fact]
    public async Task Handle_WithInvalidNewOwner_PropagatesArgumentException()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Domain.Entities.ShoppingList source = Domain.Entities.ShoppingList.Create("owner-1");
        repository.GetByIdAsync(source.Id).Returns(source);
        CopyListHandler handler = new(repository);

        await Should.ThrowAsync<ArgumentException>(async () => await handler.Handle(new CopyListCommand(source.Id, "  ", null)));
        await repository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.ShoppingList>());
    }
}