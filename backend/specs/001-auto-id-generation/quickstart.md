# Quickstart: Automatic ID Generation

## Prerequisites

- .NET SDK 10.0.100
- Local PostgreSQL reachable by Aspire configuration or `SHOPPINGLIST_DB_CONNECTION_STRING`

## 1. Build and test

```bash
dotnet build
dotnet test
```

## 2. Run API (Aspire)

```bash
dotnet run --project env/ShoppingList.AppHost
```

## 3. Validate create-list without ID

```bash
curl -i -X POST http://localhost:5000/shopping-lists \
  -H "Content-Type: application/json" \
  -d '{"owner":"alice","date":null}'
```

Expected:
- HTTP `201 Created`
- Response body includes non-empty generated `id`
- `Location` header points to `/shopping-lists/{id}`

## 4. Validate add-item without ID

```bash
curl -i -X POST http://localhost:5000/shopping-lists/{listId}/items \
  -H "Content-Type: application/json" \
  -d '{"description":"Milk","quantity":2}'
```

Expected:
- HTTP `201 Created`
- Response includes generated item `id`

## 5. Validate copy flow generates new IDs

```bash
curl -i -X POST http://localhost:5000/shopping-lists/{listId}/copy \
  -H "Content-Type: application/json" \
  -d '{"newOwner":"bob","newDate":null}'
```

Expected:
- HTTP `201 Created`
- Returned `id` differs from source list ID
- Copied items are recreated with new item IDs

## 6. Automated verification focus

- API tests in test/ShoppingList.RestApi.Tests/Endpoints verify create responses contain generated IDs.
- Application and Domain tests verify creation does not require caller-provided identity.
- Architecture tests verify dependency direction is preserved.
