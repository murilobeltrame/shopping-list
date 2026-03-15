using Ardalis.Specification;
using ShoppingList.Application.Commands;

namespace ShoppingList.Application.Handlers;

public sealed class CopyListHandler(IRepositoryBase<Domain.Entities.ShoppingList> shoppingListRepository)
{
    public async Task<Guid> Handle(CopyListCommand command)
    {
        Domain.Entities.ShoppingList? sourceList = await shoppingListRepository.GetByIdAsync(command.SourceListId) ?? throw new InvalidOperationException("List not found.");
        Domain.Entities.ShoppingList copiedList = sourceList.Copy(command.NewOwner, command.NewDate);
        _ = await shoppingListRepository.AddAsync(copiedList);
        return copiedList.Id;
    }
}