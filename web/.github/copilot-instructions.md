# Web Copilot Instructions

## Scope

This folder owns the optional web frontend and its local Speckit assets.

## Project Structure

- Source code should live under `src/`
- Tests should live under `test/`
- Speckit assets live under `.specify/` and `specs/`

## Default Engineering Rules

- Keep web work contract-first with the backend.
- Prefer TypeScript for production code unless a feature plan justifies otherwise.
- Treat accessibility, responsiveness, and clear failure states as baseline requirements.
- Keep the chosen framework and build tool explicit in each feature plan instead of assuming backend conventions.