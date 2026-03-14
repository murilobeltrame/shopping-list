# Implementation Tasks: Shopping List Domain Model & Core Use Cases

**Feature**: 001-shopping-list-domain  
**Created**: 2026-03-01  
**Branch**: `001-shopping-list-domain`  
**Status**: Ready for Implementation  
**Reference**: [plan.md](plan.md) | [data-model.md](data-model.md) | [spec.md](spec.md)

---

## Overview

This document breaks down the Shopping List domain implementation into executable tasks following Test-First Development (Red-Green-Refactor). Tasks are organized by implementation phase with clear dependencies, file paths, and success criteria.

**Total Tasks**: 26 (current iteration; T001-T026) + 6 deferred (T027-T032 for Phase 5 persistence, out-of-scope)  
**Estimated Duration (Active)**: 2-3 days (5-8 hours, current iteration only)  
**Estimated Duration (Full with Phase 5)**: 3-4 days additional (persistence layer TBD in future iteration)

---

## Phase 1: Setup & Foundation (Non-Parallelizable)

### 1.1 Domain Layer Project Structure

- [X] T001 Create Domain layer folder structure in `src/ShoppingList.Domain/`
  - Create subdirectories: `Entities/`, `Exceptions/`, `Specifications/`
  - File: `src/ShoppingList.Domain/ShoppingList.Domain.csproj` (no new dependencies needed)

- [X] T002 Create custom domain exception class in `src/ShoppingList.Domain/Exceptions/`
  - Create: `src/ShoppingList.Domain/Exceptions/DomainException.cs` (base exception for domain violations)
  - Implements: `Exception` with message + optional inner exception
  - Usage: Thrown by ShoppingListItem when invariants violated (e.g., invalid quantity)

### 1.2 Application Layer Project Structure

- [X] T003 Create Application layer folder structure in `src/ShoppingList.Application/`
  - Create subdirectories: `Commands/`, `Handlers/`, `Repositories/`, `Services/`
  - File: `src/ShoppingList.Application/ShoppingList.Application.csproj`
  - Verify: ProjectReference to ShoppingList.Domain exists

### 1.3 Test Projects

- [X] T004 Verify Domain.Tests project structure in `test/ShoppingList.Domain.Tests/`
  - Folders: `Entities/`, `Helpers/`
  - Verify references: Shouldly, AutoBogus, xUnit
  - File: `test/ShoppingList.Domain.Tests/ShoppingList.Domain.Tests.csproj`

- [X] T005 Verify Application.Tests project structure in `test/ShoppingList.Application.Tests/`
  - Folders: `Handlers/`, `Mocks/`
  - Verify references: Shouldly, AutoBogus, xUnit, Moq (for mocking repositories)
  - File: `test/ShoppingList.Application.Tests/ShoppingList.Application.Tests.csproj`

---

## Phase 2: Core Domain Entities (Parallelizable: T006 & T007 can be done together)

### 2.1 ShoppingListItem Entity Implementation

- [X] T006 [P] ShoppingListItem unit tests (RED phase)
  - Create: `test/ShoppingList.Domain.Tests/Entities/ShoppingListItemTests.cs`
  - Tests to write (all should FAIL initially):
    - `Constructor_WithValidDescription_CreatesItem` — factory creates item
    - `Constructor_WithEmptyDescription_ThrowsArgumentException` — empty string rejected
    - `Constructor_WithWhitespaceDescription_ThrowsArgumentException` — whitespace-only rejected
    - `Constructor_WithDescriptionExceeding255_ThrowsArgumentException` — length > 255 rejected
    - `Constructor_WithValidQuantity_CreatesItem` — positive int accepted
    - `Constructor_WithZeroQuantity_ThrowsArgumentException` — 0 rejected
    - `Constructor_WithNegativeQuantity_ThrowsArgumentException` — negative rejected
    - `Constructor_WithNullQuantity_CreatesItemSuccessfully` — null is valid
    - `MarkPurchased_SetsFlag_ToTrue` — purchased becomes true
    - `MarkRemoved_SetsFlag_ToTrue` — removed becomes true
    - `MarkPurchased_DoesNotAffect_RemovedFlag` — removed stays false when purchasing
    - `MarkRemoved_DoesNotAffect_PurchasedFlag` — purchased stays false when removing
    - `UpdateQuantity_WithValidValue_Updates` — quantity changes
    - `UpdateQuantity_WithZero_ThrowsArgumentException` — 0 rejected
    - `UpdateQuantity_WithNull_UpdatesToNull` — null accepted
    - `IsCompleted_WhenPurchased_ReturnsTrue` — purchased or removed = completed
    - `IsCompleted_WhenRemoved_ReturnsTrue` — purchased or removed = completed
    - `IsCompleted_WhenNeither_ReturnsFalse` — not purchased and not removed = not completed
    - Reference: [data-model.md](data-model.md#shoppinglistitem-entity)

- [X] T007 [P] ShoppingListItem entity implementation (GREEN phase)
  - Create: `src/ShoppingList.Domain/Entities/ShoppingListItem.cs`
  - Implement:
    - Public properties: `Guid Id`, `string Description`, `int? Quantity` (private setter), `bool Purchased` (private setter), `bool Removed` (private setter)
    - No public default constructor
    - Factory method: `public static ShoppingListItem Create(string description, int? quantity = null)`
    - Validation in factory:
      - Owner: `throw new ArgumentException("Description cannot be null, empty, or exceed 255 characters")`  if invalid
      - Quantity: `throw new ArgumentException("Quantity must be positive (> 0) or null")` if qty ≤ 0
    - Instance methods: `MarkPurchased()`, `MarkRemoved()`, `UpdateQuantity(int? newQuantity)`, `IsCompleted()`
    - Private constructor: `private ShoppingListItem(Guid id, string description, int? quantity, bool purchased, bool removed)`
  - Initialization: `Purchased = false`, `Removed = false`, `Id = Guid.NewGuid()` (or sequential GUID in Factory)
  - Style: Primary constructor or parameterized constructor; explicit types (no `var`)
  - Reference: [data-model.md](data-model.md#shoppinglistitem-entity)

- [X] T008 ShoppingListItem refactor & code review (REFACTOR phase)
  - Review: Invariants enforced, no public mutation, clean code
  - Verify: All tests PASS
  - Optimize: Remove redundant validation, simplify methods

### 2.2 ShoppingList Aggregate Implementation

- [X] T009 [P] ShoppingList unit tests (RED phase)
  - Create: `test/ShoppingList.Domain.Tests/Entities/ShoppingListTests.cs`
  - Tests to write (all should FAIL initially):
    - `Create_WithValidOwner_CreatesList` — list created successfully
    - `Create_WithEmptyOwner_ThrowsArgumentException` — empty owner rejected
    - `Create_WithNullOwner_ThrowsArgumentException` — null owner rejected
    - `Create_WithWhitespaceOwner_ThrowsArgumentException` — whitespace-only rejected
    - `Create_WithDateAndOwner_CreatesListWithDate` — date is optional
    - `Create_InitializesWithEmptyItems_AndFinishedFalse` — new list has 0 items, finished=false
    - `AddItem_WithValidDescription_AddsItemToList` — item added, returned
    - `AddItem_DelegatesValidationToShoppingListItem` — invalid description throws (from item factory)
    - `AddItem_InvalidQuantity_ThrowsArgumentException` — invalid qty rejected
    - `Finished_WithEmptyList_ReturnsFalse` — no items = not finished
    - `Finished_WithOnePurchasedItem_ReturnsTrue` — all items completed = finished
    - `Finished_WithOneRemovedItem_ReturnsTrue` — all items completed = finished
    - `Finished_WithMixedStates_ReturnsFalse` — some incomplete = not finished
    - `Finished_WithAllItemsCompleted_ReturnsTrue` — all items purchased or removed = finished
    - `RemoveItem_WithValidItemId_MarksItemRemoved` — item marked removed
    - `RemoveItem_WithInvalidItemId_ThrowsInvalidOperationException` — not found throws
    - `RemoveItem_RecalculatesFinished_ToTrueIfAllCompleted` — finished recalculated
    - `MarkItemPurchased_WithValidItemId_MarksPurchased` — item marked purchased
    - `MarkItemPurchased_WithInvalidItemId_ThrowsInvalidOperationException` — not found throws
    - `MarkItemPurchased_RecalculatesFinished_ToTrueIfAllCompleted` — finished recalculated
    - `UpdateItemQuantity_WithValidItemId_UpdatesQuantity` — quantity changed
    - `UpdateItemQuantity_WithInvalidItemId_ThrowsInvalidOperationException` — not found throws
    - `UpdateItemQuantity_DoesNotAffectFinished_Status` — finished unchanged when qty changes
    - `Copy_WithValidOwner_CreatesNewListWithCopiedItems` — new list created
    - `Copy_WithNewOwner_CopiedListHasNewOwner` — owner changed
    - `Copy_CopiedItems_ResetToNotPurchasedAndNotRemoved` — items reset
    - `Copy_OriginalListRemains_Unchanged` — original not modified
  - Reference: [data-model.md](data-model.md#aggregate-shoppinglist)

- [X] T010 [P] ShoppingList aggregate implementation (GREEN phase)
  - Create: `src/ShoppingList.Domain/Entities/ShoppingList.cs`
  - Implement:
    - Public properties: `Guid Id`, `string Owner`, `DateTime? Date`, `IReadOnlyCollection<ShoppingListItem> Items`
    - Computed property: `public bool Finished => Items.Count == 0 ? false : Items.All(i => i.Purchased || i.Removed);`
    - No public default constructor
    - Factory method: `public static ShoppingList Create(string owner, DateTime? date = null)`
      - Validation: `throw new ArgumentException("Owner cannot be null or empty")` if owner invalid
      - Initialization: `Owner = owner`, `Date = date`, `Items = new List<ShoppingListItem>()`
    - Instance methods:
      - `ShoppingListItem AddItem(string description, int? quantity = null)` — calls `ShoppingListItem.Create()`, adds to items, returns item
      - `void RemoveItem(Guid itemId)` — finds item, calls `MarkRemoved()`, or throws InvalidOperationException
      - `void MarkItemPurchased(Guid itemId)` — finds item, calls `MarkPurchased()`, or throws InvalidOperationException
      - `void UpdateItemQuantity(Guid itemId, int? newQuantity)` — finds item, calls `UpdateQuantity()`, or throws InvalidOperationException
      - `ShoppingList Copy(string newOwner, DateTime? newDate = null)` — creates new list via `Create()`, copies items with state reset
    - Private constructor: `private ShoppingList(Guid id, string owner, DateTime? date)`
    - Internal collection: `private List<ShoppingListItem> _items` (with public getter as IReadOnlyCollection)
  - Style: Primary constructor or parameterized constructor; explicit types (no `var`)
  - Reference: [data-model.md](data-model.md#aggregate-shoppinglist)

- [X] T011 ShoppingList refactor & code review (REFACTOR phase)
  - Review: Invariants enforced, finished calculation correct, collection managed properly
  - Verify: All tests PASS
  - Optimize: Remove redundant code, simplify item lookup logic

---

## Phase 3: Application Layer - Commands & Handlers (Parallelizable by handler)

### 3.1 Repository Interface & Commands

- [X] T012 Create repository interface in Application layer
  - Create: `src/ShoppingList.Application/Repositories/IShoppingListRepository.cs`
  - Interface methods:
    - `Task<ShoppingList?> GetByIdAsync(Guid id)`
    - `Task SaveAsync(ShoppingList list)` (insert or update)
    - `Task DeleteAsync(Guid id)` (optional, not used in MVP)
  - Namespace: `ShoppingList.Application.Repositories`

- [X] T013 Create command classes in Application layer
  - Create: `src/ShoppingList.Application/Commands/CreateListCommand.cs`
    - Properties: `string Owner`, `DateTime? Date`
    - No validation here (delegate to domain)
  - Create: `src/ShoppingList.Application/Commands/AddItemCommand.cs`
    - Properties: `Guid ListId`, `string Description`, `int? Quantity`
  - Create: `src/ShoppingList.Application/Commands/MarkItemPurchasedCommand.cs`
    - Properties: `Guid ListId`, `Guid ItemId`
  - Create: `src/ShoppingList.Application/Commands/RemoveItemCommand.cs`
    - Properties: `Guid ListId`, `Guid ItemId`
  - Create: `src/ShoppingList.Application/Commands/UpdateItemQuantityCommand.cs`
    - Properties: `Guid ListId`, `Guid ItemId`, `int? NewQuantity`
  - Create: `src/ShoppingList.Application/Commands/CopyListCommand.cs`
    - Properties: `Guid SourceListId`, `string NewOwner`, `DateTime? NewDate`

### 3.2 Command Handlers (Parallelizable)

- [X] T014 [P] CreateListHandler tests (RED phase)
  - Create: `test/ShoppingList.Application.Tests/Handlers/CreateListHandlerTests.cs`
  - Mock: `IShoppingListRepository`
  - Tests:
    - `Handle_WithValidCommand_CreatesListAndReturnsId` — handler creates list, saves, returns ID
    - `Handle_WithInvalidOwner_PropagatesArgumentException` — domain exception bubbles
    - `Handle_WithDate_CreatesListWithDate` — date passed through
  - Reference: [plan.md](plan.md#application-layer-strategy)

- [X] T015 [P] CreateListHandler implementation (GREEN phase)
  - Create: `src/ShoppingList.Application/Handlers/CreateListHandler.cs`
  - Implements: `ICommandHandler<CreateListCommand, Guid>` (WolverineFx interface)
  - Method: `async Task<Guid> Handle(CreateListCommand command)`
    - Call: `ShoppingList.Create(command.Owner, command.Date)` — may throw ArgumentException
    - Call: `await repository.SaveAsync(list)` — persist
    - Return: `list.Id`
  - Transactions: Handled by WolverineFx (marked with transactional attribute or explicit scope)

- [X] T016 [P] AddItemHandler tests (RED phase)
  - Create: `test/ShoppingList.Application.Tests/Handlers/AddItemHandlerTests.cs`
  - Mock: `IShoppingListRepository`
  - Tests:
    - `Handle_WithValidCommand_AddsItemAndReturnsItemId` — handler adds item, saves, returns item ID
    - `Handle_WithInvalidDescription_PropagatesArgumentException` — domain exception bubbles
    - `Handle_WithListNotFound_ThrowsInvalidOperationException` — repository returns null
    - `Handle_UpdatesFinishedStatusCorrectly` — finished recalculated after add

- [X] T017 [P] AddItemHandler implementation (GREEN phase)
  - Create: `src/ShoppingList.Application/Handlers/AddItemHandler.cs`
  - Implements: `ICommandHandler<AddItemCommand, Guid>`
  - Method: `async Task<Guid> Handle(AddItemCommand command)`
    - Call: `await repository.GetByIdAsync(command.ListId)` — load list
    - Throw: `InvalidOperationException("List not found")` if null
    - Call: `list.AddItem(command.Description, command.Quantity)` — may throw ArgumentException
    - Call: `await repository.SaveAsync(list)` — persist
    - Return: `item.Id` from AddItem return value

- [X] T018 [P] MarkItemPurchasedHandler tests (RED phase)
  - Create: `test/ShoppingList.Application.Tests/Handlers/MarkItemPurchasedHandlerTests.cs`
  - Tests:
    - `Handle_WithValidCommand_MarksPurchased` — item marked, list saved
    - `Handle_WithItemNotFound_PropagatesInvalidOperationException` — domain exception bubbles
    - `Handle_RecalculatesFinished` — finished updated if all items completed
    - `Handle_WithListNotFound_ThrowsInvalidOperationException` — repository returns null

- [X] T019 [P] MarkItemPurchasedHandler implementation (GREEN phase)
  - Create: `src/ShoppingList.Application/Handlers/MarkItemPurchasedHandler.cs`
  - Implements: `ICommandHandler<MarkItemPurchasedCommand>`
  - Method: `async Task Handle(MarkItemPurchasedCommand command)`
    - Load list, throw if not found
    - Call: `list.MarkItemPurchased(command.ItemId)` — may throw InvalidOperationException
    - Call: `await repository.SaveAsync(list)`

- [X] T020 [P] RemoveItemHandler tests (RED phase)
  - Create: `test/ShoppingList.Application.Tests/Handlers/RemoveItemHandlerTests.cs`
  - Tests:
    - `Handle_WithValidCommand_MarksRemoved` — item marked, list saved
    - `Handle_WithItemNotFound_PropagatesInvalidOperationException` — domain exception bubbles
    - `Handle_RecalculatesFinished` — finished updated if all items completed
    - `Handle_WithListNotFound_ThrowsInvalidOperationException` — repository returns null

- [X] T021 [P] RemoveItemHandler implementation (GREEN phase)
  - Create: `src/ShoppingList.Application/Handlers/RemoveItemHandler.cs`
  - Implements: `ICommandHandler<RemoveItemCommand>`
  - Method: `async Task Handle(RemoveItemCommand command)`
    - Load list, throw if not found
    - Call: `list.RemoveItem(command.ItemId)` — may throw InvalidOperationException
    - Call: `await repository.SaveAsync(list)`

- [X] T022 [P] UpdateItemQuantityHandler tests (RED phase)
  - Create: `test/ShoppingList.Application.Tests/Handlers/UpdateItemQuantityHandlerTests.cs`
  - Tests:
    - `Handle_WithValidCommand_UpdatesQuantity` — quantity changed, list saved
    - `Handle_WithInvalidQuantity_PropagatesArgumentException` — domain exception bubbles
    - `Handle_WithItemNotFound_PropagatesInvalidOperationException` — domain exception bubbles
    - `Handle_DoesNotAffectFinished` — finished status unchanged after quantity update

- [X] T023 [P] UpdateItemQuantityHandler implementation (GREEN phase)
  - Create: `src/ShoppingList.Application/Handlers/UpdateItemQuantityHandler.cs`
  - Implements: `ICommandHandler<UpdateItemQuantityCommand>`
  - Method: `async Task Handle(UpdateItemQuantityCommand command)`
    - Load list, throw if not found
    - Call: `list.UpdateItemQuantity(command.ItemId, command.NewQuantity)` — may throw exceptions
    - Call: `await repository.SaveAsync(list)`

- [X] T024 [P] CopyListHandler tests (RED phase)
  - Create: `test/ShoppingList.Application.Tests/Handlers/CopyListHandlerTests.cs`
  - Tests:
    - `Handle_WithValidCommand_CopiesListAndReturnsNewId` — new list created, saved, ID returned
    - `Handle_CopiedItemsResetToNotCompleted` — items have purchased=false, removed=false
    - `Handle_SourceListNotFound_ThrowsInvalidOperationException` — repository returns null for source
    - `Handle_WithInvalidNewOwner_PropagatesArgumentException` — domain exception bubbles

- [X] T025 [P] CopyListHandler implementation (GREEN phase)
  - Create: `src/ShoppingList.Application/Handlers/CopyListHandler.cs`
  - Implements: `ICommandHandler<CopyListCommand, Guid>`
  - Method: `async Task<Guid> Handle(CopyListCommand command)`
    - Load source list, throw if not found
    - Call: `sourceList.Copy(command.NewOwner, command.NewDate)` — may throw ArgumentException
    - Call: `await repository.SaveAsync(newList)` — persist new list
    - Return: `newList.Id`

---

## Phase 4: Integration & Polish

- [X] T026 Build & verify all layers compile
  - Run: `dotnet build` from repo root
  - Verify: No compilation errors in Domain, Application, Domain.Tests, Application.Tests
  - Check: All test projects can be built

---

## Phase 5: Persistence Layer - Repository & Query Specifications (Out of Scope - Listed for Clarity)

**Note**: This phase is documented for architectural completeness but is explicitly OUT OF SCOPE for the current 001-shopping-list-domain iteration. Implementation deferred to Infrastructure.Db follow-up work. Listed here for continuity planning.

### 5.1 Generic Repository Abstraction

- [ ] T027 [Future] Create generic repository interface with Ardalis.Specification
  - Create: `src/ShoppingList.Application/Repositories/IGenericRepository.cs`
  - Interface: `IGenericRepository<T> where T : class`
  - Methods:
    - `Task<T?> GetByIdAsync(Guid id)` — load aggregate by ID
    - `Task<List<T>> ListAsync(ISpecification<T> spec)` — query via specification
    - `Task<int> CountAsync(ISpecification<T> spec)` — count matching spec
    - `Task<T> AddAsync(T entity)` — insert
    - `Task UpdateAsync(T entity)` — update
    - `Task DeleteAsync(T entity)` — delete
  - Inheritance: `IShoppingListRepository : IGenericRepository<ShoppingList>`

### 5.2 Ardalis.Specification Query Specifications

- [ ] T028 [Future] Create ShoppingList query specifications in Application layer
  - Create: `src/ShoppingList.Application/Specifications/ShoppingListSpecifications.cs`
  - Specifications:
    - `GetShoppingListByIdWithItemsSpec(Guid id)` — load list with items included
    - `GetShoppingListsByOwnerSpec(string owner, int pageNumber = 1, int pageSize = 10)` — paginated lists
    - `GetFinishedListsSpec(string owner)` — filter finished = true
    - `GetActiveListsSpec(string owner)` — filter finished = false
  - Pattern: Each spec extends `BaseSpecification<ShoppingList>` with proper `.Include()` chains for eager loading

### 5.3 EF Core Repository Implementation

- [ ] T029 [Future] Create EF Core ShoppingListRepository in Infrastructure.Db layer
  - Create: `src/ShoppingList.Infrastructure.Db/Repositories/ShoppingListRepository.cs`
  - Implements: `IGenericRepository<ShoppingList>, IShoppingListRepository`
  - Constructor: Injects `ShoppingListDbContext`
  - Implementation: Use `RepositoryBase<T>` from Ardalis.Specification for standard CRUD; override `SaveAsync()` for transaction semantics
  - Transaction handling: Each Save wrapped in DbContext.SaveChangesAsync that commits transaction scope

- [ ] T030 [Future] Create EF Core DbContext with entity configurations
  - Create: `src/ShoppingList.Infrastructure.Db/ShoppingListDbContext.cs`
  - DbSets: `DbSet<ShoppingList>` (ShoppingListItem stored as owned collection via HasMany/WithOwner)
  - Migrations folder: `src/ShoppingList.Infrastructure.Db/Migrations/`
  - Initial migration: Create ShoppingLists and ShoppingListItems tables

### 5.4 Integration Tests with TestContainers

- [ ] T031 [Future] Create Infrastructure.Db integration tests with PostgreSQL TestContainers
  - Create: `test/ShoppingList.Infrastructure.Db.Tests/Repositories/ShoppingListRepositoryTests.cs`
  - TestContainers fixture: PostgreSQL container lifecycle management
  - Tests:
    - Round-trip: Create list in Domain, persist via repository, reload, verify state
    - Specification queries: Test GetByIdWithItems, GetByOwner, GetFinished specs
    - Transaction rollback: Verify ACID semantics
  - EF Core migrations: Applied to test container before each test session

### 5.5 Configuration & Dependency Injection

- [ ] T032 [Future] Wire persistence layer into Application/RestApi via DI
  - Create: `src/ShoppingList.Infrastructure.Db/ServiceCollectionExtensions.cs`
  - Extension method: `services.AddShoppingListPersistence(connectionString)` 
  - Registers: DbContext, IGenericRepository<T>, IShoppingListRepository
  - Configuration: EF Core connection string, lazy-load strategy, query tracking behavior

---

## Task Dependency Graph

```
Phase 1 (Setup - must complete first):
  T001 → T002 → T003 → T004 → T005
  
Phase 2 (Domain - T006/T007 parallel, then T009/T010 parallel):
  ┌─ T006 (tests) ────┐
  │                      ├─ T008 (review)
  └─ T007 (code) ────┘
  
  ┌─ T009 (tests) ────┐
  │                      ├─ T011 (review)
  └─ T010 (code) ────┘

Phase 3 (Application - T012/T013 setup, then handlers parallel):
  T012 → T013 ──────┐
                      ├─→ T014-T025 (all handlers, parallelizable)
                      
Phase 4 (Final - current iteration):
  All prior → T026

Phase 5 (Persistence - DEFERRED, out-of-scope for this iteration):
  (T027-T032 listed for architectural planning; not part of current implementation)
```

---

## Success Criteria Per Phase

### Phase 1: Setup
- ✅ All project folders created with correct structure
- ✅ Projects compile (no missing references)
- ✅ Test projects have required dependencies (xUnit, Shouldly, AutoBogus, Moq)

### Phase 2: Domain
- ✅ All ShoppingListItem tests PASS
- ✅ All ShoppingList tests PASS
- ✅ ShoppingListItem entity enforces all invariants
- ✅ ShoppingList aggregate manages collection and calculated finished status
- ✅ No public default constructors on either entity
- ✅ Domain layer has zero external dependencies (no EF Core, no WolverineFx, no FluentValidation)

### Phase 3: Application
- ✅ All handler tests PASS with mocked repositories
- ✅ Each handler correctly loads aggregate, calls domain methods, and persists
- ✅ Domain exceptions properly bubble (not caught/suppressed)
- ✅ Commands are simple DTOs with no logic
- ✅ Repository interface correctly defined for persistence concern (to be implemented in Infrastructure.Db)

### Phase 4: Build
- ✅ Full solution compiles with zero errors
- ✅ `dotnet test` passes all tests in Domain.Tests and Application.Tests
- ✅ No warnings or static analysis issues

### Phase 5: Persistence (DEFERRED)
- ⚠ OUT OF SCOPE: Infrastructure.Db layer listed for future iteration planning
- Tasks T027-T032 document persistence architecture but are NOT executed in current iteration
- Mocked repositories sufficient for Application.Tests in this phase

---

## Testing Approach

**Red-Green-Refactor Cycle**:
1. **RED**: Write test that fails (test file references code that doesn't exist or fails assertion)
2. **GREEN**: Write minimal code to make test pass (no refactoring, just pass the test)
3. **REFACTOR**: Clean up, optimize, ensure all tests still pass

**Example Cycle** (ShoppingListItem.Create):
1. RED: `[Fact] public void Create_WithEmptyDescription_ThrowsArgumentException() { Assert.Throws<ArgumentException>(() => ShoppingListItem.Create("")); }`
2. GREEN: Implement factory to throw if description empty
3. REFACTOR: Consolidate validation logic, improve error message

**Assertion Style**: Shouldly chainable syntax
```csharp
list.Finished.ShouldBeTrue();
list.Owner.ShouldBe("user@example.com");
item.IsCompleted().ShouldBeFalse();
```

---

## File Path Reference

| Task | File Path |
|------|-----------|
| T002 | `src/ShoppingList.Domain/Exceptions/DomainException.cs` |
| T007 | `src/ShoppingList.Domain/Entities/ShoppingListItem.cs` |
| T010 | `src/ShoppingList.Domain/Entities/ShoppingList.cs` |
| T012 | `src/ShoppingList.Application/Repositories/IShoppingListRepository.cs` |
| T013 | `src/ShoppingList.Application/Commands/*.cs` (6 files) |
| T006, T009 | `test/ShoppingList.Domain.Tests/Entities/*Tests.cs` |
| T014+, T016+, etc | `test/ShoppingList.Application.Tests/Handlers/*HandlerTests.cs` (6 files) |
| T015, T017, etc | `src/ShoppingList.Application/Handlers/*Handler.cs` (6 files) |

---

## Estimated Time Breakdown

| Phase | Tasks | Estimated Time | Notes |
|-------|-------|-----------------|--------|
| Phase 1 | T001-T005 | 15-30 min | Setup only; mostly file creation |
| Phase 2 | T006-T011 | 2-3 hours | Core domain; thorough testing |
| Phase 3 | T012-T025 | 3-4 hours | 6 handlers; parallelizable |
| Phase 4 | T026 | 10-15 min | Build & verify |
| **Total (Current)** | **T001-T026** | **5-8 hours** | **Full feature implementation (Phases 1-4)** |
| Phase 5 [Deferred] | T027-T032 | 2-3 hours | Persistence layer (future iteration) |
| **Total (With Persistence)** | **T001-T032** | **7-11 hours** | **Including deferred persistence layer** |

---

## Notes for Implementer

1. **Domain Comes First**: Implement T006-T011 before any Application layer (T012+). Domain is the source of truth.
2. **Test-Driven**: Write ALL tests first (RED), then implement code (GREEN), then refactor (REFACTOR).
3. **No Validation Validators**: Domain layer throws exceptions directly; no FluentValidation classes.
4. **Repository Mocking**: Use Moq for Application layer tests (mock IShoppingListRepository); real EF Core implementation deferred to Infrastructure.Db.Tests.
5. **Parallel Work**: Phase 2 tasks T006-T007 can be done in parallel by different devs (different entities). Same for Phase 3 (different handlers).
6. **No API Layer**: RestApi endpoints are out of scope for this iteration; Application layer handlers are pure orchestration.

---

## Ready to Implement

All tasks are clearly defined with:
- ✅ Test-first approach (tests defined before code)
- ✅ Clear file paths and namespaces
- ✅ Expected method signatures and behaviors
- ✅ Validation rules and exception types
- ✅ Success criteria and acceptance tests
- ✅ Dependencies and ordering

**Recommended Start**: `dotnet test` on Domain.Tests/ShoppingListItemTests.cs to trigger RED phase (tests will fail until entity implemented).
