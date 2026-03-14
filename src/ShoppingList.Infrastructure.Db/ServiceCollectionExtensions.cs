using Ardalis.Specification;

using Microsoft.Extensions.DependencyInjection;

using ShoppingList.Infrastructure.Db.Repositories;

namespace ShoppingList.Infrastructure.Db;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShoppingListPersistence(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepositoryBase<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepositoryBase<>), typeof(EfRepository<>));
        return services;
    }
}