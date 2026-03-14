using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class MarkItemPurchasedHandler(IRepositoryBase<Domain.Entities.ShoppingList> shoppingListRepository)
{
    public async Task Handle(MarkItemPurchasedCommand command)
    {
        Domain.Entities.ShoppingList? shoppingList = await shoppingListRepository.GetByIdAsync(command.ListId) ?? throw new InvalidOperationException("List not found.");
        shoppingList.MarkItemPurchased(command.ItemId);
        await shoppingListRepository.UpdateAsync(shoppingList);
    }
}