# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

## Summary

[Summarize the user-facing Android capability and the chosen technical approach.]

## Technical Context

**Language/Version**: Kotlin [version or NEEDS CLARIFICATION]  
**Primary Dependencies**: AndroidX plus feature-specific libraries justified in research  
**Storage**: [Room, DataStore, cache, or N/A]  
**Testing**: unit tests plus instrumentation/UI coverage when critical journeys require it  
**Target Platform**: Android [minimum SDK or NEEDS CLARIFICATION]  
**Project Type**: Native Android application  
**Performance Goals**: [startup, rendering, scrolling, sync goals]  
**Constraints**: [offline behavior, background work, device capability, permission constraints]  
**Scale/Scope**: [screen count, data volume, target user scenarios]

## Constitution Check

Verify compliance with ShoppingList Android Constitution (`.specify/memory/constitution.md`):

- ✅ **Native Android Experience First**
- ✅ **Contract-First Integration**
- ✅ **Test-First Development**
- ✅ **Resilient State and Device Awareness**
- ✅ **Accessibility, Performance, and Privacy**

## Project Structure

### Documentation

```text
specs/[###-feature]/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md
```

### Source Code

```text
src/
└── ShoppingList.Android/

test/
├── ShoppingList.Android.Tests/
└── ShoppingList.Android.UiTests/
```

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [example] | [reason] | [why] |
