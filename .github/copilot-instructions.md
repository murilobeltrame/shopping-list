# Copilot Instructions

## Repository Scope

This repository is organized into five top-level product areas:

- `backend/`
- `ios/`
- `android/`
- `web/`
- `iac/`

Keep root-level work limited to cross-cutting repository concerns, shared guidance, and workflow configuration.

## Speckit Routing

Each product area owns its own `.specify/` and `specs/` directories.

- Always resolve the target part first: `backend`, `ios`, `android`, `web`, or `iac`
- When invoking any Speckit shell script, set `SPECIFY_ROOT` to the absolute path of that part
- Use the target part paths for `.specify`, `specs`, and local `.github` context

Examples:

```bash
SPECIFY_ROOT="$PWD/backend" "$SPECIFY_ROOT/.specify/scripts/bash/check-prerequisites.sh" --json
SPECIFY_ROOT="$PWD/ios" "$SPECIFY_ROOT/.specify/scripts/bash/create-new-feature.sh" --json "Add onboarding flow"
```

## Backend Commands

The existing implementation is currently in `backend/`.

```bash
# Build entire backend solution
dotnet build backend/ShoppingList.sln

# Run all backend tests
dotnet test backend/ShoppingList.sln

# Run tests for a specific backend project
dotnet test backend/test/ShoppingList.Domain.Tests
dotnet test backend/test/ShoppingList.Application.Tests
dotnet test backend/test/ShoppingList.Infrastructure.Db.Tests
dotnet test backend/test/ShoppingList.RestApi.Tests
dotnet test backend/test/ShoppingList.Architecture.Tests

# Run a single backend test by name
dotnet test backend/ShoppingList.sln --filter "FullyQualifiedName~TestMethodName"

# Run the backend app via Aspire
dotnet run --project backend/env/ShoppingList.AppHost
```

## Backend Architecture

The backend is a **Clean Architecture** solution targeting **.NET 10**. The dependency flow is strictly inward:

```text
RestApi → Application → Domain
          Infrastructure.Db → Domain
```

- `backend/src/ShoppingList.Domain` — Core business entities, value objects, and domain events. Contains business logic and invariants. No external dependencies.
- `backend/src/ShoppingList.Application` — Use case orchestration (CQRS commands/queries and handlers), repository access through Ardalis.Specification abstractions, optional boundary pre-validation via FluentValidation, and dispatch via WolverineFx. Depends only on Domain.
- `backend/src/ShoppingList.Infrastructure.Db` — EF Core DbContext, repositories, and migrations. Depends only on Domain.
- `backend/src/ShoppingList.RestApi` — ASP.NET Core Minimal API with OpenAPI. Depends on Application and Infrastructure.Db.
- `backend/env/ShoppingList.AppHost` — .NET Aspire orchestration host for local development. Runs the API as `api-rest`.
- `backend/env/ShoppingList.ServiceDefaults` — Shared Aspire cross-cutting concerns: OpenTelemetry, health checks (`/health`, `/alive`), service discovery, and HTTP resilience.

Layer boundary correctness is enforced by `backend/test/ShoppingList.Architecture.Tests`.

## Backend Conventions

### Naming
- Private fields: `_camelCase`
- Private static fields: `s_camelCase`
- Interfaces: `IPascalCase`
- Type parameters: `TPascalCase`

### C# Style
- No `var`; use explicit types
- Use file-scoped namespaces
- Prefer primary constructors where applicable
- Prefer expression-bodied accessors and properties
- Keep `using` directives outside the namespace

### Domain Rules
- Domain entities must not expose a public default constructor
- Use factory methods and instance methods to enforce invariants
- Keep business logic in Domain; Application handles coordination and boundary pre-validation
