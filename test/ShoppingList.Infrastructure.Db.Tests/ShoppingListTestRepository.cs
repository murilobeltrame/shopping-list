using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using ShoppingList.Infrastructure.Db;

namespace ShoppingList.Infrastructure.Db.Tests;

public sealed class ShoppingListTestRepository(ApplicationContext dbContext)
    : RepositoryBase<Domain.Entities.ShoppingList>(dbContext),
        IRepositoryBase<Domain.Entities.ShoppingList>,
        IReadRepositoryBase<Domain.Entities.ShoppingList>
{
}
