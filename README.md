# Shopping list

An experimentation on Specification Driven Development and how it aligns on:
- Well defined architecture standarts;
- Not well known platform, framework or language;
- Development workflow

## Repository layout

Only cross-cutting repository content stays at the root. Each product area now owns its own implementation files and Speckit assets.

- `backend/`: .NET backend, tests, Aspire environment, backend-local `.specify/`, and backend `specs/`
- `ios/`: native iOS app area with iOS-local `.specify/` and `specs/`
- `android/`: native Android app area with Android-local `.specify/` and `specs/`
- `web/`: optional web frontend area with web-local `.specify/` and `specs/`
- `iac/`: infrastructure-as-code area with IaC-local `.specify/` and `specs/`

## Main parts

- backend: the REST api of the application
- ios: the native iOS frontend of the application
- android: the native Android frontend of the application
- (optional) web: the web frontend of the application
- iac: the infrastructure bits of the hosted part of application

## Speckit usage

Each main part owns its own Speckit workspace. Before running any Speckit shell script, resolve the target part and set `SPECIFY_ROOT` to that folder.

General pattern:

```bash
SPECIFY_ROOT="$PWD/<part>" "$SPECIFY_ROOT/.specify/scripts/bash/<script>.sh" [args]
```

Rules:

- Choose exactly one target part first: `backend`, `ios`, `android`, `web`, or `iac`
- Set `SPECIFY_ROOT` to that part's absolute path
- Use only paths under `$SPECIFY_ROOT/.specify/...`, `$SPECIFY_ROOT/specs/...`, and `$SPECIFY_ROOT/.github/...`
- Do not assume the repository root contains the active `.specify/` or `specs/` directories

### Backend context

Use this when working on the .NET API, backend tests, or Aspire setup.

```bash
SPECIFY_ROOT="$PWD/backend" "$SPECIFY_ROOT/.specify/scripts/bash/check-prerequisites.sh" --json
SPECIFY_ROOT="$PWD/backend" "$SPECIFY_ROOT/.specify/scripts/bash/create-new-feature.sh" --json "Add shopping list sharing"
```

### iOS context

Use this when working on the native iOS application and its feature specs.

```bash
SPECIFY_ROOT="$PWD/ios" "$SPECIFY_ROOT/.specify/scripts/bash/create-new-feature.sh" --json "Add onboarding flow"
SPECIFY_ROOT="$PWD/ios" "$SPECIFY_ROOT/.specify/scripts/bash/check-prerequisites.sh" --json --paths-only
```

### Android context

Use this when working on the native Android application and its feature specs.

```bash
SPECIFY_ROOT="$PWD/android" "$SPECIFY_ROOT/.specify/scripts/bash/create-new-feature.sh" --json "Add offline shopping list cache"
SPECIFY_ROOT="$PWD/android" "$SPECIFY_ROOT/.specify/scripts/bash/check-prerequisites.sh" --json
```

### Web context

Use this when working on the optional web frontend and its local planning artifacts.

```bash
SPECIFY_ROOT="$PWD/web" "$SPECIFY_ROOT/.specify/scripts/bash/create-new-feature.sh" --json "Add public landing page"
SPECIFY_ROOT="$PWD/web" "$SPECIFY_ROOT/.specify/scripts/bash/setup-plan.sh" --json
```

### IaC context

Use this when working on infrastructure definitions, environments, and deployment planning.

```bash
SPECIFY_ROOT="$PWD/iac" "$SPECIFY_ROOT/.specify/scripts/bash/create-new-feature.sh" --json "Provision production environment"
SPECIFY_ROOT="$PWD/iac" "$SPECIFY_ROOT/.specify/scripts/bash/check-prerequisites.sh" --json --include-tasks
```

### Common workflow

Typical flow inside any context:

```bash
SPECIFY_ROOT="$PWD/<part>" "$SPECIFY_ROOT/.specify/scripts/bash/create-new-feature.sh" --json "Feature description"
SPECIFY_ROOT="$PWD/<part>" SPECIFY_FEATURE="001-feature-name" "$SPECIFY_ROOT/.specify/scripts/bash/setup-plan.sh" --json
SPECIFY_ROOT="$PWD/<part>" SPECIFY_FEATURE="001-feature-name" "$SPECIFY_ROOT/.specify/scripts/bash/check-prerequisites.sh" --json --require-tasks --include-tasks
```

Replace `<part>` with the product area you are actively changing. Keep all Speckit operations scoped to that part for the duration of the task.
```
