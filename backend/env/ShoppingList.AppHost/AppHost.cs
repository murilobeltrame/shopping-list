using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddAzureSqlServer("db-server")
    .RunAsContainer()
    .AddDatabase("database");

builder.AddProject<ShoppingList_RestApi>("api-rest")
    .WaitFor(db)
    .WithReference(db);

await builder.Build().RunAsync();
