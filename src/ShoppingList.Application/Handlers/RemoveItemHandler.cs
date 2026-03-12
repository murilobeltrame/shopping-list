using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class RemoveItemHandler(IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> shoppingListRepository)
{
    private readonly IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> _shoppingListRepository = shoppingListRepository;

    public async Task Handle(RemoveItemCommand command)
    {
        global::ShoppingList.Domain.Entities.ShoppingList? shoppingList = await _shoppingListRepository.GetByIdAsync(command.ListId);
        if (shoppingList is null)
        {
            throw new InvalidOperationException("List not found.");
        }

        shoppingList.RemoveItem(command.ItemId);
        await _shoppingListRepository.UpdateAsync(shoppingList);
    }
}