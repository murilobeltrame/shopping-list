# Backend

This folder contains the ShoppingList backend implementation and its local Spec-Driven Development assets.

## Contents

- `src/`: .NET application source projects
- `test/`: automated test projects
- `env/`: Aspire orchestration and shared service defaults
- `.specify/`: backend-local Speckit templates, scripts, and constitution
- `specs/`: backend feature specifications and plans

## Working Here

Run backend build and test commands from this folder:

```bash
# Build the entire solution
dotnet build ShoppingList.sln

# Run all tests
dotnet test ShoppingList.sln

# Run tests for a specific project
dotnet test test/ShoppingList.Domain.Tests
dotnet test test/ShoppingList.Application.Tests
dotnet test test/ShoppingList.Infrastructure.Db.Tests
dotnet test test/ShoppingList.RestApi.Tests
dotnet test test/ShoppingList.Architecture.Tests

# Run a single test by name
dotnet test ShoppingList.sln --filter "FullyQualifiedName~TestMethodName"

# Generate EF Core migrations for SQL Server
dotnet ef migrations add <MigrationName> \
	--project src/ShoppingList.Infrastructure.Db \
	--startup-project src/ShoppingList.RestApi

# Apply migrations
dotnet ef database update \
	--project src/ShoppingList.Infrastructure.Db \
	--startup-project src/ShoppingList.RestApi

# Run the app via Aspire
dotnet run --project env/ShoppingList.AppHost
```

Speckit commands targeting the backend should use this folder as `SPECIFY_ROOT`.