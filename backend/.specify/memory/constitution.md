<!--
Sync Impact Report - Version 3.3.0 (2026-03-16)
================================================================
VERSION CHANGE: 3.2.0 → 3.3.0
BUMP RATIONALE: MINOR - Added Principle VIII. REST API Design Standards with
comprehensive guidance on HTTP semantics, endpoint organization, request/response
handling, filtering, pagination, and query parameter composition. Includes mandatory
rule: Response DTOs MUST NEVER include navigation properties (only scalar properties
and IDs). This principle integrates REST best practices with the existing clean
architecture and CQRS model.

MODIFIED PRINCIPLES:
  - VIII. REST API Design Standards (NEW)

ADDED SECTIONS:
  - VIII. REST API Design Standards

REMOVED SECTIONS:
  - None

TEMPLATES REQUIRING UPDATES:
  ✅ .specify/templates/plan-template.md (to include REST API design review)
  ⚠ pending: .specify/templates/spec-template.md (optionally include REST endpoint schema section)
  ⚠ pending: Individual endpoint specs should document request/response shapes and HTTP semantics

FOLLOW-UP TODOs:
  - TODO(REST_SPEC_TEMPLATE): Consider adding optional REST design section to spec-template.md
    for features involving new endpoints.
  - TODO(ENDPOINT_EXAMPLES): Document example implementations in quickstart guides once
    endpoints are developed.
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

### VIII. REST API Design Standards

**All RestApi endpoints MUST follow REST best practices with explicit HTTP semantics.**

**HTTP Semantics & Status Codes**:

- GET requests MUST return 200 (OK) for successful retrieval, 404 (Not Found) when
  resource does not exist.
- POST requests MUST return 201 (Created) on successful creation, 400 (Bad Request)
  for invalid input, 409 (Conflict) for business logic violations.
- PUT/PATCH requests MUST return 200 (OK) or 204 (No Content) on success, 404 (Not Found)
  if resource does not exist, 400 (Bad Request) for validation failures.
- DELETE requests MUST return 204 (No Content) on successful deletion, 404 (Not Found)
  if resource does not exist.
- All error responses MUST return appropriate 4xx or 5xx status codes with meaningful
  error detail in response body.

**Endpoint Organization**:

- RestApi endpoints MUST be organized by entity under `/<entity>Endpoints/` directories.
- Each entity directory MUST include:
  - `RouterExtensions.cs`: extension method that registers all entity endpoints.
  - Subdirectories for each use case: `/<UseCase>Endpoint/`
    - `Endpoint.cs`: minimal API endpoint handler
    - `Requests/`: request DTOs (if needed beyond CQRS Command/Query)
    - `Responses/`: response DTOs (if needed beyond Command/Query result)
- Example structure:
  ```
  /ShoppingListEndpoints/
    RouterExtensions.cs
    /CreateShoppingListEndpoint/
      Endpoint.cs
      Requests/CreateRequest.cs
      Responses/CreateResponse.cs
    /GetShoppingListByIdEndpoint/
      Endpoint.cs
      Responses/ShoppingListResponse.cs
    /FetchShoppingListsEndpoint/
      Endpoint.cs
      Requests/FetchRequest.cs (for query parameters)
      Responses/ShoppingListResponse.cs
  ```

**Request & Response Design**:

- Request objects MUST be defined ONLY if the CQRS Command or Query doesn't naturally
  fit API design (e.g., mixed route + body parameters requiring a composite Request object).
- Response objects MUST be defined ONLY if the expected API output doesn't align with
  the default result of Ardalis.Specification operation or repository call.
- Response objects MUST NEVER include navigation properties. Responses MUST only contain
  scalar properties and explicit value objects; related entities MUST be referenced by ID
  only. Navigation properties compromise API contract stability and increase payload size.
- When API receives multiple query parameters, they MUST be contained in a single object
  mapped with `[FromQuery]` attribute.
- When API receives a request body, it MUST be defined as a single object.
- When API receives mixed parameters (route + body), a Request object MUST be defined
  and used with `[AsParameters]` attribute.

**Required Endpoint Operations**:

- Every entity MUST expose at least two endpoints:
  - **Get by ID** endpoint: returns single entity or 404 if not found.
  - **Fetch (List)** endpoint: returns paginated collection with optional filtering.

**Filtering Rules**:

- Fetch endpoints MUST support filtering on every column except Id.
- Text fields MUST support LIKE filtering when value includes `*` (query MUST replace `*`
  with SQL wildcard `%`).
- Numeric and DateTime columns MUST support min/max range filtering (separate parameters
  for lower and upper bounds).
- All filters MUST be optional; absent filters MUST not restrict results.
- Filters MUST be passed as query parameters within a single `[FromQuery]` object.

**Pagination**:

- Fetch endpoints MAY be paginated (pagination is optional per entity requirements).
- When paginated:
  - Page index parameter MUST default to 1 (1-based, not 0-based).
  - Page length parameter MUST default to 10.
  - Both parameters MUST be optional query parameters.
  - Response MUST include pagination metadata (total count, current page, page length).

**Rationale**: Explicit HTTP semantics ensure API consumers understand intent.
Organized endpoint structure keeps code maintainable and discoverable. Request/response
separation enforces clean boundaries. Query parameter composition reduces parameter
explosion and improves readability. Pagination and filtering patterns are industry-standard
and essential for working with large datasets. Mandatory entity-level Get and Fetch
operations provide minimum CRUD coverage.

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
- New RestApi endpoints MUST conform to Principle VIII (HTTP semantics, organization, pagination, filtering).
- New entities MUST have corresponding Get-by-Id and Fetch (List) endpoints.
- All filtering parameters MUST be composed into a single `[FromQuery]` object.
- Endpoints MUST use correct HTTP status codes (201 for creation, 204 for delete, 404 for missing, 400 for validation).
- Response DTOs MUST only be defined when API output structure differs from underlying query/command results.
- Response DTOs MUST NEVER expose navigation properties; only scalar properties and IDs are permitted.

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

**Version**: 3.3.0 | **Ratified**: 2026-02-28 | **Last Amended**: 2026-03-16
