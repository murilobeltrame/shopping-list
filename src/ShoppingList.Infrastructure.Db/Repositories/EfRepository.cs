using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

namespace ShoppingList.Infrastructure.Db.Repositories;

public sealed class EfRepository<T>(ApplicationContext dbContext)
    : RepositoryBase<T>(dbContext), IRepositoryBase<T>, IReadRepositoryBase<T>
    where T : class
{
}
