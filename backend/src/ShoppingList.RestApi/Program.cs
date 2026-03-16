using JasperFx;
using JasperFx.CodeGeneration;
using Microsoft.EntityFrameworkCore;

using ShoppingList.Application.Commands;
using ShoppingList.Application.Handlers;
using ShoppingList.Infrastructure.Db;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
string? connectionString = builder.Configuration.GetConnectionString("database");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'database' is required. Configure ConnectionStrings:database before startup.");
}

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    });
});
builder.Services.AddShoppingListPersistence();

builder.Host.UseWolverine(o =>
{
    o.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;
    o.Discovery.IncludeAssembly(typeof(CreateListHandler).Assembly);
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using IServiceScope scope = app.Services.CreateScope();
    ApplicationContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost("/shopping-lists", async (IMessageBus bus, CreateShoppingListRequest request) =>
{
    Guid id = await bus.InvokeAsync<Guid>(new CreateListCommand(request.Owner, request.Date));
    return Results.Created($"/shopping-lists/{id}", new IdResponse(id));
}).WithName("CreateShoppingList");

app.MapPost("/shopping-lists/{listId}/items", async (IMessageBus bus, Guid listId, AddShoppingListItemRequest request) =>
{
    Guid itemId = await bus.InvokeAsync<Guid>(new AddItemCommand(listId, request.Description, request.Quantity));
    return Results.Created($"/shopping-lists/{listId}/items/{itemId}", new IdResponse(itemId));
}).WithName("AddShoppingListItem");

app.MapPost("/shopping-lists/{listId}/copy", async (IMessageBus bus, Guid listId, CopyShoppingListRequest request) =>
{
    Guid newId = await bus.InvokeAsync<Guid>(new CopyListCommand(listId, request.NewOwner, request.NewDate));
    return Results.Created($"/shopping-lists/{newId}", new IdResponse(newId));
}).WithName("CopyShoppingList");

app.MapPost("/shopping-lists/{listId}/items/{itemId}/purchase", async (IMessageBus bus, Guid listId, Guid itemId) =>
{
    await bus.InvokeAsync(new MarkItemPurchasedCommand(listId, itemId));
    return Results.NoContent();
}).WithName("MarkItemPurchased");

app.MapDelete("/shopping-lists/{listId}/items/{itemId}", async (IMessageBus bus, Guid listId, Guid itemId) =>
{
    await bus.InvokeAsync(new RemoveItemCommand(listId, itemId));
    return Results.NoContent();
}).WithName("RemoveShoppingListItem");

app.MapPut("/shopping-lists/{listId}/items/{itemId}/quantity", async (IMessageBus bus, Guid listId, Guid itemId, UpdateShoppingListItemQuantityRequest request) =>
{
    await bus.InvokeAsync(new UpdateItemQuantityCommand(listId, itemId, request.Quantity));
    return Results.NoContent();
}).WithName("UpdateShoppingListItemQuantity");

if (app.Environment.IsEnvironment("Testing"))
{
    await app.RunAsync();
}
else
{
    await app.RunJasperFxCommands(args);
}

internal sealed record CreateShoppingListRequest(string Owner, DateTime? Date);
internal sealed record AddShoppingListItemRequest(string Description, int? Quantity);
internal sealed record CopyShoppingListRequest(string NewOwner, DateTime? NewDate);
internal sealed record UpdateShoppingListItemQuantityRequest(int? Quantity);
internal sealed record IdResponse(Guid Id);

public partial class Program { }
