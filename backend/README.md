# Backend

This folder contains the ShoppingList backend implementation and its local Spec-Driven Development assets.

## Contents

- `src/`: .NET application source projects
- `test/`: automated test projects
- `env/`: Aspire orchestration and shared service defaults
- `.specify/`: backend-local Speckit templates, scripts, and constitution
- `specs/`: backend feature specifications and plans

## Working Here

Run backend build and test commands from this folder:

```bash
dotnet build ShoppingList.sln
dotnet test ShoppingList.sln
dotnet run --project env/ShoppingList.AppHost
```

Speckit commands targeting the backend should use this folder as `SPECIFY_ROOT`.