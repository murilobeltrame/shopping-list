using JasperFx;
using JasperFx.CodeGeneration;

using ShoppingList.Infrastructure.Db;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureNpgsqlDbContext<ApplicationContext>("database");

builder.Host.UseWolverine(o => o.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();

await app.RunJasperFxCommands(args);
