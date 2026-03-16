# Research: Migrate Backend Database to Azure SQL

## Decision 1: Use SQL Server EF Core provider and Aspire Azure SQL integration

- Decision: Replace Npgsql provider usage with SQL Server provider (`UseSqlServer`) and Aspire Azure SQL integration in composition root and AppHost.
- Rationale: Azure SQL is SQL Server-compatible; using native provider/integration minimizes dialect mismatch risk and preserves managed service capabilities.
- Alternatives considered: Keep PostgreSQL for local and only switch in cloud (rejected due to provider divergence risk), use generic ADO.NET abstractions without provider-specific tuning (rejected due to lower operational clarity and resilience options).

## Decision 2: Regenerate migrations for SQL Server instead of mixing providers

- Decision: Remove PostgreSQL-specific migrations and generate a fresh SQL Server migration chain for current domain schema.
- Rationale: EF migrations are provider-specific; mixed migration histories increase maintenance risk and can produce invalid SQL in deployment pipelines.
- Alternatives considered: Keep old PostgreSQL migrations and append SQL Server migrations (rejected due to split history complexity), hand-author migration scripts (rejected due to higher manual error rate).

## Decision 3: Keep domain and API contracts unchanged; migration is persistence-only

- Decision: Do not change domain entity behavior or public REST API contracts; only change infrastructure/provider wiring.
- Rationale: Feature goal is storage backend migration with no user-visible behavior changes.
- Alternatives considered: Introduce API version bump during migration (rejected because no contract changes are required), refactor domain model concurrently (rejected to reduce migration risk surface).

## Decision 4: Preserve real-database integration tests using SQL Server containers

- Decision: Keep integration tests container-backed and switch test infrastructure from PostgreSQL containers to SQL Server containers.
- Rationale: Constitution requires real database testing; this verifies SQL Server-specific relational behavior and migration correctness.
- Alternatives considered: Use EF in-memory provider for speed (rejected by constitution and behavioral mismatch risk), rely on unit tests only (rejected due to migration and SQL dialect blind spots).

## Decision 5: Startup resilience and failure behavior

- Decision: Configure SQL Server connection resiliency for transient failures and fail fast at startup for invalid/missing configuration.
- Rationale: Azure SQL introduces transient network conditions; resilient retry for transient errors plus explicit startup failure for fatal config errors balances reliability and debuggability.
- Alternatives considered: No retries (rejected due to noisy transient failures), retries on all errors (rejected because it can hide persistent misconfiguration).

## Decision 6: Local developer DB access strategy

- Decision: Provide SQL Server container via Aspire for local runs and document CLI/GUI inspection options in quickstart.
- Rationale: Preserves one-command local startup while keeping inspection tooling flexible across macOS/Linux/Windows.
- Alternatives considered: Require shared remote Azure SQL for all developers (rejected due to cost/connectivity coupling), ship OS-specific GUI dependency as mandatory (rejected due to portability constraints).

## Clarifications Resolved

- Performance target clarified: p95 API latency must be at least as good as current baseline, with tolerated regression capped at 10% pending benchmark verification.
- Scope clarified: Migration applies to backend infrastructure, orchestration, tests, and configuration only; no frontend/mobile/iac behavior changes are in scope.
- Data migration clarified: No live PostgreSQL-to-Azure SQL data migration is required for this feature due to early-stage environment assumptions.

## Implementation Notes (2026-03-15)

- Runtime provider wiring now uses SQL Server in API startup and design-time context factory.
- AppHost now uses Azure SQL resource configuration with local container execution support.
- PostgreSQL migrations were replaced with SQL Server-specific migrations generated through EF Core tools.
- Integration tests now run on SQL Server Testcontainers (`Testcontainers.MsSql`) and pass.
