using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class UpdateItemQuantityHandler(IRepositoryBase<Domain.Entities.ShoppingList> shoppingListRepository)
{
    public async Task Handle(UpdateItemQuantityCommand command)
    {
        Domain.Entities.ShoppingList? shoppingList = await shoppingListRepository.GetByIdAsync(command.ListId) ?? throw new InvalidOperationException("List not found.");
        shoppingList.UpdateItemQuantity(command.ItemId, command.NewQuantity);
        await shoppingListRepository.UpdateAsync(shoppingList);
    }
}