# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`  
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Path Conventions

- **Source**: `src/ShoppingList.Android/`
- **Tests**: `test/ShoppingList.Android.Tests/`, `test/ShoppingList.Android.UiTests/`
- **Specs**: `specs/[###-feature-name]/`

## Phase 1: Setup

- [ ] T001 Create or update Android feature structure under `src/ShoppingList.Android/`
- [ ] T002 Configure test targets or shared Android test helpers under `test/`
- [ ] T003 [P] Create feature-specific contract notes in `specs/[###-feature-name]/contracts/`

## Phase 2: Foundations

- [ ] T004 Define screen state, navigation, and data flow in `src/ShoppingList.Android/`
- [ ] T005 Define backend contract mapping and error handling boundaries in `src/ShoppingList.Android/`
- [ ] T006 Define persistence, offline, or background work behavior only when required by the plan

## Phase 3: User Story 1 (P1)

**Goal**: [Describe story]
**Independent Test**: [How to validate this story independently]

- [ ] T007 [P] [US1] Add unit tests for feature state and business rules in `test/ShoppingList.Android.Tests/`
- [ ] T008 [P] [US1] Add instrumentation or UI coverage for the critical journey in `test/ShoppingList.Android.UiTests/`
- [ ] T009 [US1] Implement models, state holders, or presenters in `src/ShoppingList.Android/`
- [ ] T010 [US1] Implement screens and navigation flow in `src/ShoppingList.Android/`
- [ ] T011 [US1] Implement backend integration and error mapping in `src/ShoppingList.Android/`

## Phase N: Polish & Cross-Cutting

- [ ] T0NN Update quickstart and contract notes in `specs/[###-feature-name]/`
- [ ] T0NN Validate accessibility, loading, empty, and failure states
- [ ] T0NN Refactor touched areas while preserving behavior
