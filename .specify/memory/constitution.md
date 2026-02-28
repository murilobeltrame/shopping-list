<!--
Sync Impact Report - Version 1.0.0 (2026-02-28)
================================================================
VERSION CHANGE: [INITIAL] → 1.0.0
BUMP RATIONALE: Initial constitution creation for ShoppingList project

MODIFIED PRINCIPLES: N/A (initial creation)
ADDED SECTIONS:
  - I. Clean Architecture & Dependency Inversion
  - II. Test-First Development (NON-NEGOTIABLE)
  - III. Specification Pattern for Data Access
  - IV. Real Database Testing
  - V. Modern C# Idioms
  - VI. CQRS Pattern
  - VII. Explicit Types Everywhere
  - Technology Stack & Dependencies
  - Code Quality & Architecture Enforcement

REMOVED SECTIONS: N/A (initial creation)

TEMPLATES REQUIRING UPDATES:
  ✅ plan-template.md - Aligned Constitution Check section with 7 principles
  ✅ spec-template.md - Aligned requirements structure with Clean Architecture
  ✅ tasks-template.md - Aligned task categorization with TDD and layer structure

FOLLOW-UP TODOs: None - all placeholders resolved
================================================================
-->

# ShoppingList Constitution

## Core Principles

### I. Clean Architecture & Dependency Inversion

**Dependencies MUST flow inward**: RestApi → Application → Domain; Infrastructure.Db → Domain only.

- Domain layer has ZERO external dependencies (pure business logic)
- Application layer depends ONLY on Domain (use cases, CQRS handlers)
- Infrastructure.Db depends ONLY on Domain (EF Core, repositories, migrations)
- RestApi depends on Application and Infrastructure.Db (composition root)
- All layers use file-scoped namespaces matching folder structure
- Layer boundaries are enforced by `ShoppingList.Architecture.Tests`

**Rationale**: Dependency inversion ensures testability, maintainability, and business logic independence from infrastructure concerns.

### II. Test-First Development (NON-NEGOTIABLE)

**TDD is MANDATORY for all features**. The Red-Green-Refactor cycle is strictly enforced:

1. Write test using Shouldly assertions
2. Verify test FAILS (Red)
3. Implement minimal code to pass (Green)
4. Refactor while keeping tests green (Refactor)

**Test structure requirements**:

- One test project per source layer (Domain.Tests, Application.Tests, Infrastructure.Db.Tests, RestApi.Tests, Architecture.Tests)
- Use Shouldly library for all assertions (e.g., `result.ShouldBe(expected)`)
- Use AutoBogus library to generate fake test data
- Test naming: `[MethodName]_[Scenario]_[ExpectedResult]`
- Arrange-Act-Assert pattern strictly enforced

**Rationale**: Test-first ensures features are testable by design, catches regressions early, and serves as living documentation.

### III. Specification Pattern for Data Access

**ALL Entity Framework Core queries MUST use Ardalis.Specification library**. Direct DbSet queries are prohibited except in specifications.

- Create specification classes inheriting from `Specification<TEntity>`
- Encapsulate query logic, filtering, includes, and ordering in specifications
- Repository pattern uses `IRepository<T>` from Ardalis.Specification
- Keep specifications in Application layer, implementations in Infrastructure.Db
- Specifications MUST be unit testable without database

**Example**:

```csharp
public sealed class ActiveItemsWithCategorySpec : Specification<ShoppingItem>
{
    public ActiveItemsWithCategorySpec() =>
        Query
            .Where(item => item.IsActive)
            .Include(item => item.Category)
            .OrderBy(item => item.Name);
}
```

**Rationale**: Specification pattern provides reusable, testable query logic while maintaining separation from infrastructure details.

### IV. Real Database Testing

**Database tests MUST run against real database instances using TestContainers**. In-memory databases are prohibited for integration tests.

- Use TestContainers to spin up PostgreSQL/SQL Server containers for tests
- Infrastructure.Db.Tests project contains integration tests with real DbContext
- Each test class disposes container after test execution
- Migrations MUST be tested against real database schema
- Test data setup uses real EF Core operations, not manual SQL

**Rationale**: In-memory databases have behavioral differences from production databases. TestContainers ensure tests catch real-world database issues.

### V. Modern C# Idioms

**Prefer modern C# language features** for conciseness and immutability:

- **Primary constructors** MUST be used for dependency injection and simple initialization
- **Expression-bodied members** (=>) MUST be used for properties and accessors
- Methods and full constructors use block syntax
- File-scoped namespaces (enforced via `.editorconfig`)
- Record types for DTOs and value objects where immutability is desired

**Example**:

```csharp
public sealed class ShoppingListService(IRepository<ShoppingList> repository)
{
    private readonly IRepository<ShoppingList> _repository = repository;
    
    public int Count => _repository.CountAsync().Result;
}
```

**Rationale**: Primary constructors reduce boilerplate; expression-bodied members improve readability for simple members.

### VI. CQRS Pattern

**Command-Query Responsibility Segregation MUST be applied** in the Application layer:

- Commands modify state, return void or simple results
- Queries return data, NEVER modify state
- Handlers are separate classes: `CommandHandler`, `QueryHandler`
- Commands and Queries are immutable record types
- MediatR or similar pattern for handler dispatching

**Structure**:

```
Application/
├── Commands/
│   ├── CreateShoppingList/
│   │   ├── CreateShoppingListCommand.cs
│   │   └── CreateShoppingListCommandHandler.cs
├── Queries/
│   ├── GetShoppingList/
│   │   ├── GetShoppingListQuery.cs
│   │   └── GetShoppingListQueryHandler.cs
```

**Rationale**: CQRS provides clear separation of concerns, simplifies testing, and enables independent scaling of read/write operations.

### VII. Explicit Types Everywhere

**The `var` keyword is PROHIBITED** throughout the codebase. All variable declarations MUST use explicit types.

- Local variables: `List<string> items = new();` not `var items = new List<string>();`
- Return types always explicit
- Enforced via `.editorconfig`: `csharp_style_var_*` all set to `false`
- Exception: `foreach` with complex generic types may use `var` when type is obvious from context

**Rationale**: Explicit types improve code readability, especially for developers unfamiliar with the codebase, and reduce cognitive load during code review.

## Technology Stack & Dependencies

**Framework & Runtime**:

- .NET 10 (SDK 10.0.100) as specified in `global.json`
- C# 13 language features
- ASP.NET Core Minimal APIs for REST endpoints
- .NET Aspire for orchestration (local development)

**Mandatory Libraries**:

- **Data Access**: Entity Framework Core, Ardalis.Specification
- **Testing**: xUnit, Shouldly, AutoBogus, TestContainers
- **Observability**: OpenTelemetry (via ServiceDefaults)
- **API**: ASP.NET Core OpenAPI/Swagger

**Project Structure Enforcement**:

- Source: `src/` (Domain, Application, Infrastructure.Db, RestApi)
- Tests: `test/` (one test project per source project + Architecture.Tests)
- Environment: `env/` (AppHost, ServiceDefaults)
- Aspire runs API as `"api-rest"` service

## Code Quality & Architecture Enforcement

**Automated Enforcement**:

- `.editorconfig` enforces naming conventions and code style
- `ShoppingList.Architecture.Tests` validates layer dependencies using ArchUnitNET or NetArchTest
- All projects MUST build without warnings (`dotnet build`)
- All tests MUST pass before merge (`dotnet test`)

**Naming Conventions** (`.editorconfig` enforced):

- Private fields: `_camelCase`
- Private static fields: `s_camelCase`
- Interfaces: `IPascalCase`
- Type parameters: `TPascalCase`
- Types, methods, properties: `PascalCase`
- Parameters, locals: `camelCase`

**Health & Observability**:

- `/health` endpoint for liveness checks
- `/alive` endpoint for readiness checks
- Structured logging via OpenTelemetry
- Distributed tracing for request flows

## Governance

**Authority**: This constitution supersedes all other development practices and decisions.

**Amendment Process**:

1. Proposed changes documented with rationale
2. Impact analysis on existing code and templates
3. Version bump per semantic versioning (MAJOR.MINOR.PATCH)
4. Sync impact report prepended to constitution
5. Update dependent templates and documentation

**Compliance**:

- All feature specs MUST include Constitution Check section
- Architecture tests MUST pass before any PR merge
- Code reviews MUST verify adherence to all principles
- Complexity violations MUST be justified in `plan.md`

**Development Guidance**: For active development, reference `.github/copilot-instructions.md` for build commands, architecture overview, and conventions.

**Version**: 1.0.0 | **Ratified**: 2026-02-28 | **Last Amended**: 2026-02-28
