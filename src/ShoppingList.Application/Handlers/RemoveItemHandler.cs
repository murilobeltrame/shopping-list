using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class RemoveItemHandler(IRepositoryBase<Domain.Entities.ShoppingList> shoppingListRepository)
{
    public async Task Handle(RemoveItemCommand command)
    {
        Domain.Entities.ShoppingList? shoppingList = await shoppingListRepository.GetByIdAsync(command.ListId) ?? throw new InvalidOperationException("List not found.");
        shoppingList.RemoveItem(command.ItemId);
        await shoppingListRepository.UpdateAsync(shoppingList);
    }
}