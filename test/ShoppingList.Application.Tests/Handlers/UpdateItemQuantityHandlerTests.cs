using NSubstitute;
using Shouldly;
using ShoppingList.Application.Commands;
using ShoppingList.Application.Handlers;
using Ardalis.Specification;

namespace ShoppingList.Application.Tests.Handlers;

public class UpdateItemQuantityHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_UpdatesQuantity()
    {
        IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>>();
        global::ShoppingList.Domain.Entities.ShoppingList shoppingList = global::ShoppingList.Domain.Entities.ShoppingList.Create("owner-1");
        global::ShoppingList.Domain.Entities.ShoppingListItem item = shoppingList.AddItem("Milk", 1);
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        UpdateItemQuantityHandler handler = new(repository);
        UpdateItemQuantityCommand command = new(shoppingList.Id, item.Id, 5);

        await handler.Handle(command);

        item.Quantity.ShouldBe(5);
        await repository.Received(1).UpdateAsync(shoppingList);
    }

    [Fact]
    public async Task Handle_WithInvalidQuantity_PropagatesArgumentException()
    {
        IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>>();
        global::ShoppingList.Domain.Entities.ShoppingList shoppingList = global::ShoppingList.Domain.Entities.ShoppingList.Create("owner-1");
        global::ShoppingList.Domain.Entities.ShoppingListItem item = shoppingList.AddItem("Milk", 1);
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        UpdateItemQuantityHandler handler = new(repository);
        UpdateItemQuantityCommand command = new(shoppingList.Id, item.Id, 0);

        await Should.ThrowAsync<ArgumentException>(async () => await handler.Handle(command));
        await repository.DidNotReceive().UpdateAsync(Arg.Any<global::ShoppingList.Domain.Entities.ShoppingList>());
    }

    [Fact]
    public async Task Handle_WithItemNotFound_PropagatesInvalidOperationException()
    {
        IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>>();
        global::ShoppingList.Domain.Entities.ShoppingList shoppingList = global::ShoppingList.Domain.Entities.ShoppingList.Create("owner-1");
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        UpdateItemQuantityHandler handler = new(repository);

        await Should.ThrowAsync<InvalidOperationException>(async () => await handler.Handle(new UpdateItemQuantityCommand(shoppingList.Id, Guid.NewGuid(), 2)));
        await repository.DidNotReceive().UpdateAsync(Arg.Any<global::ShoppingList.Domain.Entities.ShoppingList>());
    }

    [Fact]
    public async Task Handle_DoesNotAffectFinished()
    {
        IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>>();
        global::ShoppingList.Domain.Entities.ShoppingList shoppingList = global::ShoppingList.Domain.Entities.ShoppingList.Create("owner-1");
        global::ShoppingList.Domain.Entities.ShoppingListItem item = shoppingList.AddItem("Milk", 1);
        shoppingList.MarkItemPurchased(item.Id);
        bool finishedBefore = shoppingList.Finished;
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        UpdateItemQuantityHandler handler = new(repository);
        await handler.Handle(new UpdateItemQuantityCommand(shoppingList.Id, item.Id, 3));

        shoppingList.Finished.ShouldBe(finishedBefore);
    }
}