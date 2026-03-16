# Implementation Plan: Migrate Backend Database to Azure SQL

**Branch**: `001-azure-sql` | **Date**: 2026-03-15 | **Spec**: `/Users/murilobeltrame/Development/std/dotnet/llm-pairing-experimentation/backend/specs/001-azure-sql/spec.md`
**Input**: Feature specification from `/Users/murilobeltrame/Development/std/dotnet/llm-pairing-experimentation/backend/specs/001-azure-sql/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Migrate backend persistence from PostgreSQL/Npgsql to Azure SQL (SQL Server provider) while preserving all existing domain behavior and API contracts. The technical approach is to switch EF Core provider and Aspire orchestration resources, regenerate SQL Server-specific EF migrations, maintain CQRS/application/domain boundaries unchanged, and validate parity through existing unit/integration/API tests plus real SQL Server container-backed integration tests.

## Technical Context

**Language/Version**: .NET 10 (SDK 10.0.100), C# 13  
**Primary Dependencies**: ASP.NET Core, Entity Framework Core, Ardalis.Specification, Ardalis.Specification.EntityFrameworkCore, WolverineFx, FluentValidation (when boundary validation is needed)  
**Storage**: Azure SQL (production and cloud deployments), SQL Server container for local development and CI integration tests  
**Testing**: xUnit, Shouldly (assertions), AutoBogus (fake data), TestContainers (real DB tests)  
**Target Platform**: ASP.NET Core REST API, .NET Aspire orchestration for local development  
**Project Type**: Clean Architecture REST API with CQRS pattern  
**Performance Goals**: Match or improve existing PostgreSQL baseline for p95 endpoint latency under equivalent load; no endpoint regression larger than 10% p95 delta  
**Constraints**: Preserve API contract compatibility, keep dependency flow inward, regenerate provider-specific migrations (no mixed PostgreSQL + SQL Server migration chain), startup must fail fast on invalid DB configuration  
**Scale/Scope**: Single backend service and current ShoppingList bounded context; migration includes infrastructure, app host, tests, and configuration paths used by local and cloud deployments

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Verify compliance with ShoppingList Constitution (`.specify/memory/constitution.md`):

- ✅ **I. Domain-Centric Clean Architecture**: PASS. Migration scope is infrastructure/orchestration/persistence provider only; domain and application business rules remain in existing layers.
- ✅ **II. Test-First**: PASS. Plan includes DB-provider migration tests and regression verification before final refactor completion.
- ✅ **III. Specification Pattern**: PASS. Repository abstractions and open-generic `EfRepository<T>` remain unchanged; provider swap does not introduce direct `DbSet<T>` access.
- ✅ **IV. Real Database Testing**: PASS. Integration tests remain container-based, now using SQL Server container images instead of PostgreSQL.
- ✅ **V. Domain Entity Integrity**: PASS. No entity construction or invariants are modified by this feature.
- ✅ **VI. CQRS Messaging & Validation**: PASS. Wolverine-based dispatch remains unchanged; no new boundary validation paths are introduced.
- ✅ **VII. Modern C# Style Rules**: PASS. Planned implementation keeps existing style constraints (`var` prohibition, constructor usage, file-scoped namespaces).

If any violations are needed, document justification in Complexity Tracking section below.

## Project Structure

### Documentation (this feature)

```text
specs/001-azure-sql/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
src/
├── ShoppingList.Domain/          # Core entities, value objects, domain events (no dependencies)
├── ShoppingList.Application/     # CQRS commands/queries, handlers (depends on Domain only)
├── ShoppingList.Infrastructure.Db/  # EF Core, repositories, migrations (depends on Domain only)
└── ShoppingList.RestApi/         # ASP.NET Core Minimal API (depends on Application + Infrastructure.Db)

test/
├── ShoppingList.Domain.Tests/           # Domain layer unit tests (Shouldly, AutoBogus)
├── ShoppingList.Application.Tests/     # Application layer unit tests (Shouldly, AutoBogus)
├── ShoppingList.Infrastructure.Db.Tests/  # Integration tests with TestContainers
├── ShoppingList.RestApi.Tests/         # API endpoint tests
└── ShoppingList.Architecture.Tests/    # Layer dependency enforcement tests

env/
├── ShoppingList.AppHost/         # .NET Aspire orchestration
└── ShoppingList.ServiceDefaults/ # OpenTelemetry, health checks, resilience
```

**Structure Decision**: Clean Architecture with strict inward dependency flow enforced by Architecture.Tests

## Phase 0 Research Results

Research findings are documented in:

- `/Users/murilobeltrame/Development/std/dotnet/llm-pairing-experimentation/backend/specs/001-azure-sql/research.md`

All technical context clarifications are resolved with explicit decisions on provider packages, migration strategy, connection resiliency, local dev database runtime, and contract-compatibility verification approach.

## Phase 1 Design Results

Design artifacts generated:

- `/Users/murilobeltrame/Development/std/dotnet/llm-pairing-experimentation/backend/specs/001-azure-sql/data-model.md`
- `/Users/murilobeltrame/Development/std/dotnet/llm-pairing-experimentation/backend/specs/001-azure-sql/contracts/rest-api-compatibility.md`
- `/Users/murilobeltrame/Development/std/dotnet/llm-pairing-experimentation/backend/specs/001-azure-sql/quickstart.md`

## Post-Design Constitution Check

- ✅ **I. Domain-Centric Clean Architecture**: Design keeps change-set in Infrastructure.Db, RestApi composition root, AppHost, and test infrastructure; no domain leakage.
- ✅ **II. Test-First**: Quickstart sequence begins with failing integration/provider tests before implementation updates.
- ✅ **III. Specification Pattern**: Data model and contracts do not introduce ad-hoc repository/query paths; Ardalis.Specification remains the access abstraction.
- ✅ **IV. Real Database Testing**: Quickstart requires SQL Server container-backed integration tests and provider-specific migration verification.
- ✅ **V. Domain Entity Integrity**: Entity model remains unchanged; no constructor/invariant policy regressions introduced.
- ✅ **VI. CQRS Messaging & Validation**: API contract remains stable and Wolverine command handling remains transport mechanism.
- ✅ **VII. Modern C# Style Rules**: Planned touched files remain under existing C# style constraints; no violations required.

No constitution violations require Complexity Tracking exceptions.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | N/A | N/A |

## Implementation Status (2026-03-15)

- SQL Server provider migration implemented across Infrastructure.Db, RestApi, AppHost, and integration tests.
- SQL Server migration set regenerated and validated.
- Full regression gate passed: `dotnet test ShoppingList.sln` (93 passed, 0 failed).
