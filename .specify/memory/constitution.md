<!--
Sync Impact Report - Version 2.0.0 (2026-03-01)
================================================================
VERSION CHANGE: 1.0.0 → 2.0.0
BUMP RATIONALE: Backward-incompatible governance redefinition for mediator,
application responsibilities, and domain entity construction rules.

MODIFIED PRINCIPLES:
  - I. Clean Architecture & Dependency Inversion → I. Domain-Centric Clean Architecture
  - V. Modern C# Idioms → V. Domain Entity Integrity & Construction
  - VI. CQRS Pattern → VI. CQRS Messaging & Validation Boundaries
  - VII. Explicit Types Everywhere → VII. Modern C# Style Rules

ADDED SECTIONS:
  - None

REMOVED SECTIONS:
  - None

TEMPLATES REQUIRING UPDATES:
  ✅ .specify/templates/plan-template.md
  ✅ .specify/templates/tasks-template.md
  ✅ .github/copilot-instructions.md
  ⚠ .specify/templates/commands/*.md (directory not present)

FOLLOW-UP TODOs: None
================================================================
-->

# ShoppingList Constitution

## Core Principles

### I. Domain-Centric Clean Architecture

**Dependencies MUST flow inward**: RestApi → Application → Domain; Infrastructure.Db → Domain only.

- Domain layer has ZERO external dependencies and contains ALL business rules,
  invariants, and state transitions.
- Application layer orchestrates use cases, transactions, and integration flow;
  Application MUST NOT host core business rules.
- Infrastructure.Db depends ONLY on Domain for persistence concerns.
- RestApi depends on Application and Infrastructure.Db as composition root.
- Layer boundaries are enforced by `ShoppingList.Architecture.Tests`.

**Rationale**: Centralizing business logic in Domain protects invariants and
keeps behavior independent from delivery and persistence concerns.

### II. Test-First Development (NON-NEGOTIABLE)

**TDD is MANDATORY for all features** with strict Red-Green-Refactor:

1. Write tests first using Shouldly assertions.
2. Confirm tests fail.
3. Implement the minimum code to pass.
4. Refactor safely while keeping tests green.

**Test requirements**:

- One test project per source layer plus architecture tests.
- Shouldly MUST be used for assertions.
- AutoBogus MUST be used for fake data generation.
- Tests use Arrange-Act-Assert with descriptive names.

**Rationale**: Test-first development ensures behavior is specified before
implementation and prevents regressions.

### III. Specification Pattern for Data Access

**ALL EF Core queries MUST use Ardalis.Specification**.

- Query logic, includes, ordering, and pagination MUST be encapsulated in
  specification classes.
- Direct ad-hoc `DbSet` querying outside specifications is prohibited.
- Repositories MUST consume specifications for reads.

**Rationale**: Specifications make query behavior reusable, composable, and
testable.

### IV. Real Database Testing

**Integration database tests MUST run against real DB engines with TestContainers**.

- In-memory providers are prohibited for integration and migration testing.
- Infrastructure.Db tests MUST verify migrations and relational behavior on
  real containers.
- Test fixtures MUST create and dispose containers predictably.

**Rationale**: Real-database testing avoids false confidence caused by
behavioral differences in in-memory providers.

### V. Domain Entity Integrity & Construction

**Domain entities MUST protect invariants through controlled construction and behavior.**

- Domain entities MUST NOT expose a public parameterless constructor.
- Entity creation MUST happen through explicit factory methods and/or
  non-default constructors that enforce invariants.
- State changes MUST occur through entity instance methods, not property
  setters from outside the aggregate.
- Invalid state transitions MUST be rejected in Domain methods.

**Rationale**: Controlled construction and encapsulated behavior prevent invalid
states and keep invariants enforceable at all times.

### VI. CQRS Messaging & Validation Boundaries

**CQRS MUST be implemented in Application using Mediator.Net and FluentValidation.**

- Commands modify state; queries return data and MUST NOT mutate state.
- `Mediator.Net` is the mandatory dispatching library (MediatR is prohibited).
- Application request pre-validation MUST be implemented with FluentValidation.
- Validators handle input and boundary rules only; domain rules remain in Domain.
- Handlers coordinate repositories, domain behavior, and transaction boundaries.

**Rationale**: Clear boundaries keep validation and orchestration in Application
while preserving business logic authority in Domain.

### VII. Modern C# Style Rules

**C# code MUST follow project style constraints for readability and consistency.**

- Prefer primary constructors where applicable.
- Prefer expression-bodied members for simple members/accessors.
- `var` is prohibited; explicit types are required.
- Use file-scoped namespaces and project naming conventions from `.editorconfig`.

**Rationale**: Consistent style reduces cognitive load and review friction.

## Technology Stack & Dependencies

**Framework & Runtime**:

- .NET 10 (SDK 10.0.100) and C# 13
- ASP.NET Core Minimal APIs
- .NET Aspire for local orchestration

**Mandatory Libraries**:

- Data Access: Entity Framework Core, Ardalis.Specification
- Application Flow: Mediator.Net, FluentValidation
- Testing: xUnit, Shouldly, AutoBogus, TestContainers
- Observability: OpenTelemetry

**Project Structure Enforcement**:

- Source: `src/` (Domain, Application, Infrastructure.Db, RestApi)
- Tests: `test/` (one project per source layer + Architecture.Tests)
- Environment: `env/` (AppHost, ServiceDefaults)

## Code Quality & Architecture Enforcement

**Automated Enforcement**:

- Architecture tests MUST enforce dependency direction and layer isolation.
- Build and tests MUST pass before merge.
- Reviews MUST reject business-rule leakage from Domain into Application/API.

**Quality Gates**:

- New requests MUST include FluentValidation validators in Application.
- New domain entities MUST avoid public default constructors.
- New aggregate behavior MUST be implemented as entity instance methods.

## Governance

**Authority**: This constitution supersedes all other engineering practices.

**Amendment Process**:

1. Document change intent and rationale.
2. Classify version bump by governance impact.
3. Update constitution and prepend sync impact report.
4. Propagate changes to templates and runtime guidance.
5. Validate compliance checks and quality gates.

**Versioning Policy**:

- MAJOR: backward-incompatible principle redefinition/removal.
- MINOR: new principle/section or materially expanded governance.
- PATCH: clarification-only wording with no governance behavior change.

**Compliance Review Expectations**:

- Every feature plan MUST pass Constitution Check before implementation.
- PR review MUST verify all applicable MUST statements.
- Any justified violation MUST be documented in `plan.md` Complexity Tracking.

**Version**: 2.0.0 | **Ratified**: 2026-02-28 | **Last Amended**: 2026-03-01
