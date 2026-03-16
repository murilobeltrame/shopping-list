# Tasks: Migrate Backend Database to Azure SQL

**Input**: Design documents from `/specs/001-azure-sql/`  
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

## Format: `[ID] [P?] [Story] Description`

- `[P]`: Can run in parallel (different files, no direct dependency)
- `[Story]`: User story identifier (`[US1]`, `[US2]`, `[US3]`)
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare package and configuration baseline for SQL Server/Azure SQL migration.

- [X] T001 Update SQL provider package references in `src/ShoppingList.Infrastructure.Db/ShoppingList.Infrastructure.Db.csproj`
- [X] T002 Update Aspire host package references for SQL Server resources in `env/ShoppingList.AppHost/ShoppingList.AppHost.csproj`
- [X] T003 [P] Add Azure SQL local/deployment configuration values in `src/ShoppingList.RestApi/appsettings.Development.json`
- [X] T004 [P] Add Azure SQL local/deployment configuration values in `src/ShoppingList.RestApi/appsettings.json`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Implement cross-story persistence wiring and test infrastructure changes that all stories depend on.

- [X] T005 Switch design-time DbContext provider from Npgsql to SQL Server in `src/ShoppingList.Infrastructure.Db/ApplicationContextFactory.cs`
- [X] T006 Switch runtime DbContext registration from PostgreSQL to SQL Server in `src/ShoppingList.RestApi/Program.cs`
- [X] T007 Replace PostgreSQL Aspire resource with Azure SQL resource in `env/ShoppingList.AppHost/AppHost.cs`
- [X] T008 Replace PostgreSQL TestContainers fixture with SQL Server fixture in `test/ShoppingList.Infrastructure.Db.Tests/PostgreSqlFixture.cs`
- [X] T009 [P] Update infrastructure test repository wiring for SQL Server fixture in `test/ShoppingList.Infrastructure.Db.Tests/ShoppingListTestRepository.cs`
- [X] T010 Regenerate SQL Server EF migrations in `src/ShoppingList.Infrastructure.Db/Migrations/`
- [X] T011 [P] Remove obsolete PostgreSQL migration artifacts from `src/ShoppingList.Infrastructure.Db/Migrations/20260312002750_InitialShoppingListSchema.cs`

**Checkpoint**: Foundation complete and all user stories can be implemented independently.

---

## Phase 3: User Story 1 - Persist all shopping-list operations on Azure SQL (Priority: P1) 🎯 MVP

**Goal**: All shopping list write/read flows persist correctly using Azure SQL/SQL Server provider.  
**Independent Test**: Execute create/add/purchase/remove/update/copy flows and verify persistence parity on SQL Server-backed infrastructure tests.

### Tests for User Story 1

- [X] T012 [P] [US1] Add provider-specific persistence assertions for create/add/purchase/remove/update/copy in `test/ShoppingList.Infrastructure.Db.Tests/CopyFlowPersistenceTests.cs`
- [X] T013 [P] [US1] Add SQL Server identity/key-generation persistence checks in `test/ShoppingList.Infrastructure.Db.Tests/GeneratedIdPersistenceTests.cs`
- [X] T014 [US1] Add startup migration smoke test against SQL Server in `test/ShoppingList.Infrastructure.Db.Tests/GeneratedIdPersistenceTests.cs`

### Implementation for User Story 1

- [X] T015 [US1] Configure SQL Server resilience and migration behavior in `src/ShoppingList.Infrastructure.Db/ApplicationContextFactory.cs`
- [X] T016 [US1] Enforce fail-fast startup behavior for missing/invalid DB configuration in `src/ShoppingList.RestApi/Program.cs`
- [X] T017 [P] [US1] Align SQL column mappings and constraints for provider parity in `src/ShoppingList.Infrastructure.Db/Configurations/ShoppingListConfiguration.cs`
- [X] T018 [P] [US1] Align SQL column mappings and constraints for provider parity in `src/ShoppingList.Infrastructure.Db/Configurations/ShoppingListItemConfiguration.cs`
- [X] T019 [US1] Validate repository wiring still resolves Ardalis specifications correctly in `src/ShoppingList.Infrastructure.Db/ServiceCollectionExtensions.cs`
- [X] T020 [P] [US1] Add or adjust provider-specific notes for migration commands in `backend/README.md`
- [X] T021 [US1] Verify end-to-end persistence invariants through existing mutation endpoints tests in `test/ShoppingList.RestApi.Tests/Endpoints/ShoppingListMutationTests.cs`

**Checkpoint**: User Story 1 is independently functional and testable.

---

## Phase 4: User Story 2 - Run local stack with containerized SQL Server (Priority: P1)

**Goal**: Developers can start AppHost locally with SQL Server container and run integration tests without Azure subscription.  
**Independent Test**: Run AppHost locally, confirm SQL Server container + API startup, and run infrastructure integration tests successfully.

### Tests for User Story 2

- [X] T022 [P] [US2] Add AppHost startup test coverage for SQL Server dependency readiness in `test/ShoppingList.RestApi.Tests/ShoppingListApiFactory.cs`
- [X] T023 [US2] Add local-container connection verification test in `test/ShoppingList.Infrastructure.Db.Tests/GeneratedIdPersistenceTests.cs`

### Implementation for User Story 2

- [X] T024 [US2] Configure SQL Server container settings and database reference in `env/ShoppingList.AppHost/AppHost.cs`
- [X] T025 [P] [US2] Update API test factory to use SQL Server-backed runtime configuration in `test/ShoppingList.RestApi.Tests/ShoppingListApiFactory.cs`
- [X] T026 [P] [US2] Update local development commands for SQL Server startup and checks in `specs/001-azure-sql/quickstart.md`
- [X] T027 [US2] Document local SQL Server inspection workflow in `specs/001-azure-sql/quickstart.md`
- [X] T028 [US2] Document backend local SQL Server build/test/run flow in `backend/README.md`

**Checkpoint**: User Stories 1 and 2 both work independently.

---

## Phase 5: User Story 3 - Preserve behavior and API compatibility (Priority: P2)

**Goal**: Existing API contracts and business behavior remain unchanged after provider migration.  
**Independent Test**: Run full backend test suite unchanged and verify compatibility contract checks pass.

### Tests for User Story 3

- [X] T029 [P] [US3] Verify create endpoint contract invariants remain unchanged in `test/ShoppingList.RestApi.Tests/Endpoints/ShoppingListCreationTests.cs`
- [X] T030 [P] [US3] Verify mutation endpoint status/body invariants remain unchanged in `test/ShoppingList.RestApi.Tests/Endpoints/ShoppingListMutationTests.cs`
- [X] T031 [P] [US3] Verify application handlers remain behaviorally identical under provider swap in `test/ShoppingList.Application.Tests/Handlers/CopyListHandlerTests.cs`
- [X] T032 [US3] Verify architecture dependency direction remains unchanged in `test/ShoppingList.Architecture.Tests/LayerDependencyTests.cs`

### Implementation for User Story 3

- [X] T033 [US3] Update compatibility verification notes and assertions in `specs/001-azure-sql/contracts/rest-api-compatibility.md`
- [X] T034 [US3] Update migration decisions and expected parity outcomes in `specs/001-azure-sql/research.md`

**Checkpoint**: All stories are independently functional and testable.

---

## Phase 6: Polish & Cross-Cutting

- [X] T035 [P] Update implementation sequencing and command checklist in `specs/001-azure-sql/quickstart.md`
- [X] T036 [P] Update final technical summary and constraints in `specs/001-azure-sql/plan.md`
- [X] T037 Run full backend regression suite and record pass criteria in `specs/001-azure-sql/quickstart.md`
- [X] T038 [P] Validate generated SQL Server migration metadata consistency in `src/ShoppingList.Infrastructure.Db/Migrations/ApplicationContextModelSnapshot.cs`
- [X] T039 Finalize feature task completion evidence in `specs/001-azure-sql/tasks.md`

---

## Dependencies & Execution Order

- Phase 1 must complete before Phase 2.
- Phase 2 must complete before Phases 3, 4, and 5.
- US1 (Phase 3) is the MVP and should be delivered first.
- US2 (Phase 4) depends on foundational AppHost/provider changes but can proceed in parallel with late US1 verification tasks.
- US3 (Phase 5) depends on US1 implementation being stable; it can run after US1 checkpoint and alongside US2 documentation tasks.
- Phase 6 starts after selected user stories are complete.

Recommended story order:

1. US1 (MVP)
2. US2
3. US3

## Parallel Execution Examples

### User Story 1

- Run T012 and T013 in parallel (different test files).
- Run T017 and T018 in parallel (separate configuration files).
- Run T020 in parallel with T021 (docs vs tests).

### User Story 2

- Run T025 and T026 in parallel (test factory vs quickstart docs).
- Run T027 and T028 in parallel (different documentation files).

### User Story 3

- Run T029, T030, and T031 in parallel (separate test files/projects).
- Run T033 and T034 in parallel (contract doc vs research doc).

## Implementation Strategy

### MVP First (US1)

- Complete Phases 1 and 2.
- Deliver US1 and validate persistence parity on SQL Server.
- This yields immediate business value: backend fully persists on Azure SQL-compatible engine.

### Incremental Delivery

- Add US2 to guarantee local developer operability and reproducible test flows.
- Add US3 to lock compatibility guarantees and prevent regressions.

### Validation Gates

- After each story checkpoint, run story-specific tests first.
- Before Phase 6 completion, run full suite: `dotnet test backend/ShoppingList.sln`.
