using NSubstitute;
using Shouldly;
using ShoppingList.Application.Commands;
using ShoppingList.Application.Handlers;
using Ardalis.Specification;

namespace ShoppingList.Application.Tests.Handlers;

public class MarkItemPurchasedHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_MarksPurchased()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Domain.Entities.ShoppingList shoppingList = Domain.Entities.ShoppingList.Create("owner-1");
        Domain.Entities.ShoppingListItem item = shoppingList.AddItem("Milk");
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        MarkItemPurchasedHandler handler = new(repository);
        MarkItemPurchasedCommand command = new(shoppingList.Id, item.Id);

        await handler.Handle(command);

        item.Purchased.ShouldBeTrue();
        await repository.Received(1).UpdateAsync(shoppingList);
    }

    [Fact]
    public async Task Handle_WithItemNotFound_PropagatesInvalidOperationException()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Domain.Entities.ShoppingList shoppingList = Domain.Entities.ShoppingList.Create("owner-1");
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        MarkItemPurchasedHandler handler = new(repository);
        MarkItemPurchasedCommand command = new(shoppingList.Id, Guid.NewGuid());

        await Should.ThrowAsync<InvalidOperationException>(async () => await handler.Handle(command));
        await repository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Entities.ShoppingList>());
    }

    [Fact]
    public async Task Handle_RecalculatesFinished()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Domain.Entities.ShoppingList shoppingList = Domain.Entities.ShoppingList.Create("owner-1");
        Domain.Entities.ShoppingListItem item = shoppingList.AddItem("Milk");
        repository.GetByIdAsync(shoppingList.Id).Returns(shoppingList);

        MarkItemPurchasedHandler handler = new(repository);
        MarkItemPurchasedCommand command = new(shoppingList.Id, item.Id);

        await handler.Handle(command);

        shoppingList.Finished.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WithListNotFound_ThrowsInvalidOperationException()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        Guid listId = Guid.NewGuid();
        repository.GetByIdAsync(listId).Returns((Domain.Entities.ShoppingList?)null);
        MarkItemPurchasedHandler handler = new(repository);

        await Should.ThrowAsync<InvalidOperationException>(async () => await handler.Handle(new MarkItemPurchasedCommand(listId, Guid.NewGuid())));
    }
}