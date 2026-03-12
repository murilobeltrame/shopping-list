using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class CreateListHandler(IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> shoppingListRepository)
{
    private readonly IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> _shoppingListRepository = shoppingListRepository;

    public async Task<Guid> Handle(CreateListCommand command)
    {
        global::ShoppingList.Domain.Entities.ShoppingList shoppingList = global::ShoppingList.Domain.Entities.ShoppingList.Create(command.Owner, command.Date);
        _ = await _shoppingListRepository.AddAsync(shoppingList);
        return shoppingList.Id;
    }
}