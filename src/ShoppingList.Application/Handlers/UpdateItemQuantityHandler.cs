using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class UpdateItemQuantityHandler(IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> shoppingListRepository)
{
    private readonly IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> _shoppingListRepository = shoppingListRepository;

    public async Task Handle(UpdateItemQuantityCommand command)
    {
        global::ShoppingList.Domain.Entities.ShoppingList? shoppingList = await _shoppingListRepository.GetByIdAsync(command.ListId);
        if (shoppingList is null)
        {
            throw new InvalidOperationException("List not found.");
        }

        shoppingList.UpdateItemQuantity(command.ItemId, command.NewQuantity);
        await _shoppingListRepository.UpdateAsync(shoppingList);
    }
}