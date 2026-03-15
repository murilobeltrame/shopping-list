# ShoppingList iOS Constitution

## Core Principles

### I. Native iOS Experience First

Features MUST feel native on iPhone and iPad, using platform-appropriate interaction patterns, navigation, accessibility semantics, and lifecycle handling.

### II. Contract-First Integration

All backend interactions MUST be driven by explicit API contracts from the backend specs. Client-side assumptions about payloads, errors, pagination, or authentication are prohibited unless they are documented.

### III. Test-First Development (NON-NEGOTIABLE)

Every feature MUST follow Red-Green-Refactor. Unit tests cover presentation and domain logic; UI or integration tests cover critical user journeys when behavior depends on navigation, rendering, or device services.

### IV. Resilient Client State

Loading, empty, offline, retry, and failure states are first-class requirements. Features MUST define how local state is restored, refreshed, and reconciled with server truth.

### V. Accessibility, Privacy, and Performance

Features MUST ship with accessible labels and focus order, respect user privacy, and define measurable performance expectations for startup, screen transitions, and list rendering.

## Project Structure & Quality Gates

- Source code lives under `src/`
- Tests live under `test/`
- Specs live under `specs/`
- Every plan MUST define the chosen iOS UI stack and testing split
- Pull requests MUST document contract changes, state-management impact, and accessibility impact

## Governance

This constitution supersedes local implementation preferences. Any justified deviation MUST be documented in `plan.md` under Complexity Tracking and approved during review.

**Version**: 1.0.0 | **Ratified**: 2026-03-14 | **Last Amended**: 2026-03-14
