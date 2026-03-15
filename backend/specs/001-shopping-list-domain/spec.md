# Feature Specification: Shopping List Domain Model & Core Use Cases

**Feature Branch**: `001-shopping-list-domain`  
**Created**: 2026-03-01  
**Status**: Draft  
**Input**: Domain entities for shopping list management with use cases for list and item management

## Clarifications

### Session 2026-03-01

- Q: What type should represent the Owner property? → A: String (user ID/email identifier)
- Q: What numeric type for Quantity property? → A: Integer, positive only (> 0 when specified), nullable allowed
- Q: What constraints for Description property? → A: 1-255 characters (short format for item names)
- Q: How are ShoppingLists uniquely identified? → A: Unique ID (GUID or integer); multiple lists per owner allowed
- Q: How are ShoppingListItems uniquely identified? → A: Unique ID using sequential GUID; allows duplicate descriptions and explicit item referencing

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create a Shopping List (Priority: P1)

As a user, I want to create a new shopping list so that I can start tracking items I need to purchase.

**Why this priority**: Creating a list is the foundational action; without it, users cannot perform any list operations. This is essential to the feature.

**Independent Test**: Can be fully tested by instantiating a ShoppingList entity with a required owner and optional date, and verifying the list is created with correct initial state (no items, not finished).

**Acceptance Scenarios**:

1. **Given** a user has not created a list yet, **When** the user creates a new list with required owner and optional date, **Then** the list is created successfully with no items and finished status is `false`
2. **Given** a user creates a list without a date, **When** the list is created, **Then** the date defaults to `null` and the list is still valid
3. **Given** a user creates a list with a past date, **When** validation occurs, **Then** the list accepts the date (date validation is not restricted)

---

### User Story 2 - Add Items to Shopping List (Priority: P1)

As a user, I want to add items to my shopping list so that I can track what I need to purchase.

**Why this priority**: Adding items is the primary workflow; users create lists specifically to add items to them. Essential for MVP.

**Independent Test**: Can be fully tested by creating a list and adding items with required description and optional quantity, verifying items are added and initial state is correct (not purchased, not removed).

**Acceptance Scenarios**:

1. **Given** a shopping list exists, **When** the user adds an item with a required description, **Then** the item is added with purchased=`false` and removed=`false`
2. **Given** a shopping list exists, **When** the user adds an item with description and quantity, **Then** the item is added with the specified quantity
3. **Given** a shopping list exists, **When** the user adds an item without quantity, **Then** the item is added with quantity defaulting to `null`
4. **Given** a shopping list has multiple items with various states, **When** a new item is added, **Then** the list's finished status is recalculated as `false` (since the new item is not purchased/removed)

---

### User Story 3 - Mark Item as Purchased (Priority: P2)

As a user, I want to mark items as purchased so that I can track my progress on the shopping list.

**Why this priority**: Core workflow feature; high value but secondary to item creation. Enables tracking completion.

**Independent Test**: Can be fully tested by creating a list with an item and marking it as purchased, verifying the item state changes and list's finished status is recalculated.

**Acceptance Scenarios**:

1. **Given** a shopping list has an unpurchased item, **When** the user marks the item as purchased, **Then** the item's purchased flag changes to `true`
2. **Given** a shopping list has one item marked as purchased, **When** that item is the only item in the list, **Then** the list's finished status is `true`
3. **Given** a shopping list has multiple items with mixed states, **When** all items are either purchased or removed, **Then** the list's finished status is `true`
4. **Given** a user marks an item as purchased, **When** the action is applied, **Then** the removed flag remains unchanged

---

### User Story 4 - Remove Item from Shopping List (Priority: P2)

As a user, I want to remove items from my shopping list so that I can manage items I no longer need to purchase.

**Why this priority**: Important workflow feature for list management; allows users to correct mistakes or mark items as no longer needed.

**Independent Test**: Can be fully tested by creating a list with items, removing one, and verifying the item state and list's finished status recalculation.

**Acceptance Scenarios**:

1. **Given** a shopping list has items, **When** the user removes an item, **Then** the item's removed flag is set to `true`
2. **Given** a shopping list has one item marked as removed, **When** that item is the only item in the list, **Then** the list's finished status is `true`
3. **Given** a user removes an item, **When** the action is applied, **Then** the purchased flag remains unchanged
4. **Given** a shopping list has all items either purchased or removed, **When** this state is reached, **Then** the list's finished status automatically becomes `true`

---

### User Story 5 - Update Item Quantity (Priority: P2)

As a user, I want to change the quantity of items on my shopping list so that I can purchase the correct amounts.

**Why this priority**: Enables users to manage item details; useful but not blocking MVP functionality.

**Independent Test**: Can be fully tested by creating an item with quantity, updating it, and verifying the quantity changes while other properties remain unaffected.

**Acceptance Scenarios**:

1. **Given** a shopping list has an item with a quantity, **When** the user changes the quantity, **Then** the item's quantity is updated to the new value
2. **Given** a shopping list has an item with a quantity, **When** the user sets quantity to `null`, **Then** the item's quantity is cleared
3. **Given** a user updates an item's quantity, **When** the update is applied, **Then** purchased and removed flags remain unchanged

---

### User Story 6 - Copy Existing Shopping List (Priority: P3)

As a user, I want to copy an existing shopping list so that I can quickly create a new list with the same items without manually re-entering them.

**Why this priority**: Convenience feature that improves user experience; valuable but not essential for MVP. Can be implemented after core workflows are stable.

**Acceptance Scenarios**:

1. **Given** a shopping list exists with multiple items in various states, **When** the user copies the list, **Then** a new list is created with the same owner and date
2. **Given** a shopping list is copied, **When** items are copied to the new list, **Then** all items have description and quantity matching the originals
3. **Given** items are copied to a new list, **When** they are copied, **Then** purchased and removed flags are reset to `false` (items start as unpurchased and not removed)
4. **Given** a source list is copied, **When** the copy is created, **Then** the original list remains unchanged

---

### Edge Cases

- What happens when a user tries to add an item with an empty or whitespace-only description? → Reject with validation error; must be 1-255 characters
- What happens when description exceeds 255 characters? → Reject with validation error
- How does the system handle quantity values that are zero or negative? → Reject with validation error; only positive integers (> 0) are valid when quantity is specified
- What happens when all items in a list are removed (removed=`true`)? → List should be marked as finished
- Can a user remove or purchase items from a list they don't own? → [OUT OF SCOPE] Ownership/permissions enforcement is deferred to a future iteration (RestApi/authorization layer). Current Domain/Application scope assumes an authorized caller context; identity/permission validation occurs at API boundary.

## Requirements *(mandatory)*

### Functional Requirements

**FR-001**: ShoppingList entity MUST have:
- Unique identifier (ID property; GUID or integer for uniquely identifying each list)
- Required Owner property (non-null, non-empty string representing the user ID/email identifier)
- Optional Date property (nullable DateTime; no validation constraints on past/future dates)
- Calculated Finished property: returns `true` if and only if ALL items in the list have either purchased=`true` OR removed=`true`

**FR-002**: ShoppingListItem entity MUST have:
- Unique identifier (ID property; sequential GUID for uniquely identifying each item and preserving order)
- Required Description property (non-empty string, 1-255 characters)
- Optional Quantity property (nullable positive integer; must be > 0 when specified, null is allowed, zero and negative values are invalid)
- Purchased property (boolean, defaults to `false`)
- Removed property (boolean, defaults to `false`)

**FR-003**: ShoppingList entity MUST NOT expose a public parameterless constructor; instantiation MUST occur through factory method or instance constructor that enforces owner requirement.

**FR-004**: ShoppingListItem entity MUST NOT expose a public parameterless constructor; instantiation MUST occur through factory method or instance constructor that enforces description requirement.

**FR-005**: The system MUST provide a use case for creating a ShoppingList with required owner and optional date.

**FR-006**: The system MUST provide a use case for adding ShoppingListItem to a ShoppingList, with required description and optional quantity; new items MUST default to purchased=`false` and removed=`false`.

**FR-007**: The system MUST provide a use case for marking a ShoppingListItem as purchased; this action MUST update the purchased flag and recalculate the list's finished status.

**FR-008**: The system MUST provide a use case for removing a ShoppingListItem from active tracking; this action MUST update the removed flag and recalculate the list's finished status.

**FR-009**: The system MUST provide a use case for updating a ShoppingListItem's quantity without affecting purchased or removed flags.

**FR-010**: The system MUST provide a use case for copying a ShoppingList with all its items; copied items MUST have purchased=`false` and removed=`false` regardless of source item state.

**FR-011**: ShoppingList MUST track all items and expose a collection of ShoppingListItem entities.

**FR-012**: All state changes MUST occur through domain entity methods or factory creation; no direct property mutation from external callers.

**FR-013**: ShoppingListItem quantity validation MUST reject zero and negative values. When quantity is specified (not null), it MUST be a positive integer (> 0).

**FR-014**: ShoppingListItem description validation MUST reject empty strings, whitespace-only strings, and descriptions exceeding 255 characters. Valid descriptions are 1-255 non-whitespace characters.

**FR-015**: ShoppingList MUST be uniquely identifiable by its ID. Multiple ShoppingLists per owner are allowed. Each owner can have many independent lists.

**FR-016**: ShoppingListItem MUST be uniquely identifiable by its sequential GUID. Multiple items with identical descriptions are allowed within a single list. Sequential GUID ensures database performance and natural ordering.

### Key Entities *(include if feature involves data)*

- **ShoppingList**: Represents a shopping list owned by a user. Has a unique identifier (ID) to distinguish between multiple lists owned by the same user. Contains a collection of ShoppingListItem entities and a calculated finished status. The finished status is derived: `true` if all items are either purchased or removed. Should support creation with optional date and required owner.

- **ShoppingListItem**: Represents an individual item on a shopping list. Has a unique sequential GUID identifier, required description (1-255 characters), optional positive integer quantity, and status flags for purchased and removed. Initial state has both flags as `false`. Items are added to ShoppingList and can transition between states through domain behavior methods. Multiple items with the same description are allowed.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Domain entities (ShoppingList and ShoppingListItem) correctly enforce invariants: ShoppingList requires owner, ShoppingListItem requires description, neither exposes public default constructors

- **SC-002**: ShoppingList's finished status is accurately calculated based on item states: list is finished only when all items have purchased=`true` OR removed=`true`

- **SC-003**: All six use cases can be executed without violating domain invariants: Create List, Add Item, Mark Purchased, Remove Item, Update Quantity, Copy List each functions as specified

- **SC-004**: Domain entities are fully tested with unit tests (Shouldly assertions) covering happy paths, edge cases, and invariant violations

- **SC-005**: All use case flows can transition items through their lifecycle (unpurchased → purchased, removed) without side effects or state leakage

- **SC-006**: Copy List use case produces a new list with items reset to purchased=`false` and removed=`false` regardless of source state

## Assumptions

- Date validation (past/future constraints) is not enforced at domain level; API layer may add such validation if needed
- Owner is a simple identifier/reference; ownership/permission model is handled by Application layer and RestApi, not enforced in Domain
- Quantity is a simple numeric value; unit/measurement semantics are not enforced in Domain
- List copying creates independent copies; no shared references between original and copied items
- Finished status is calculated on-demand; no persistence or caching of calculated state

## Out of Scope

- User authentication and authorization
- Sharing lists between users
- Real-time list synchronization
- Historical audit of item state changes
- Soft-delete vs. hard-delete semantics
- API endpoints and HTTP contracts
- Persistence layer and database schema
