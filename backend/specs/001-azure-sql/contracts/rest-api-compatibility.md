# Contract: REST API Compatibility During Azure SQL Migration

## Contract Type

Public HTTP API compatibility contract.

## Contract Statement

The Azure SQL migration MUST NOT change externally observable REST API contracts for existing shopping list endpoints.

## Endpoints In Scope

- `POST /shopping-lists`
- `POST /shopping-lists/{listId}/items`
- `POST /shopping-lists/{listId}/copy`
- `POST /shopping-lists/{listId}/items/{itemId}/purchase`
- `DELETE /shopping-lists/{listId}/items/{itemId}`
- `PUT /shopping-lists/{listId}/items/{itemId}/quantity`

## Compatibility Requirements

- Request shapes remain unchanged.
- Response shapes and status codes remain unchanged.
- Error semantics remain equivalent for validation and not-found scenarios.
- Id generation format remains unchanged (GUID values).
- Existing consumers require no code changes.

## Persistence Configuration Contract

- Runtime must obtain DB connection configuration from environment/app configuration paths used by current hosting model.
- Invalid or missing DB configuration must fail startup with explicit diagnostics.
- Pending migrations must be applied before serving requests.

## Verification

- Existing API tests continue to pass without test expectation rewrites.
- Integration tests validate persistence parity on SQL Server.

## Verification Results (2026-03-15)

- `test/ShoppingList.RestApi.Tests/Endpoints/ShoppingListCreationTests.cs` passed with no request/response contract changes.
- `test/ShoppingList.RestApi.Tests/Endpoints/ShoppingListMutationTests.cs` passed with unchanged status code expectations.
- Full suite execution (`dotnet test ShoppingList.sln`) passed with 81/81 tests successful.
