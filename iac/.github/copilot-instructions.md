# IaC Copilot Instructions

## Scope

This folder owns infrastructure-as-code assets and their local Speckit assets.

## Project Structure

- Infrastructure source should live under `src/` or `modules/`, as defined by the feature plan
- Tests and validation assets should live under `test/`
- Speckit assets live under `.specify/` and `specs/`

## Default Engineering Rules

- Treat infrastructure as versioned product code.
- Keep environments reproducible and explicitly parameterized.
- Prefer reviewable plans and validation before any apply step.
- Model security, least privilege, observability, and rollback paths as first-class requirements.