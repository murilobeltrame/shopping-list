<!--
Sync Impact Report - Version 3.2.0 (2026-03-14)
================================================================
VERSION CHANGE: 3.1.0 → 3.2.0
BUMP RATIONALE: MINOR - Principle III materially expanded with two new MUST rules:
(1) ApplicationContext MUST NOT expose public DbSet<T> properties;
(2) Infrastructure MUST use open-generic EfRepository<T> as the canonical
implementation; entity-specific repository subclasses are prohibited for
standard CRUD/query scenarios. Principle VII expanded with explicit rule
banning private backing fields for constructor-injected parameters.

MODIFIED PRINCIPLES:
  - III. Specification-First Repository Pattern (expanded)
  - VII. Modern C# Style Rules (expanded)

ADDED SECTIONS:
  - None

REMOVED SECTIONS:
  - None

TEMPLATES REQUIRING UPDATES:
  ✅ .specify/templates/plan-template.md (Constitution Check item III updated)
  ✅ .specify/memory/constitution.md (quality gates aligned)
  ⚠ pending: .specify/templates/commands/*.md (directory does not exist in repository)

FOLLOW-UP TODOs:
  - TODO(COMMAND_TEMPLATES): Add `.specify/templates/commands/` if command templates are introduced,
    then align wording with Constitution v3.2.0.
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

### III. Specification-First Repository Pattern

**Repository pattern MUST rely on Ardalis.Specification abstractions end-to-end.**

- Application layer MUST depend on Ardalis.Specification abstractions
  (`IReadRepositoryBase<T>`, `IRepositoryBase<T>`, or equivalent abstractions from the library).
- Application layer MUST NOT define parallel custom repository contracts for standard
  CRUD/query behavior already covered by Ardalis.Specification abstractions.
- Infrastructure.Db MUST provide EF Core concrete implementations via a single open-generic
  repository (`EfRepository<T>`) registered for all `IRepositoryBase<T>` and
  `IReadRepositoryBase<T>` DI bindings. Entity-specific repository subclasses are
  prohibited unless the entity requires custom domain-driven query logic impossible to
  express through Ardalis.Specification specifications alone.
- `ApplicationContext` MUST NOT expose public `DbSet<T>` properties. All data access MUST
  go exclusively through the Ardalis.Specification repository layer, which uses EF Core's
  `Set<T>()` method internally.
- Query logic, includes, ordering, and pagination MUST be encapsulated in
  specification classes.
- Direct ad-hoc `DbSet` querying outside specifications is prohibited.

**Rationale**: Using one abstraction model avoids duplicate repository contracts,
keeps query behavior composable, and cleanly separates Application orchestration
from Infrastructure persistence implementation details. Hiding DbSet properties
prevents bypassing the repository abstraction from other layers.

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

**CQRS MUST be implemented in Application using WolverineFx with validation strategy flexibility.**

- Commands modify state; queries return data and MUST NOT mutate state.
- `WolverineFx` is the mandatory dispatching library (MediatR and Mediator.Net are prohibited).
- Application request pre-validation follows hybrid strategy:
  - SHOULD use FluentValidation for boundary/input validation (e.g., string length, null checks, format rules).
  - MAY omit FluentValidation when Domain construction/behavior already enforces the same invariants **without loss of error clarity at the boundary**.
  - Example: If `ShoppingList.Create(owner)` throws ArgumentException for empty owner with clear message, Application MAY delegate validation to domain rather than redundantly validate in a validator.
- Validators (when used) handle input and boundary rules only; domain rules remain in Domain.
- Handlers coordinate repositories, domain behavior, and transaction boundaries.

**Rationale**: Domain-centric validation keeps business logic in Domain where invariants
are enforced; redundant validation frameworks add overhead without stronger guarantees.
Clear, domain-enforced error messages provide sufficient boundary feedback.

### VII. Modern C# Style Rules

**C# code MUST follow project style constraints for readability and consistency.**

- Prefer primary constructors where applicable; constructor-injected parameters MUST be
  used directly and MUST NOT be copied into private backing fields.
- Prefer expression-bodied members for simple members/accessors.
- `var` is prohibited; explicit types are required.
- Use file-scoped namespaces and project naming conventions from `.editorconfig`.

**Rationale**: Consistent style reduces cognitive load and review friction.
Eliminating redundant backing fields keeps dependencies visible at the constructor
signature and avoids unnecessary boilerplate.

## Technology Stack & Dependencies

**Framework & Runtime**:

- .NET 10 (SDK 10.0.100) and C# 13
- ASP.NET Core Minimal APIs
- .NET Aspire for local orchestration

**Mandatory Libraries**:

- Data Access: Entity Framework Core, Ardalis.Specification, Ardalis.Specification.EntityFrameworkCore
- Application Flow: WolverineFx, FluentValidation
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

- New requests MUST document boundary validation strategy in Application:
  use FluentValidation when boundary validation is needed beyond domain-enforced invariants.
- New domain entities MUST avoid public default constructors.
- New aggregate behavior MUST be implemented as entity instance methods.
- New repository usage in Application MUST use Ardalis.Specification abstractions,
  and Infrastructure.Db MUST provide concrete EF implementations via open-generic `EfRepository<T>`.
- `ApplicationContext` MUST NOT expose public `DbSet<T>` properties; adding such properties is a
  build-review violation.
- Classes using primary constructors MUST NOT introduce private backing fields for
  constructor-injected parameters.

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

**Version**: 3.2.0 | **Ratified**: 2026-02-28 | **Last Amended**: 2026-03-14
