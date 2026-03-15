# Copilot Instructions

## Build & Test Commands

```bash
# Build entire solution
dotnet build

# Run all tests
dotnet test

# Run tests for a specific project
dotnet test test/ShoppingList.Domain.Tests
dotnet test test/ShoppingList.Application.Tests
dotnet test test/ShoppingList.Infrastructure.Db.Tests
dotnet test test/ShoppingList.RestApi.Tests
dotnet test test/ShoppingList.Architecture.Tests

# Run a single test by name
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Run the app via Aspire (orchestrates all services)
dotnet run --project env/ShoppingList.AppHost
```

## Architecture

This is a **Clean Architecture** solution targeting **.NET 10**. The dependency flow is strictly inward:

```
RestApi → Application → Domain
          Infrastructure.Db → Domain
```

- **`src/ShoppingList.Domain`** — Core business entities, value objects, and domain events. Contains business logic and invariants. No external dependencies.
- **`src/ShoppingList.Application`** — Use case orchestration (CQRS commands/queries and handlers), repository access through Ardalis.Specification abstractions, optional boundary pre-validation via FluentValidation, and dispatch via WolverineFx. Depends only on Domain.
- **`src/ShoppingList.Infrastructure.Db`** — EF Core DbContext, repositories, and migrations. Depends only on Domain.
- **`src/ShoppingList.RestApi`** — ASP.NET Core Minimal API with OpenAPI. Depends on Application and Infrastructure.Db.
- **`env/ShoppingList.AppHost`** — .NET Aspire orchestration host for local development. Runs the API as `"api-rest"`.
- **`env/ShoppingList.ServiceDefaults`** — Shared Aspire cross-cutting concerns: OpenTelemetry, health checks (`/health`, `/alive`), service discovery, and HTTP resilience.

Layer boundary correctness is enforced by **`test/ShoppingList.Architecture.Tests`**.

## Key Conventions

### Naming (enforced via `.editorconfig`)
- Private fields: `_camelCase`
- Private static fields: `s_camelCase`
- Interfaces: `IPascalCase`
- Type parameters: `TPascalCase`
- Everything else: `PascalCase` (types, methods, properties, events) or `camelCase` (locals, parameters)

### C# Style
- **No `var`** — explicit types everywhere (`csharp_style_var_*` all set to `false`)
- **File-scoped namespaces** — `namespace Foo.Bar;` not `namespace Foo.Bar { }`
- **Primary constructors** preferred over explicit constructor bodies
- **Expression-bodied** accessors and properties; not methods or constructors
- `using` directives go **outside** the namespace, system directives sorted first
- Readonly fields are enforced as warnings

### Domain Modeling Rules
- Domain entities must not expose a public default constructor
- Use factory methods and instance methods to enforce invariants
- Keep business logic in Domain; Application handles coordination and boundary pre-validation

### Project Structure
- Source projects live under `src/`
- Test projects live under `test/`, one per source layer plus architecture tests
- Aspire infrastructure lives under `env/`
- Namespaces must match folder structure (`dotnet_style_namespace_match_folder`)
