using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class CopyListHandler(IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> shoppingListRepository)
{
    private readonly IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList> _shoppingListRepository = shoppingListRepository;

    public async Task<Guid> Handle(CopyListCommand command)
    {
        global::ShoppingList.Domain.Entities.ShoppingList? sourceList = await _shoppingListRepository.GetByIdAsync(command.SourceListId);
        if (sourceList is null)
        {
            throw new InvalidOperationException("List not found.");
        }

        global::ShoppingList.Domain.Entities.ShoppingList copiedList = sourceList.Copy(command.NewOwner, command.NewDate);
        _ = await _shoppingListRepository.AddAsync(copiedList);
        return copiedList.Id;
    }
}