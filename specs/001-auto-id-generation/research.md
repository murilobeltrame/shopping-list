# Research: Automatic ID Generation

## Decision 1: Domain-generated IDs remain the single source of truth

- Decision: Keep ID generation in Domain factories/behavior (`ShoppingList.Create`, `ShoppingListItem.Create`, and list copy flow via `AddItem`) using `Guid.NewGuid()`.
- Rationale: This aligns with Domain-centric invariants and guarantees that creation paths do not trust client-provided IDs.
- Alternatives considered: 
  - Database-generated IDs: rejected because entity identity would be unavailable before persistence and require larger persistence refactors.
  - API-layer ID generation: rejected because it leaks business identity rules outside Domain.

## Decision 2: Creation API contracts omit ID fields in request payloads

- Decision: Define create request payloads without `id` fields (`POST /shopping-lists`, `POST /shopping-lists/{listId}/items`, `POST /shopping-lists/{listId}/copy`).
- Rationale: The most reliable way to ignore client-supplied IDs is to avoid binding these fields in the first place.
- Alternatives considered:
  - Accept-and-ignore ID fields: rejected because it keeps ambiguous API contracts and encourages misuse.

## Decision 3: Return generated IDs in create responses

- Decision: All create endpoints return `201 Created` with body `{ "id": "<guid>" }` and `Location` header.
- Rationale: Satisfies traceability requirement and supports follow-up operations with persisted IDs.
- Alternatives considered:
  - Return only `Location` header: rejected because response-body ID is explicitly required by feature success criteria.

## Decision 4: Validation strategy stays hybrid with Domain as primary invariant guard

- Decision: Keep domain-enforced validation for owner/description/quantity invariants; no additional FluentValidation rules are needed for this feature.
- Rationale: Existing domain error messages are clear and the feature introduces no new boundary-only validation rule.
- Alternatives considered:
  - Add FluentValidation on create DTOs: rejected as redundant for currently covered invariants.

## Decision 5: Test approach for feature acceptance

- Decision: Verify behavior through Application and RestApi tests; include uniqueness assertions across multiple create calls.
- Rationale: This directly proves FR-001..FR-008 and SC-001..SC-004 from consumer-visible behavior.
- Alternatives considered:
  - Infrastructure-only verification: rejected because it does not validate API contract behavior end-to-end.

## Resolved Technical Context

- Storage: PostgreSQL (via Aspire + EF Core).
- Performance goals: no regression from baseline create flows; maintain low-latency local create operations.
- Constraints: ACID consistency for aggregate persistence and ID stability across retrievals.
- Scale/scope: single-service shopping list API, standard consumer workload, UUID collision risk effectively negligible for required scale.
