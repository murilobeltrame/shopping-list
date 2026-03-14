using Ardalis.Specification;

using Microsoft.Extensions.DependencyInjection;

using ShoppingList.Infrastructure.Db.Repositories;

namespace ShoppingList.Infrastructure.Db;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShoppingListPersistence(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>, ShoppingListRepository>();
        services.AddScoped<IReadRepositoryBase<global::ShoppingList.Domain.Entities.ShoppingList>, ShoppingListRepository>();
        return services;
    }
}