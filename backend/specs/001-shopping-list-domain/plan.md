# Implementation Plan: Shopping List Domain Model & Core Use Cases

**Branch**: `001-shopping-list-domain` | **Date**: 2026-03-01 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/001-shopping-list-domain/spec.md`

## Summary

Implement the foundational domain model for shopping list management with two aggregate roots (ShoppingList and ShoppingListItem) following domain-driven design principles. The domain layer enforces all invariants: ShoppingList requires an owner, ShoppingListItem requires a non-empty description (1-255 chars), quantities must be positive integers when specified, and ShoppingList's finished status is calculated from item states. Application layer provides six CQRS use case handlers (Create List, Add Item, Mark Purchased, Remove Item, Update Quantity, Copy List) with minimal validation delegation—domain rules stay in Domain, boundary/input validation in Application only when domain logic is insufficient.

## Technical Context

**Language/Version**: .NET 10 (SDK 10.0.100), C# 13  
**Primary Dependencies**: Entity Framework Core (persistence), WolverineFx (CQRS dispatch), FluentValidation (selective boundary validation only)  
**Storage**: PostgreSQL (via EF Core)  
**Testing**: xUnit, Shouldly (assertions), AutoBogus (fake data), TestContainers (real DB tests)  
**Target Platform**: Clean Architecture REST API with .NET Aspire orchestration  
**Project Type**: Domain + Application layers (RestApi layer explicitly excluded from this iteration per user request)  

**Performance Goals**: No specific requirements; general best practices (avoid N+1 queries, use specifications for queries)  
**Constraints**: ACID transactions for list operations, calculated finished status computed on-demand  
**Scale/Scope**: Single-tenant, no real-time sync, focused on domain correctness over scale

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Verify compliance with ShoppingList Constitution (`.specify/memory/constitution.md`):

- ✅ **I. Domain-Centric Clean Architecture**: Business logic confined to Domain; dependency flow Domain ← Application (handlers call domain methods). No external dependencies in Domain.
- ✅ **II. Test-First**: TDD applied: entity invariant tests first, then factory methods, then use case handlers. Red-Green-Refactor enforced.
- ✅ **III. Specification Pattern**: Queries will use Ardalis.Specification in Infrastructure.Db layer; Application layer defines specifications near handlers for query logic encapsulation.
- ✅ **IV. Real Database Testing**: TestContainers used for Infrastructure.Db.Tests (deferred to follow-up iteration); domain tests are pure unit tests (no container required).
- ✅ **V. Domain Entity Integrity**: ShoppingList and ShoppingListItem have no public default constructors; instantiation via factory methods or primary constructors enforcing invariants.
- ✅ **VI. CQRS Messaging & Validation**: Handlers dispatched via WolverineFx; FluentValidation MAY be omitted for domain rules where constructors/methods enforce invariants clearly (per Constitution v3.0.0); boundary-level input validation uses FluentValidation when needed.
- ✅ **VII. Modern C# Style Rules**: Primary constructors used; expression-bodied accessors; explicit types (no `var`); file-scoped namespaces.

**Complexity Decisions / Deviations**: 
- **Validation Strategy Deviation**: Per user request, FluentValidation is deferred or omitted where domain logic is sufficient. Only boundary-level input validation (e.g., string length, null checks that can't be tested via domain construction) uses FluentValidation. Domain invariants (Owner non-empty, Quantity > 0, Description 1-255) are enforced by entity constructors/methods, not validators.
- **API Layer**: Explicitly excluded from this iteration per user request; RestApi implementation deferred to future phase.

## Design & Architecture Decisions

### Domain Layer Strategy

**Entities & Aggregates**:

1. **ShoppingList (Aggregate Root)**
   - Unique ID (GUID or integer; decision deferred to Infrastructure layer)
   - Owner (string, required, non-empty; enforced in constructor)
   - Date (DateTime?, optional)
   - Items (IReadOnlyCollection<ShoppingListItem>; managed internally)
   - Finished (computed property: `true` if all items are purchased OR removed)
   - Factory Method: `ShoppingList.Create(string owner, DateTime? date = null)` — enforces owner non-null/non-empty
   - Instance Methods:
     - `ShoppingListItem AddItem(string description, int? quantity = null)` — creates item, adds to collection, returns created item
     - `void RemoveItem(Guid itemId)` — finds item, marks removed=true, recalculates finished
     - `void MarkItemPurchased(Guid itemId)` — finds item, marks purchased=true, recalculates finished
     - `void UpdateItemQuantity(Guid itemId, int? newQuantity)` — validates quantity (> 0 if not null) and updates
     - `ShoppingList Copy(string newOwner, DateTime? newDate = null)` — creates new list with copied items (items reset to purchased=false, removed=false)

2. **ShoppingListItem (Value Object or nested entity)**
   - Unique ID (sequential GUID; generated in constructor)
   - Description (string, 1-255 chars; enforced in constructor)
   - Quantity (int?, positive only when specified; enforced in constructor via method)
   - Purchased (bool, default false)
   - Removed (bool, default false)
   - No public default constructor
   - Factory: `ShoppingListItem.Create(string description, int? quantity = null)` — validates description, quantity
   - Instance Methods:
     - `void MarkPurchased()` → purchased = true
     - `void MarkRemoved()` → removed = true
     - `void UpdateQuantity(int? newQuantity)` → validates quantity
   - Helper: `bool IsCompleted()` → purchased || removed

**Validation Philosophy**:
- Constructor validation via exceptions (ArgumentException, DomainException) for invariant violations
- No validation layer in Domain; validation is enforcement
- Example: `ShoppingListItem.Create("", null)` throws immediately; not deferred to validator
- Quantity validation: If quantity is provided (not null), must be > 0; throwing in setter/method

### Application Layer Strategy

**CQRS Structure** (handlers via WolverineFx):

1. **Commands & Handlers** (6 total):
   - `CreateListCommand(string owner, DateTime? date)` → `CreateListHandler` → calls `ShoppingList.Create()`, returns ID
   - `AddItemCommand(Guid listId, string description, int? quantity)` → `AddItemHandler` → loads list, calls `list.AddItem()`, saves
   - `MarkItemPurchasedCommand(Guid listId, Guid itemId)` → `MarkItemPurchasedHandler` → loads list, calls `list. MarkItemPurchased(itemId)`, saves
   - `RemoveItemCommand(Guid listId, Guid itemId)` → `RemoveItemHandler` → loads list, calls `list.RemoveItem(itemId)`, saves
   - `UpdateItemQuantityCommand(Guid listId, Guid itemId, int? newQuantity)` → `UpdateItemQuantityHandler` → loads list, calls `list.UpdateItemQuantity(itemId, newQuantity)`, saves
   - `CopyListCommand(Guid sourceListId, string newOwner, DateTime? newDate)` → `CopyListHandler` → loads source list, calls `list.Copy(newOwner, newDate)`, saves new list

2. **Validation Strategy**:
   - **No FluentValidation for domain invariants** (owner non-empty, quantity > 0, description 1-255)
   - **Minimal boundary validation** (if any): Check for null/empty strings at handler entry (can delegate to domain constructor which throws)
   - **Example**: `AddItemHandler` receives description string; immediately calls `ShoppingListItem.Create(description, quantity)` which validates; if invalid, constructor throws and handler propagates (Application layer doesn't catch)
   - **Justification**: Domain exceptions (e.g., ArgumentException) bubble to RestApi layer (future) where they map to 400 Bad Request; no need for FluentValidation validators

3. **Repositories** (interface defined in Application, implementation in Infrastructure.Db):
   - `IShoppingListRepository.GetByIdAsync(Guid id)` → returns ShoppingList or null
   - `IShoppingListRepository.SaveAsync(ShoppingList list)` → persists (insert or update)

4. **Transaction Handling**:
   - Handlers are transactional (WolverineFx or explicit UnitOfWork pattern); each command is ACID

### Testing Strategy

**Domain Layer Tests** (ShoppingList.Domain.Tests):
- Unit tests for ShoppingList factory and methods
- Unit tests for ShoppingListItem factory and state transitions
- Test invariants: constructor exceptions, finished status calculation, item state transitions
- Example: `[Fact] public void Create_WithEmptyOwner_ThrowsArgumentException() { Assert.Throws<ArgumentException>(() => ShoppingList.Create("")); }`
- Example: `[Fact] public void Finished_AllItemsPurchased_ReturnsTrue() { ... }`

**Application Layer Tests** (ShoppingList.Application.Tests):
- Unit tests for handlers using mocked repositories
- Test command execution, state changes, return values
- Example: `[Fact] public async Task AddItemHandler_ValidItem_AddsToList() { ... }`

**Infrastructure Integration Tests** (ShoppingList.Infrastructure.Db.Tests):
- TestContainers PostgreSQL; test EF Core mapping and persistence
- Test round-trip: create list in domain, persist, reload, verify state

## Complexity Tracking

| Decision | Why | Alternative Rejected |
|----------|-----|----------------------|
| Domain-first validation (no FluentValidation) | Keeps invariants in Domain; simpler, more maintainable | Dual validation (domain + FluentValidation) causes duplication and confusion |
| Sequential GUID for ShoppingListItem ID | Preserves insertion order in DB; better performance | UUID (random GUID) loses natural ordering |
| Finished calculated on-demand | Simplifies state management; no stale cached state | Persisted finished flag requires invalidation logic |
| No API layer in this iteration | Scope: focus on core domain; API adds new concerns | Complete full stack introduces untestable coupling |

### Infrastructure Layer Strategy

**Persistence Implementation** (using Ardalis.Specification & EF Core):

1. **Generic Repository Interface** (`Infrastructure.Db`)
   - `IGenericRepository<T>` (base contract)
     - `Task<T?> GetByIdAsync(Guid id)` — single aggregate by ID
     - `Task<List<T>> ListAsync(ISpecification<T> spec)` — query via specification
     - `Task<int> CountAsync(ISpecification<T> spec)` — count matching spec
     - `Task<T> AddAsync(T entity)` — insert and return
     - `Task UpdateAsync(T entity)` — update
     - `Task DeleteAsync(T entity)` — delete
   - `IReadRepository<T>` (query-only subset)
     - `Task<T?> GetByIdAsync(Guid id)`
     - `Task<List<T>> ListAsync(ISpecification<T> spec)`
     - `Task<int> CountAsync(ISpecification<T> spec)`

2. **ShoppingList Specifications** (Ardalis.Specification queries)
   - `GetShoppingListByIdSpec` — load list with items (includes)
   - `GetShoppingListsByOwnerSpec(string owner)` — paginated owner's lists
   - `GetShoppingListsFinishedSpec` — filter finished/active
   - Specifications live in `Application/Specifications/` (query logic near use cases)

3. **EF Core Repository Implementation**
   - `ShoppingListRepository : IGenericRepository<ShoppingList>, IShoppingListRepository`
   - Uses `ISpecification<ShoppingList>` for dynamic query building
   - DbContext injected; transactions handled via SaveChangesAsync
   - Lives in `Infrastructure.Db/Repositories/`

4. **EF Core DbContext & Mappings**
   - `ShoppingListDbContext` with DbSets for ShoppingList (items stored as owned collection)
   - Entity configurations in `Infrastructure.Db/Configurations/`
   - Migrations stored in `Infrastructure.Db/Migrations/`

5. **Transaction Scopes** (via WolverineFx or explicit UnitOfWork)
   - Handlers use [TransactionScope] or explicit DbContext SaveChangesAsync within transaction
   - Ensures ACID compliance for all aggregate mutations

**Persistence Deferred Notes**:
- EF Core implementation (DbContext, migrations) is NOT included in this Domain + Application iteration
- Infrastructure.Db.Tests use TestContainers (PostgreSQL) for integration validation
- Implementation will be added in a follow-up iteration
