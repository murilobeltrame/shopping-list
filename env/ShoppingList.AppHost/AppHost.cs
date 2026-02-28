using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<ShoppingList_RestApi>("api-rest");

await builder.Build().RunAsync();
