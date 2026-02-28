using ShoppingList.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureNpgsqlDbContext<ApplicationContext>("database");

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();

await app.RunAsync();
