using NSubstitute;
using Shouldly;
using ShoppingList.Application.Commands;
using ShoppingList.Application.Handlers;
using Ardalis.Specification;

namespace ShoppingList.Application.Tests.Handlers;

public class RemoveItemHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_MarksRemoved()
    {
        IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>>();
        global::ShoppingList.Domain.Entities.ShoppingList shoppingList = global::ShoppingList.Domain.Entities.ShoppingList.Create("owner-1");
        global::ShoppingList.Domain.Entities.ShoppingListItem item = shoppingList.AddItem("Milk");
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        RemoveItemHandler handler = new(repository);
        RemoveItemCommand command = new(shoppingList.Id, item.Id);

        await handler.Handle(command);

        item.Removed.ShouldBeTrue();
        await repository.Received(1).UpdateAsync(shoppingList);
    }

    [Fact]
    public async Task Handle_WithItemNotFound_PropagatesInvalidOperationException()
    {
        IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>>();
        global::ShoppingList.Domain.Entities.ShoppingList shoppingList = global::ShoppingList.Domain.Entities.ShoppingList.Create("owner-1");
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        RemoveItemHandler handler = new(repository);
        RemoveItemCommand command = new(shoppingList.Id, Guid.NewGuid());

        await Should.ThrowAsync<InvalidOperationException>(async () => await handler.Handle(command));
        await repository.DidNotReceive().UpdateAsync(Arg.Any<global::ShoppingList.Domain.Entities.ShoppingList>());
    }

    [Fact]
    public async Task Handle_RecalculatesFinished()
    {
        IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>>();
        global::ShoppingList.Domain.Entities.ShoppingList shoppingList = global::ShoppingList.Domain.Entities.ShoppingList.Create("owner-1");
        global::ShoppingList.Domain.Entities.ShoppingListItem item = shoppingList.AddItem("Milk");
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        RemoveItemHandler handler = new(repository);
        RemoveItemCommand command = new(shoppingList.Id, item.Id);

        await handler.Handle(command);

        shoppingList.Finished.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithListNotFound_ThrowsInvalidOperationException()
    {
        IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>>();
        Guid listId = Guid.NewGuid();
        repository.GetByIdAsync(listId).Returns((global::ShoppingList.Domain.Entities.ShoppingList?)null);
        RemoveItemHandler handler = new(repository);

        await Should.ThrowAsync<InvalidOperationException>(async () => await handler.Handle(new RemoveItemCommand(listId, Guid.NewGuid())));
    }
}