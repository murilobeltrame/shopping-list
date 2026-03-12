using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class AddItemHandler(IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> shoppingListRepository)
{
    private readonly IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> _shoppingListRepository = shoppingListRepository;

    public async Task<Guid> Handle(AddItemCommand command)
    {
        global::ShoppingList.Domain.Entities.ShoppingList? shoppingList = await _shoppingListRepository.GetByIdAsync(command.ListId);
        if (shoppingList is null)
        {
            throw new InvalidOperationException("List not found.");
        }

        global::ShoppingList.Domain.Entities.ShoppingListItem item = shoppingList.AddItem(command.Description, command.Quantity);
        await _shoppingListRepository.UpdateAsync(shoppingList);
        return item.Id;
    }
}