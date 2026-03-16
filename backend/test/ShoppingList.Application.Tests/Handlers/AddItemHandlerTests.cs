using NSubstitute;
using Shouldly;
using ShoppingList.Application.Commands;
using ShoppingList.Application.Handlers;
using Ardalis.Specification;

namespace ShoppingList.Application.Tests.Handlers;

public class AddItemHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_AddsItemAndReturnsItemId()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Domain.Entities.ShoppingList shoppingList = Domain.Entities.ShoppingList.Create("owner-1");
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);
        AddItemHandler handler = new(repository);
        AddItemCommand command = new(shoppingList.Id, "Milk", 2);

        Guid itemId = await handler.Handle(command);

        itemId.ShouldNotBe(Guid.Empty);
        shoppingList.Items.Count.ShouldBe(1);
        await repository.Received(1).UpdateAsync(shoppingList);
    }

    [Fact]
    public async Task Handle_WithInvalidDescription_PropagatesArgumentException()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Domain.Entities.ShoppingList shoppingList = Domain.Entities.ShoppingList.Create("owner-1");
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);
        AddItemHandler handler = new(repository);
        AddItemCommand command = new(shoppingList.Id, "", 2);

        await Should.ThrowAsync<ArgumentException>(async () => await handler.Handle(command));
        await repository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Entities.ShoppingList>());
    }

    [Fact]
    public async Task Handle_WithListNotFound_ThrowsInvalidOperationException()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Guid listId = Guid.NewGuid();
        repository.GetByIdAsync(listId).Returns((Domain.Entities.ShoppingList?)null);
        AddItemHandler handler = new(repository);
        AddItemCommand command = new(listId, "Milk", 1);

        await Should.ThrowAsync<InvalidOperationException>(async () => await handler.Handle(command));
    }

    [Fact]
    public async Task Handle_UpdatesFinishedStatusCorrectly()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Domain.Entities.ShoppingList shoppingList = Domain.Entities.ShoppingList.Create("owner-1");
        Domain.Entities.ShoppingListItem initialItem = shoppingList.AddItem("Bread");
        shoppingList.MarkItemPurchased(initialItem.Id);
        shoppingList.Finished.ShouldBeTrue();

        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);
        AddItemHandler handler = new(repository);
        AddItemCommand command = new(shoppingList.Id, "Milk", 1);

        _ = await handler.Handle(command);

        shoppingList.Finished.ShouldBeFalse();
    }
}