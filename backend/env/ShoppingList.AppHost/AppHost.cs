using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddAzurePostgresFlexibleServer("db-server")
    .RunAsContainer(c => c.WithPgWeb())
    .AddDatabase("database");

builder.AddProject<ShoppingList_RestApi>("api-rest")
    .WaitFor(db)
    .WithReference(db);

await builder.Build().RunAsync();
