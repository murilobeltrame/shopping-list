# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`  
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Path Conventions

- **Source**: `src/ShoppingList.iOS/`
- **Tests**: `test/ShoppingList.iOS.Tests/`, `test/ShoppingList.iOS.UITests/`
- **Specs**: `specs/[###-feature-name]/`

## Phase 1: Setup

- [ ] T001 Create or update iOS feature structure under `src/ShoppingList.iOS/`
- [ ] T002 Configure test targets or shared test helpers under `test/`
- [ ] T003 [P] Create feature-specific contract notes in `specs/[###-feature-name]/contracts/`

## Phase 2: Foundations

- [ ] T004 Define view state, navigation, and data flow for the feature in `src/ShoppingList.iOS/`
- [ ] T005 Define backend contract mapping and error handling boundaries in `src/ShoppingList.iOS/`
- [ ] T006 Define persistence or caching behavior only if required by the feature plan

## Phase 3: User Story 1 (P1)

**Goal**: [Describe story]
**Independent Test**: [How to validate this story independently]

- [ ] T007 [P] [US1] Add unit tests for feature state and business rules in `test/ShoppingList.iOS.Tests/`
- [ ] T008 [P] [US1] Add UI or integration coverage for the critical journey in `test/ShoppingList.iOS.UITests/`
- [ ] T009 [US1] Implement models, presenters, or feature state in `src/ShoppingList.iOS/`
- [ ] T010 [US1] Implement screens and navigation flow in `src/ShoppingList.iOS/`
- [ ] T011 [US1] Implement backend integration and error mapping in `src/ShoppingList.iOS/`

## Phase N: Polish & Cross-Cutting

- [ ] T0NN Update quickstart and contract notes in `specs/[###-feature-name]/`
- [ ] T0NN Validate accessibility, loading, empty, and failure states
- [ ] T0NN Refactor touched areas while preserving behavior
