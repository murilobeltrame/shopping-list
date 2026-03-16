# Feature Specification: Migrate Backend Database to Azure SQL

**Feature Branch**: `001-azure-sql`  
**Created**: 2026-03-15  
**Status**: Draft  
**Input**: User description: "change backend database to AzureSql"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - All Shopping List Data Persisted via Azure SQL (Priority: P1)

As a platform operator, I want the deployed backend to store all shopping list data in an Azure SQL managed database so that the system runs fully within the Microsoft Azure ecosystem and benefits from a managed relational service.

**Why this priority**: This is the core goal of the feature. All other stories depend on the system successfully reading from and writing to Azure SQL. Without this, the migration has no value.

**Independent Test**: Can be fully tested by deploying the backend pointing at an Azure SQL instance, executing all shopping list operations (create list, add items, mark purchased, remove, copy), and confirming all data is correctly persisted and retrieved with no errors.

**Acceptance Scenarios**:

1. **Given** the backend is configured to connect to an Azure SQL database, **When** a user creates a shopping list, **Then** the list is persisted and retrievable with no data loss
2. **Given** the backend is running against Azure SQL, **When** all shopping list operations are performed (add item, mark purchased, remove item, update quantity, copy list), **Then** each operation succeeds and data is consistent
3. **Given** the backend starts up with a fresh Azure SQL database, **When** the application initializes, **Then** the database schema is automatically created or migrated without manual intervention
4. **Given** the backend attempts to connect to Azure SQL and the connection string is missing or invalid, **When** the application starts, **Then** a clear, actionable error is reported and the application does not start silently with corrupt state

---

### User Story 2 - Local Development Works with a Containerized SQL Server (Priority: P1)

As a backend developer, I want to run the full application stack locally using a containerized SQL Server instance so that I can develop and test without needing an Azure subscription or remote database access.

**Why this priority**: Developer productivity and local test reliability are equally critical; if developers cannot run the stack locally, the feedback loop breaks and the team cannot iterate on features.

**Independent Test**: Can be fully tested by running the local Aspire orchestration, confirming a SQL Server container starts automatically, and executing the full suite of integration tests against it.

**Acceptance Scenarios**:

1. **Given** a developer starts the local development environment, **When** the orchestrator launches, **Then** a SQL Server container starts automatically alongside the API with no manual setup required
2. **Given** the local SQL Server container is running, **When** the developer runs integration tests, **Then** all tests pass against the containerized database
3. **Given** the local environment is running, **When** the developer inspects the database, **Then** a database management UI is accessible locally (equivalent to pgweb for the previous PostgreSQL setup)

---

### User Story 3 - Existing Schema and Data Behaviour Preserved (Priority: P2)

As an end user, I want all existing shopping list features to continue working correctly after the database change so that the migration is invisible and does not break any workflows.

**Why this priority**: Correctness is non-negotiable. The migration must be transparent to end users — no feature regressions, no data format changes visible through the API.

**Independent Test**: Can be fully tested by running the existing automated test suite (unit and integration) against the new database backend and confirming all tests pass without modification to test logic.

**Acceptance Scenarios**:

1. **Given** the backend is running against Azure SQL, **When** the full automated test suite is executed, **Then** all previously passing tests continue to pass
2. **Given** the API is running against Azure SQL, **When** a client calls any existing endpoint, **Then** the response contract is identical to the previous behaviour
3. **Given** a shopping list with items exists in the database, **When** the list is retrieved, **Then** all item attributes (description, quantity, purchased, removed) are returned correctly and without data truncation or type coercion errors

---

### Edge Cases

- What happens when the database schema migration fails on startup (e.g., insufficient permissions on Azure SQL)?
- How does the system handle an Azure SQL connection that is briefly unavailable during a transient network event?
- What happens if the new SQL Server-specific migration is applied to a database that previously had the PostgreSQL schema (schema mismatch)?
- How are existing EF Core migrations handled — are they replaced entirely or appended to?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST use Azure SQL as its database provider in all non-development environments
- **FR-002**: The system MUST provide a containerized SQL Server database for local development and testing without requiring any manual setup steps
- **FR-003**: The system MUST automatically apply pending database schema migrations on startup
- **FR-004**: The system MUST preserve all existing shopping list data operations: create list, add item, mark item as purchased, remove item, update item quantity, and copy list
- **FR-005**: The system MUST surface a clear startup error if the database connection string is absent or the connection cannot be established
- **FR-006**: The database schema MUST be regenerated to target the SQL Server dialect, replacing the existing PostgreSQL-dialect migrations
- **FR-007**: The system MUST continue to pass the full existing automated test suite (unit and integration) after the migration
- **FR-008**: The Aspire orchestration host MUST be updated to provision an Azure SQL resource for cloud environments and a SQL Server container for local runs

### Key Entities

- **ShoppingList**: Represents a user's shopping list; attributes: unique ID, owner identifier, optional date, finished status — no change in structure, only in storage dialect
- **ShoppingListItem**: Represents an item on a shopping list; attributes: unique ID, description, optional integer quantity, purchased flag, removed flag — no change in structure
- **Database Migration**: A versioned schema change script; existing PostgreSQL-dialect migrations will be replaced with SQL Server-dialect equivalents

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All existing automated tests (unit and integration) pass without modification to test logic after the migration is complete
- **SC-002**: The full application stack starts from scratch locally in under 2 minutes with no manual configuration steps beyond running the orchestration command
- **SC-003**: All shopping list API endpoints return responses with latency equal to or better than the PostgreSQL baseline under equivalent load conditions
- **SC-004**: Zero data loss or truncation occurs for any shopping list field when stored in and retrieved from Azure SQL
- **SC-005**: The database schema is applied automatically on first boot; operators perform zero manual schema setup steps

## Assumptions

- The project is in early development with no production data, so replacing existing PostgreSQL migrations with new SQL Server migrations is acceptable (no data migration from PostgreSQL to SQL Server is required)
- The local development environment supports running SQL Server in a container (Docker or equivalent is available on developer machines)
- Cloud deployments target Azure and can provision an Azure SQL resource
- The Aspire `RunAsContainer` pattern used for local PostgreSQL will be reused for local SQL Server
