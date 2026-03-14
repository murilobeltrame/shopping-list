using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class CreateListHandler(IRepositoryBase<Domain.Entities.ShoppingList> shoppingListRepository)
{
    public async Task<Guid> Handle(CreateListCommand command)
    {
        Domain.Entities.ShoppingList shoppingList = Domain.Entities.ShoppingList.Create(command.Owner, command.Date);
        _ = await shoppingListRepository.AddAsync(shoppingList);
        return shoppingList.Id;
    }
}