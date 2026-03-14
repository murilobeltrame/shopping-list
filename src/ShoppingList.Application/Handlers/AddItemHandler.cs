using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class AddItemHandler(IRepositoryBase<Domain.Entities.ShoppingList> shoppingListRepository)
{
    public async Task<Guid> Handle(AddItemCommand command)
    {
        Domain.Entities.ShoppingList? shoppingList = await shoppingListRepository.GetByIdAsync(command.ListId) ?? throw new InvalidOperationException("List not found.");
        Domain.Entities.ShoppingListItem item = shoppingList.AddItem(command.Description, command.Quantity);
        await shoppingListRepository.UpdateAsync(shoppingList);
        return item.Id;
    }
}