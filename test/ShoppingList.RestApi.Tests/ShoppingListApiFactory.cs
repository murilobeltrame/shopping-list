using Ardalis.Specification;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace ShoppingList.RestApi.Tests;

public sealed class ShoppingListApiFactory : WebApplicationFactory<Program>
{
    internal IRepositoryBase<ShoppingList.Domain.Entities.ShoppingList> Repository { get; } =
        Substitute.For<IRepositoryBase<ShoppingList.Domain.Entities.ShoppingList>>();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureWebHost(webBuilder => webBuilder.UseTestServer());
        IHost host = builder.Build();
        host.Start();
        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:database",
            "Host=localhost;Port=5432;Database=test;Username=test;Password=test");

        builder.ConfigureServices(services =>
        {
            RemoveRegistrations(services, typeof(IRepositoryBase<ShoppingList.Domain.Entities.ShoppingList>));
            RemoveRegistrations(services, typeof(IReadRepositoryBase<ShoppingList.Domain.Entities.ShoppingList>));

            services.AddScoped<IRepositoryBase<ShoppingList.Domain.Entities.ShoppingList>>(_ => Repository);
            services.AddScoped<IReadRepositoryBase<ShoppingList.Domain.Entities.ShoppingList>>(
                _ => (IReadRepositoryBase<ShoppingList.Domain.Entities.ShoppingList>)Repository);
        });
    }

    private static void RemoveRegistrations(IServiceCollection services, Type serviceType)
    {
        List<ServiceDescriptor> descriptors = services
            .Where(d => d.ServiceType == serviceType)
            .ToList();

        foreach (ServiceDescriptor descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }
}
