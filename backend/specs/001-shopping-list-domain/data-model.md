# Data Model: Shopping List Domain

**Feature**: 001-shopping-list-domain  
**Created**: 2026-03-01  
**Status**: Design Complete  
**Purpose**: Define entity structures, relationships, and invariant enforcement for the Shopping List domain

---

## Aggregate: ShoppingList

### Purpose
Root aggregate for managing shopping lists. Represents a user's shopping list with a collection of items to purchase.

### Identity
- **ID**: Guid (or int for EF Core identity; implementation detail deferred)
- **Identity Pattern**: Surrogate key; unique within system
- **Multiple per Owner**: Yes; owner can have many independent lists

### Properties

| Property | Type | Constraints | Default | Enforced By |
|----------|------|-----------|---------|-------------|
| **Id** | Guid | Not null, unique | Generated | Entity constructor |
| **Owner** | string | Not null, not empty, valid identifier | Required param | Constructor validation |
| **Date** | DateTime? | Optional, no future/past restrictions | null | None (optional) |
| **Items** | IReadOnlyCollection\<ShoppingListItem\> | Ordered, managed internally | Empty collection | Internal collection |
| **Finished** | bool (computed) | Calculated: ALL items are purchased OR removed | false | Property getter (on-demand) |

### Constructor Signature

```csharp
// Factory Method (preferred)
public static ShoppingList Create(string owner, DateTime? date = null)
{
    // Validates: owner must be non-null, non-empty, non-whitespace
    // Throws: ArgumentException if validation fails
    // Returns: new ShoppingList instance with empty items collection
}

// Private constructor (for factory/EF Core hydration)
private ShoppingList(Guid id, string owner, DateTime? date)
{
    // Enforcement: All parameters validated before assignment
    // Internal: Called only by factory or EF Core
}
```

### Invariants

1. **Owner Invariant**: Owner property is required, non-null, non-empty string
   - Validation: Constructor throws `ArgumentException("Owner cannot be null or empty")` if violated
   - Scope: Enforced at creation time; immutable after creation

2. **Items Collection Invariant**: Items collection is internally managed; cannot be directly modified from outside
   - Validation: `IReadOnlyCollection<ShoppingListItem>` prevents external mutation
   - Scope: Factory method `AddItem()`, `RemoveItem()` are sole entry points

3. **Finished Status Invariant**: Finished is true if and only if ALL items are completed (purchased OR removed)
   - Validation: Calculated on-demand; recomputed after item state changes
   - Scope: Recalculated after each item add/remove/state change

### Behavior Methods

#### AddItem
```csharp
public ShoppingListItem AddItem(string description, int? quantity = null)
{
    // Validates: description (delegates to ShoppingListItem.Create)
    // Validates: quantity (delegates to ShoppingListItem.Create)
    // Side Effects: Adds new item to internal items collection
    // Returns: Created ShoppingListItem (caller can capture ID)
    // Finished Status: Recalculated to false (new item is not completed)
}
```

**Constraints**:
- Description: 1-255 characters, non-empty, non-whitespace
- Quantity: Positive integer (> 0) if specified, null allowed
- Throws: `ArgumentException` if description or quantity invalid (via ShoppingListItem.Create)

#### RemoveItem
```csharp
public void RemoveItem(Guid itemId)
{
    // Validates: itemId exists in current items collection
    // Side Effects: Sets removed=true on the item
    // Finished Status: Recalculated (true if ALL items now completed)
    // Throws: InvalidOperationException("Item not found") if itemId not in collection
}
```

#### MarkItemPurchased
```csharp
public void MarkItemPurchased(Guid itemId)
{
    // Validates: itemId exists in current items collection
    // Side Effects: Sets purchased=true on the item
    // Finished Status: Recalculated (true if ALL items now completed)
    // Throws: InvalidOperationException("Item not found") if itemId not in collection
}
```

#### UpdateItemQuantity
```csharp
public void UpdateItemQuantity(Guid itemId, int? newQuantity)
{
    // Validates: itemId exists in current items collection
    // Validates: newQuantity (delegates to item.UpdateQuantity)
    // Side Effects: Updates item quantity
    // Finished Status: UNCHANGED (quantity doesn't affect finished calculation)
    // Throws: InvalidOperationException("Item not found") if itemId not in collection
    // Throws: ArgumentException if newQuantity invalid (via item.UpdateQuantity)
}
```

**Constraints**:
- newQuantity: Positive integer (> 0) if specified, null allowed

#### Copy
```csharp
public ShoppingList Copy(string newOwner, DateTime? newDate = null)
{
    // Validates: newOwner (delegates to ShoppingList.Create)
    // Side Effects: Creates new list with copied items (items reset to purchased=false, removed=false)
    // Returns: New independent ShoppingList instance
    // Original List: UNCHANGED (immutable pattern; original list unaffected)
    // Throws: ArgumentException if newOwner invalid (via ShoppingList.Create)
}
```

**Constraints**:
- newOwner: Same as Owner constraint (non-null, non-empty string)
- Quantity: Copied as-is from source items
- Item States: ALL copied items have purchased=false, removed=false regardless of source state

### Computed Properties

#### Finished
```csharp
public bool Finished
{
    get 
    {
        // Returns: true if collection is empty OR all items satisfy: (purchased || removed)
        // Logic: return !Items.Any() || Items.All(i => i.Purchased || i.Removed);
        // Calculation: On-demand (no caching)
        // Performance: O(n) where n = number of items; acceptable for expected list sizes (typically < 100 items)
    }
}
```

**Semantics**:
- Empty list with no items: Finished = false (per spec: "finished status is false" for new list)
- List with >= 1 item, all completed: Finished = true
- List with >= 1 item, any incomplete: Finished = false

---

## Entity: ShoppingListItem

### Purpose
Individual item within a ShoppingList. Represents a product or thing to purchase with associated metadata.

### Identity
- **ID**: Guid (sequential GUID preferred for ordering)
- **Identity Pattern**: Surrogate key; unique identifier for explicit item referencing
- **Scope**: Unique within parent ShoppingList (not globally unique)
- **Child of ShoppingList**: Cannot exist independently; always part of a list

### Properties

| Property | Type | Constraints | Default | Enforced By |
|----------|------|-----------|---------|-------------|
| **Id** | Guid | Not null, unique within list, sequential | Generated | Entity constructor |
| **Description** | string | Not null, 1-255 chars, non-empty | Required param | Constructor validation |
| **Quantity** | int? | Nullable, positive (> 0) if specified | null | Method validation |
| **Purchased** | bool | Boolean flag | false | Constructor default |
| **Removed** | bool | Boolean flag | false | Constructor default |

### Constructor Signature

```csharp
// Factory Method (preferred)
public static ShoppingListItem Create(string description, int? quantity = null)
{
    // Validates: description (1-255 chars, non-empty)
    // Validates: quantity (positive if not null)
    // Throws: ArgumentException if validation fails
    // Returns: new ShoppingListItem with purchased=false, removed=false, auto-generated Guid ID
}

// Private constructor (for factory/EF Core hydration)
private ShoppingListItem(Guid id, string description, int? quantity, bool purchased, bool removed)
{
    // Enforcement: All invariants already validated before constructor call
    // Internal: Called only by factory or EF Core
}
```

### Invariants

1. **Description Invariant**: Required, non-empty, 1-255 characters
   - Validation: Constructor throws `ArgumentException("Description cannot be null, empty, or exceed 255 characters")` if violated
   - Scope: Enforced at creation time; immutable after creation
   - Examples:
     - Valid: "Apples", "2% Milk (half gallon)", "Organic free-range eggs"
     - Invalid: "", " ", "A very long description exceeding 255 characters..."

2. **Quantity Invariant**: Nullable; if specified (not null), must be positive integer > 0
   - Validation: Constructor and `UpdateQuantity()` throw `ArgumentException("Quantity must be positive (> 0) or null")` if violated
   - Scope: Validated at creation and update
   - Examples:
     - Valid: null, 1, 2, 5, 100
     - Invalid: 0, -1, -5

3. **Initial State Invariant**: New items always have purchased=false, removed=false
   - Validation: Constructor hardcodes these values
   - Scope: Immutable; ensured at creation
   - Rationale: Items cannot be created in "already completed" state

4. **State Exclusivity**: Item state transitions are additive (not mutually exclusive)
   - Note: Item can have both purchased=true AND removed=true (both flags are independent)
   - Semantics: "Marked as purchased but then removed from list" is valid state
   - Completion: Item is considered "completed" if purchased OR removed (not AND)

### Behavior Methods

#### MarkPurchased
```csharp
public void MarkPurchased()
{
    // Side Effects: Sets purchased = true
    // State: removed flag UNCHANGED
    // Returns: void (modifies in-place)
}
```

**State Examples**:
- Before: purchased=false, removed=false → After: purchased=true, removed=false ✓
- Before: purchased=true, removed=false → After: purchased=true, removed=false (idempotent)
- Before: purchased=false, removed=true → After: purchased=true, removed=true (both flags can be set)

#### MarkRemoved
```csharp
public void MarkRemoved()
{
    // Side Effects: Sets removed = true
    // State: purchased flag UNCHANGED
    // Returns: void (modifies in-place)
}
```

**State Examples**:
- Before: purchased=false, removed=false → After: purchased=false, removed=true ✓
- Before: purchased=false, removed=true → After: purchased=false, removed=true (idempotent)
- Before: purchased=true, removed=false → After: purchased=true, removed=true (both flags can be set)

#### UpdateQuantity
```csharp
public void UpdateQuantity(int? newQuantity)
{
    // Validates: newQuantity (positive if not null)
    // Side Effects: Sets Quantity = newQuantity
    // State: purchased and removed flags UNCHANGED
    // Returns: void (modifies in-place)
    // Throws: ArgumentException("Quantity must be positive (> 0) or null") if invalid
}
```

**Constraints**:
- newQuantity: Positive integer (> 0) if specified, null allowed
- Examples:
  - Valid updates: null → 5, 5 → 3, 2 → null
  - Invalid updates: 5 → 0, 5 → -1

#### IsCompleted
```csharp
public bool IsCompleted()
{
    // Returns: true if (purchased || removed)
    // Logic: return Purchased || Removed;
    // Semantics: Item is "done" if it's been purchased or removed from list
}
```

### State Transition Diagram

```
Initial State (after Create):
┌─────────────────────────────────────────┐
│ purchased=false, removed=false          │
│ Completed=false                         │
└─────────────────────────────────────────┘
       ↙                              ↖
   MarkPurchased()               MarkRemoved()
       ↓                              ↓
┌─────────────┐                 ┌─────────────┐
│ purchased=T │                 │ removed=T   │
│ removed=F   │  ←→ (both OK)   │ purchased=F │
│ Completed=T │    ↔            │ Completed=T │
└─────────────┘                 └─────────────┘
       ↓                              ↓
       └──────────→ ↙────────────────┘
            Both True
       (Edge case state)
    purchased=T, removed=T
    Completed=T
```

---

## Relationships

### ShoppingList ↔ ShoppingListItem

**Cardinality**: 1:N (one list has many items)

**Lifecycle**:
- Items cannot exist without a parent ShoppingList
- Items are created via `ShoppingList.AddItem()`
- Items are removed via `ShoppingList.RemoveItem()`
- No orphaned items; deletion is cascading

**Referential Integrity**:
- Items store no explicit reference to parent list (implicit via collection membership)
- EF Core: Configured with parent-child relationship; cascade delete enforced at DB level

**Navigation**:
- Parent → Child: `ShoppingList.Items` (IReadOnlyCollection)
- Child → Parent: Not exposed in domain; maintained via EF Core FK (ShoppingListId in DB)

---

## Validation Rules Summary

### Domain Layer Validation (Constructor Enforcement)

| Rule | Entity | Trigger | Behavior |
|------|--------|---------|----------|
| Owner non-empty | ShoppingList | `Create(owner)` | Throw ArgumentException |
| Description 1-255 | ShoppingListItem | `Create(description)` | Throw ArgumentException |
| Quantity positive | ShoppingListItem | `Create(qty)` or `UpdateQuantity(qty)` | Throw ArgumentException if qty ≤ 0 (when not null) |
| Finished calculated | ShoppingList | Auto (on property read) | Compute !(Items.Any()) \|\| Items.All(i => i.Purchased \|\| i.Removed) |

### No FluentValidation
- Domain invariants enforced by constructors/methods
- No validator classes needed (per design decision)
- Exceptions bubble to Application layer (handlers don't catch them unless need to transform)

---

## Type Definitions (C# Code Shapes)

### ShoppingList
```csharp
namespace ShoppingList.Domain;

public sealed class ShoppingList
{
    // Identity
    public Guid Id { get; }
    
    // Data
    public string Owner { get; }
    public DateTime? Date { get; }
    
    // Collections
    public IReadOnlyCollection<ShoppingListItem> Items { get; }
    
    // Computed
    public bool Finished => !Items.Any() || Items.All(i => i.Purchased || i.Removed);
    
    // Factory
    public static ShoppingList Create(string owner, DateTime? date = null) { ... }
    
    // Behaviors
    public ShoppingListItem AddItem(string description, int? quantity = null) { ... }
    public void RemoveItem(Guid itemId) { ... }
    public void MarkItemPurchased(Guid itemId) { ... }
    public void UpdateItemQuantity(Guid itemId, int? newQuantity) { ... }
    public ShoppingList Copy(string newOwner, DateTime? newDate = null) { ... }
    
    // Private constructor for factory/EF Core
    private ShoppingList(Guid id, string owner, DateTime? date) { ... }
}
```

### ShoppingListItem
```csharp
namespace ShoppingList.Domain;

public sealed class ShoppingListItem
{
    // Identity
    public Guid Id { get; }
    
    // Data
    public string Description { get; }
    public int? Quantity { get; private set; }
    public bool Purchased { get; private set; }
    public bool Removed { get; private set; }
    
    // Semantics
    public bool IsCompleted() => Purchased || Removed;
    
    // Factory
    public static ShoppingListItem Create(string description, int? quantity = null) { ... }
    
    // Behaviors
    public void MarkPurchased() { ... }
    public void MarkRemoved() { ... }
    public void UpdateQuantity(int? newQuantity) { ... }
    
    // Private constructor for factory/EF Core
    private ShoppingListItem(Guid id, string description, int? quantity, bool purchased, bool removed) { ... }
}
```

---

## EF Core Mapping Considerations

### Entity Type Configuration

**ShoppingList**:
- Primary Key: `Id` (Guid)
- Required Properties: `Owner` (string, max length 255 or VARCHAR)
- Optional Properties: `Date` (DateTime?)
- Shadow FK: `ICollection<ShoppingListItem> Items` (one-to-many)

**ShoppingListItem**:
- Primary Key: `Id` (Guid)
- Foreign Key: `ShoppingListId` (Guid, required, shadow property)
- Required Properties: `Description` (string, max length 255)
- Optional Properties: `Quantity` (int?)
- Value Properties: `Purchased` (bool), `Removed` (bool)

### No Separate Value Objects
- Items are entities (have identity via ID), not value objects
- No additional value object wrappers needed (Owner is just string, no vo needed)

---

## Summary Table: Complete Entity Reference

| Aspect | ShoppingList | ShoppingListItem |
|--------|--------------|-----------------|
| **Type** | Aggregate Root | Entity |
| **Identity** | Guid Id | Guid Id |
| **Lifecycle** | Independent | Owned by ShoppingList |
| **Creation** | `ShoppingList.Create(owner, date?)` | `ShoppingListItem.Create(desc, qty?)` |
| **Is Mutable** | Yes (items added/removed) | Yes (qty, flags modified) |
| **Immutable Props** | Owner | Description, Id |
| **Mutable Props** | Items collection | Quantity, Purchased, Removed |
| **Computed Props** | Finished | IsCompleted (method) |
| **Key Invariants** | Owner non-empty; Items managed | Description 1-255; Qty > 0 if set |
| **Validation** | Constructor throws | Constructor & methods throw |
| **Exceptions** | ArgumentException, InvalidOperationException | ArgumentException |

---

## Design Rationale

### Why No Public Default Constructors?
- Ensures all invariants checked at construction time
- Prevents invalid state creation (e.g., empty-owner list)
- Aligns with Clean Architecture and Domain-Driven Design principles
- Forces explicit factory methods or explicit parameter constructors

### Why Sequential GUID for ShoppingListItem ID?
- Preserves insertion order in database (unlike random GUID)
- Better clustering and query performance
- Natural ordering for UI display without explicit sort
- Avoids fragmentation in B-tree indexes

### Why IsCompleted() as Method, Not Property?
- Computed property possible; method chosen for clarity
- Method name explicitly signals computation
- Allows logging/debugging hooks if needed later
- Distinguishes from simple state properties

### Why No FluentValidation in Domain?
- Domain layer should own validation logic
- Constructor exceptions provide clear throw points
- Avoids duplication (validation in two places)
- Simpler mental model: valid aggregates only exist post-construction

---

## Future Considerations

### For Future Phases
- **ID Strategy**: Currently deferred to Infrastructure layer (Guid vs int)
- **Event Sourcing**: Domain could emit events (ShoppingListCreated, ItemAdded, etc.)
- **Concurrency**: Optimistic locking via ETag or Version property
- **Soft Delete**: Logical delete flag for historical audit (not in scope)
- **Audit Trail**: CreatedAt, UpdatedAt timestamps, CreatedBy tracking
