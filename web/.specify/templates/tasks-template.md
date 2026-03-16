# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`  
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Path Conventions

- **Source**: `src/`
- **Tests**: `test/`, `e2e/`
- **Specs**: `specs/[###-feature-name]/`

## Phase 1: Setup

- [ ] T001 Create or update the chosen frontend structure under `src/`
- [ ] T002 Configure test and end-to-end scaffolding under `test/` and `e2e/`
- [ ] T003 [P] Create feature-specific contract notes in `specs/[###-feature-name]/contracts/`

## Phase 2: Foundations

- [ ] T004 Define route, screen, and state boundaries in `src/`
- [ ] T005 Define typed API contract mapping and error handling in `src/`
- [ ] T006 Define auth, caching, or optimistic update strategy only when required by the plan

## Phase 3: User Story 1 (P1)

**Goal**: [Describe story]
**Independent Test**: [How to validate this story independently]

- [ ] T007 [P] [US1] Add unit or component tests in `test/`
- [ ] T008 [P] [US1] Add integration or end-to-end coverage for the critical journey in `e2e/`
- [ ] T009 [US1] Implement typed models and state handling in `src/`
- [ ] T010 [US1] Implement UI, routing, and interactions in `src/`
- [ ] T011 [US1] Implement API integration and error mapping in `src/`

## Phase N: Polish & Cross-Cutting

- [ ] T0NN Update quickstart and contract notes in `specs/[###-feature-name]/`
- [ ] T0NN Validate accessibility, responsiveness, and failure states
- [ ] T0NN Refactor touched areas while preserving behavior
