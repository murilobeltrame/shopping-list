---

description: "Task list template for feature implementation"
---

# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`  
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: Tests are REQUIRED by constitution (TDD), unless explicitly waived and justified in `plan.md`.

**Organization**: Tasks are grouped by user story so each story can be implemented and tested independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no direct dependency)
- **[Story]**: User story identifier (US1, US2, US3)
- Include exact file paths in every task

## Path Conventions

- **Source**: `src/[Project].Domain/`, `src/[Project].Application/`, `src/[Project].Infrastructure.Db/`, `src/[Project].RestApi/`
- **Tests**: `test/[Project].Domain.Tests/`, `test/[Project].Application.Tests/`, `test/[Project].Infrastructure.Db.Tests/`, `test/[Project].RestApi.Tests/`, `test/[Project].Architecture.Tests/`
- **Environment**: `env/[Project].AppHost/`, `env/[Project].ServiceDefaults/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Initialize solution and baseline conventions.

- [ ] T001 Create/verify project structure per implementation plan
- [ ] T002 Add required packages (Ardalis.Specification, Ardalis.Specification.EntityFrameworkCore, WolverineFx, FluentValidation if needed, Shouldly, AutoBogus, TestContainers)
- [ ] T003 [P] Configure `.editorconfig` and analyzers
- [ ] T004 [P] Configure Aspire host/service defaults as needed

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core architecture that blocks all user stories until complete.

- [ ] T005 Configure EF Core DbContext and migrations in `src/[Project].Infrastructure.Db/`
- [ ] T006 [P] Implement Ardalis.Specification repository wiring with Application abstractions and Infrastructure.Db EF concrete implementation
- [ ] T007 [P] Configure WolverineFx pipeline and handler registration in `src/[Project].Application/`
- [ ] T008 [P] Add FluentValidation pipeline behavior in `src/[Project].Application/` only when boundary validation is required beyond domain invariants
- [ ] T009 Configure TestContainers base fixtures for integration tests
- [ ] T010 Add/extend architecture tests for layer boundaries and dependency direction

**Checkpoint**: Foundation complete — user stories can proceed.

---

## Phase 3: User Story 1 - [Title] (Priority: P1) 🎯 MVP

**Goal**: [Brief outcome delivered by this story]  
**Independent Test**: [How this story is tested independently]

### Tests for User Story 1 (REQUIRED)

> Write tests first, confirm failure, then implement.

- [ ] T011 [P] [US1] Domain tests with Shouldly and AutoBogus in `test/[Project].Domain.Tests/`
- [ ] T012 [P] [US1] Application handler/validator tests in `test/[Project].Application.Tests/`
- [ ] T013 [P] [US1] Infrastructure integration tests with TestContainers in `test/[Project].Infrastructure.Db.Tests/`
- [ ] T014 [P] [US1] REST API endpoint tests in `test/[Project].RestApi.Tests/`

### Implementation for User Story 1

- [ ] T015 [P] [US1] Create/extend entity in `src/[Project].Domain/Entities/[EntityName].cs` without public default constructor
- [ ] T016 [US1] Add factory method(s) and behavior instance methods in Domain entity
- [ ] T017 [P] [US1] Implement command/query contracts in `src/[Project].Application/[Commands|Queries]/[Feature]/`
- [ ] T018 [US1] Implement FluentValidation validators in `src/[Project].Application/Validators/` only for boundary rules not already enforced with clear domain errors
- [ ] T019 [US1] Implement WolverineFx handlers for command/query orchestration
- [ ] T020 [US1] Implement specification(s) in `src/[Project].Application/Specifications/`
- [ ] T021 [US1] Implement persistence adapters in `src/[Project].Infrastructure.Db/`
- [ ] T022 [US1] Implement endpoint(s) in `src/[Project].RestApi/`

**Checkpoint**: User Story 1 is independently functional and testable.

---

## Phase 4: User Story 2 - [Title] (Priority: P2)

**Goal**: [Brief outcome delivered by this story]  
**Independent Test**: [How this story is tested independently]

### Tests for User Story 2 (REQUIRED)

- [ ] T023 [P] [US2] Domain/Application tests with Shouldly + AutoBogus
- [ ] T024 [P] [US2] Integration tests with TestContainers
- [ ] T025 [P] [US2] API tests

### Implementation for User Story 2

- [ ] T026 [P] [US2] Implement/extend Domain behavior first
- [ ] T027 [US2] Implement Application validator (if needed) + WolverineFx handler
- [ ] T028 [US2] Implement specification/persistence/API integration

**Checkpoint**: User Stories 1 and 2 both work independently.

---

## Phase 5: User Story 3 - [Title] (Priority: P3)

**Goal**: [Brief outcome delivered by this story]  
**Independent Test**: [How this story is tested independently]

### Tests for User Story 3 (REQUIRED)

- [ ] T029 [P] [US3] Domain/Application tests
- [ ] T030 [P] [US3] Integration tests with TestContainers
- [ ] T031 [P] [US3] API tests

### Implementation for User Story 3

- [ ] T032 [P] [US3] Implement/extend Domain behavior first
- [ ] T033 [US3] Implement Application validator (if needed) + WolverineFx handler
- [ ] T034 [US3] Implement specification/persistence/API integration

**Checkpoint**: All stories are independently functional.

---

## Phase N: Polish & Cross-Cutting

- [ ] T035 [P] Update docs and quickstart
- [ ] T036 Refactor while preserving tests
- [ ] T037 [P] Add missing architecture/compliance tests
- [ ] T038 Verify no public default constructors on new/changed entities
- [ ] T039 Run full test suite and fix story-scoped issues

---

## Dependencies & Execution Order

- Setup (Phase 1) starts immediately
- Foundation (Phase 2) blocks all stories
- Stories execute by priority (P1 → P2 → P3), parallel when feasible
- Polish executes after selected stories are complete

## Constitution Alignment Checklist

- [ ] Business rules are in Domain, not Application/API
- [ ] Application boundary validation strategy is explicit (FluentValidation when needed, otherwise clear domain enforcement)
- [ ] WolverineFx is used for dispatching (no MediatR/Mediator.Net)
- [ ] Domain entities avoid public default constructors
- [ ] Entity mutations use instance methods enforcing invariants
- [ ] Application uses Ardalis.Specification abstractions; Infrastructure.Db provides EF concrete implementations
- [ ] Integration DB tests use TestContainers
