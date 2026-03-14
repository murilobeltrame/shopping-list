# REST API Contract: Automatic ID Generation

## Create Shopping List

- Method: `POST`
- Route: `/shopping-lists`
- Request body:
```json
{
  "owner": "alice",
  "date": "2026-03-13T00:00:00Z"
}
```
- Response: `201 Created`
```json
{
  "id": "c2a0d67c-42d5-4c95-a02a-af9c655d3e44"
}
```
- Contract notes:
  - Request does not accept `id`.
  - Response always includes generated non-empty `id`.

## Add Shopping List Item

- Method: `POST`
- Route: `/shopping-lists/{listId}/items`
- Request body:
```json
{
  "description": "Milk",
  "quantity": 2
}
```
- Response: `201 Created`
```json
{
  "id": "7cf96ba8-0915-40fd-b4cf-05862f2f3400"
}
```
- Contract notes:
  - Request does not accept item `id`.
  - Generated item `id` is returned.

## Copy Shopping List

- Method: `POST`
- Route: `/shopping-lists/{listId}/copy`
- Request body:
```json
{
  "newOwner": "bob",
  "newDate": null
}
```
- Response: `201 Created`
```json
{
  "id": "d3b4d7c5-4f4a-4d36-9f95-3f0bbf7642e8"
}
```
- Contract notes:
  - Returned `id` must differ from source list `id`.
  - Copied items are recreated with new generated IDs.

## Non-create Operations (Unchanged)

- `POST /shopping-lists/{listId}/items/{itemId}/purchase` -> `204 No Content`
- `DELETE /shopping-lists/{listId}/items/{itemId}` -> `204 No Content`
- `PUT /shopping-lists/{listId}/items/{itemId}/quantity` -> `204 No Content`
- Existing operations continue to use persisted system-generated IDs.
