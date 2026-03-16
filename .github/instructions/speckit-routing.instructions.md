---
applyTo: "**"
description: "Use when running Speckit workflows in this mono-repo; resolve the target part and set SPECIFY_ROOT before using any .specify or specs path."
---

This repository contains multiple product areas, each with its own Speckit workspace:

- `backend/`
- `ios/`
- `android/`
- `web/`
- `iac/`

When a task involves Speckit:

1. Resolve the target part first.
2. Set `SPECIFY_ROOT` to the absolute path of that part.
3. Use only `$SPECIFY_ROOT/.specify/...`, `$SPECIFY_ROOT/specs/...`, and `$SPECIFY_ROOT/.github/...` paths.

Examples:

```bash
SPECIFY_ROOT="$PWD/backend" "$SPECIFY_ROOT/.specify/scripts/bash/check-prerequisites.sh" --json
SPECIFY_ROOT="$PWD/web" "$SPECIFY_ROOT/.specify/scripts/bash/create-new-feature.sh" --json "Add public landing page"
```

Do not assume the repository root contains the active `.specify/` or `specs/` directories.