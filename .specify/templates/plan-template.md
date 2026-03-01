# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

**Language/Version**: .NET 10 (SDK 10.0.100), C# 13  
**Primary Dependencies**: ASP.NET Core, Entity Framework Core, Ardalis.Specification, WolverineFx, FluentValidation  
**Storage**: [Specify database: PostgreSQL, SQL Server, or NEEDS CLARIFICATION]  
**Testing**: xUnit, Shouldly (assertions), AutoBogus (fake data), TestContainers (real DB tests)  
**Target Platform**: ASP.NET Core REST API, .NET Aspire orchestration for local development  
**Project Type**: Clean Architecture REST API with CQRS pattern  
**Performance Goals**: [domain-specific, e.g., <200ms p95 latency, 1000 req/s or NEEDS CLARIFICATION]  
**Constraints**: [domain-specific, e.g., ACID transactions required, eventual consistency acceptable or NEEDS CLARIFICATION]  
**Scale/Scope**: [domain-specific, e.g., 10k users, 100k entities, multi-tenant or NEEDS CLARIFICATION]

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Verify compliance with ShoppingList Constitution (`.specify/memory/constitution.md`):

- ✅ **I. Domain-Centric Clean Architecture**: Is business logic confined to Domain and dependency flow inward only?
- ✅ **II. Test-First**: Are tests planned BEFORE implementation (Red-Green-Refactor)?
- ✅ **III. Specification Pattern**: Are all queries using Ardalis.Specification?
- ✅ **IV. Real Database Testing**: Are TestContainers planned for DB integration tests?
- ✅ **V. Domain Entity Integrity**: Do entities avoid public default constructors and enforce behavior via instance methods?
- ✅ **VI. CQRS Messaging & Validation**: Are handlers dispatched with WolverineFx and pre-validations implemented with FluentValidation?
- ✅ **VII. Modern C# Style Rules**: Are primary constructors/expression-bodied members preferred and `var` avoided?

If any violations are needed, document justification in Complexity Tracking section below.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Verify the structure below matches your actual project layout.
  For ShoppingList, this follows .NET Clean Architecture with Aspire orchestration.
-->

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

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
