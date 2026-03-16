# Quickstart: Azure SQL Migration (Backend)

## Goal

Implement and verify backend migration from PostgreSQL to Azure SQL/SQL Server while preserving behavior.

## Prerequisites

- .NET SDK 10.0.100
- Docker runtime available for local SQL Server container usage
- From repository root or backend folder as noted

## 1. Write/Adjust Failing Tests First (Red)

- Update infrastructure integration test setup to target SQL Server container.
- Add/adjust migration smoke test to validate schema applies successfully on SQL Server.
- Ensure at least one provider-specific test fails before implementation updates.

Suggested commands:

```bash
cd backend

dotnet test test/ShoppingList.Infrastructure.Db.Tests
```

## 2. Implement Provider Migration (Green)

- Replace provider wiring from Npgsql/PostgreSQL to SQL Server in:
- infrastructure DbContext factory
- API composition root
- AppHost orchestration resource wiring
- Remove PostgreSQL-specific migration files and generate SQL Server migration(s).

Suggested commands:

```bash
cd backend

# Regenerate migrations for SQL Server
dotnet ef migrations add InitialShoppingListSchema \
	--project src/ShoppingList.Infrastructure.Db \
	--startup-project src/ShoppingList.Infrastructure.Db \
	--output-dir Migrations

dotnet build ShoppingList.sln
```

## 3. Validate Full Regression Suite

Run all tests to ensure no behavioral regressions:

```bash
cd backend

dotnet test ShoppingList.sln
```

## 4. Run the App Locally with Aspire

```bash
cd backend

dotnet run --project env/ShoppingList.AppHost
```

Validate:

- SQL Server container starts with API.
- API endpoints perform full list/item lifecycle correctly.
- Startup fails clearly when connection settings are intentionally invalid.

## 5. Optional: Inspect Local SQL Server

Use any SQL Server-compatible tool (for example, Azure Data Studio or sqlcmd) to verify tables/data after running endpoint flows.

## Done Criteria

- All tests green on SQL Server-backed setup.
- API contract unchanged for all existing endpoints.
- SQL Server-specific migrations apply cleanly on fresh database.
- No constitution violations introduced.

## Verification Snapshot (2026-03-15)

- `dotnet build ShoppingList.sln`: Passed.
- `dotnet test test/ShoppingList.Infrastructure.Db.Tests`: Passed with SQL Server Testcontainers.
- `dotnet test ShoppingList.sln`: Passed (93 tests, 0 failures).
