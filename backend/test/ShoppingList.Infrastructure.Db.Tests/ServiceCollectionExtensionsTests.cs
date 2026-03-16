using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using ShoppingList.Infrastructure.Db.Repositories;

namespace ShoppingList.Infrastructure.Db.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddShoppingListPersistence_RegistersOpenGenericRepositories()
    {
        ServiceCollection services = new();
        services.AddDbContext<ApplicationContext>(options =>
            options.UseInMemoryDatabase("repository-wiring"));

        services.AddShoppingListPersistence();

        ServiceProvider provider = services.BuildServiceProvider();
        IRepositoryBase<Domain.Entities.ShoppingList> repository =
            provider.GetRequiredService<IRepositoryBase<Domain.Entities.ShoppingList>>();
        IReadRepositoryBase<Domain.Entities.ShoppingList> readRepository =
            provider.GetRequiredService<IReadRepositoryBase<Domain.Entities.ShoppingList>>();

        repository.ShouldBeOfType<EfRepository<Domain.Entities.ShoppingList>>();
        readRepository.ShouldBeOfType<EfRepository<Domain.Entities.ShoppingList>>();
    }
}
