# Android Copilot Instructions

## Scope

This folder owns the native Android client and its local Speckit assets.

## Project Structure

- Source code should live under `src/`
- Tests should live under `test/`
- Speckit assets live under `.specify/` and `specs/`

## Default Engineering Rules

- Keep the client native-first and Android-centric.
- Prefer spec-first planning before creating app structure.
- Keep UI work accessible, responsive, and aligned with backend API contracts.
- Treat offline behavior, loading states, and failure states as first-class requirements.
- Do not reuse backend conventions unless they are explicitly relevant to Android work.