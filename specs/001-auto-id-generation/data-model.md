# Data Model: Automatic ID Generation

## Entity: ShoppingList

- Purpose: Aggregate root representing a user-owned shopping list.
- Identity: `Id : Guid` (system-generated in Domain).
- Fields:
  - `Id : Guid` (required, immutable after creation)
  - `Owner : string` (required, non-empty/non-whitespace)
  - `Date : DateTime?` (optional)
  - `Items : IReadOnlyCollection<ShoppingListItem>` (child collection)
  - `Finished : bool` (computed, not persisted as source-of-truth)
- Validation rules:
  - `Owner` must be non-empty.
- State transitions:
  - Created via `Create(owner, date)` with generated `Id`.
  - Items added via `AddItem(description, quantity)`.
  - Item-level state changes delegated through aggregate methods.
  - Copy operation creates a new `ShoppingList` with new generated `Id`.

## Entity: ShoppingListItem

- Purpose: Child entity for shopping-list entries.
- Identity: `Id : Guid` (system-generated in Domain).
- Fields:
  - `Id : Guid` (required, immutable after creation)
  - `Description : string` (required, max 255)
  - `Quantity : int?` (optional, if provided must be > 0)
  - `Purchased : bool`
  - `Removed : bool`
- Validation rules:
  - `Description` must be non-empty and length <= 255.
  - `Quantity` must be null or > 0.
- State transitions:
  - Created through `ShoppingListItem.Create(...)` with generated `Id`.
  - `MarkPurchased`, `MarkRemoved`, and `UpdateQuantity` mutate item state.

## Relationships

- `ShoppingList (1) -> (many) ShoppingListItem`
- Persistence foreign key: `ShoppingListId` (shadow property in EF mapping).
- Delete behavior: cascade from list to items.

## ID Generation Rules

- IDs are generated only by Domain creation paths.
- Client request payloads for create flows do not define `id` fields.
- Generated IDs are returned to clients in create responses.
- IDs remain stable through persistence/retrieval and are used by all non-create commands.
