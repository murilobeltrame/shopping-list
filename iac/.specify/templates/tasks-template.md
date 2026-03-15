# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`  
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Path Conventions

- **Modules**: `modules/`
- **Environments**: `environments/`
- **Validation**: `test/`
- **Specs**: `specs/[###-feature-name]/`

## Phase 1: Setup

- [ ] T001 Create or update module and environment structure under `modules/` and `environments/`
- [ ] T002 Configure validation, formatting, and plan tooling under `test/` or local tooling files
- [ ] T003 [P] Create feature-specific contract and rollout notes in `specs/[###-feature-name]/contracts/`

## Phase 2: Foundations

- [ ] T004 Define module inputs, outputs, and environment boundaries
- [ ] T005 Define identity, secret, and access requirements
- [ ] T006 Define state, drift detection, and rollout strategy

## Phase 3: User Story 1 (P1)

**Goal**: [Describe story]
**Independent Test**: [How to validate this story independently]

- [ ] T007 [P] [US1] Add validation or policy coverage in `test/`
- [ ] T008 [US1] Implement or update shared modules in `modules/`
- [ ] T009 [US1] Implement or update environment composition in `environments/`
- [ ] T010 [US1] Capture plan, expected blast radius, and rollback notes in `specs/[###-feature-name]/quickstart.md`

## Phase N: Polish & Cross-Cutting

- [ ] T0NN Update operational notes and contract docs in `specs/[###-feature-name]/`
- [ ] T0NN Re-run validation and plan review
- [ ] T0NN Refactor touched infrastructure definitions while preserving intent
