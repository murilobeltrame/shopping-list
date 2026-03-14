using NSubstitute;
using Shouldly;
using ShoppingList.Application.Commands;
using ShoppingList.Application.Handlers;
using Ardalis.Specification;

namespace ShoppingList.Application.Tests.Handlers;

public class CreateListHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesListAndReturnsId()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        CreateListHandler handler = new(repository);
        CreateListCommand command = new("owner-1", null);

        Guid result = await handler.Handle(command);

        result.ShouldNotBe(Guid.Empty);
        await repository.Received(1).AddAsync(Arg.Is<Domain.Entities.ShoppingList>(l => l.Id == result && l.Owner == "owner-1"));
    }

    [Fact]
    public async Task Handle_WithInvalidOwner_PropagatesArgumentException()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        CreateListHandler handler = new(repository);
        CreateListCommand command = new("   ", null);

        await Should.ThrowAsync<ArgumentException>(async () => await handler.Handle(command));
        await repository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.ShoppingList>());
    }

    [Fact]
    public async Task Handle_WithDate_CreatesListWithDate()
    {
        IRepositoryBase<Domain.Entities.ShoppingList> repository = Substitute.For<IRepositoryBase<Domain.Entities.ShoppingList>>();
        CreateListHandler handler = new(repository);
        DateTime date = new(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        CreateListCommand command = new("owner-1", date);

        Guid result = await handler.Handle(command);

        result.ShouldNotBe(Guid.Empty);
        await repository.Received(1).AddAsync(Arg.Is<Domain.Entities.ShoppingList>(l => l.Date == date));
    }
}