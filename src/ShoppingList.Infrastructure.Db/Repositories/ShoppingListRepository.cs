using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

namespace ShoppingList.Infrastructure.Db.Repositories;

public sealed class ShoppingListRepository(ApplicationContext dbContext)
    : RepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>(dbContext),
        IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>,
        IReadRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>
{
}