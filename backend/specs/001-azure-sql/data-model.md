# Data Model: Migrate Backend Database to Azure SQL

## Overview

Business entities remain unchanged. This feature changes persistence provider and schema dialect only.

## Entities

### ShoppingList

- Description: Aggregate root representing a shopping list owned by a user.
- Fields:
- `Id` (Guid, required)
- `Owner` (string, required)
- `Date` (DateTime?, optional)
- `Finished` (bool, required)
- Relationships:
- One-to-many with `ShoppingListItem`
- Validation rules:
- Owner is required and non-empty
- Finished is derived/recomputed by domain behavior
- State transitions:
- Created
- Updated as items are added/changed
- Finished becomes true when all items are purchased or removed

### ShoppingListItem

- Description: Item belonging to a shopping list.
- Fields:
- `Id` (Guid, required)
- `ShoppingListId` (Guid, required FK)
- `Description` (string, required)
- `Quantity` (int?, optional)
- `Purchased` (bool, required)
- `Removed` (bool, required)
- Relationships:
- Many-to-one with `ShoppingList`
- Validation rules:
- Description is required within existing domain constraints
- Quantity is optional and must respect existing domain constraints when provided
- State transitions:
- Added (default Purchased=false, Removed=false)
- Marked purchased
- Marked removed
- Quantity updated

### MigrationArtifact

- Description: Provider-specific EF migration metadata and scripts for SQL Server.
- Fields:
- `MigrationId` (string)
- `ProductVersion` (string)
- `Provider` (string, expected SQL Server)
- Validation rules:
- Migration chain must be SQL Server-consistent
- No PostgreSQL provider SQL should remain in active migration set

## Referential and Behavioral Invariants

- Foreign key from `ShoppingListItem.ShoppingListId` to `ShoppingList.Id` remains enforced.
- Existing uniqueness and indexing semantics remain unchanged unless required by SQL Server provider conventions.
- API-level payload semantics for list/item resources remain unchanged.
