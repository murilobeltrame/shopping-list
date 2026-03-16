# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

## Summary

[Summarize the user-facing iOS capability and the chosen technical approach.]

## Technical Context

**Language/Version**: Swift [version or NEEDS CLARIFICATION]  
**Primary Dependencies**: Native iOS frameworks plus feature-specific libraries justified in research  
**Storage**: [local persistence, cache, or N/A]  
**Testing**: XCTest, UI tests when critical journeys require device-level coverage  
**Target Platform**: iOS [minimum version or NEEDS CLARIFICATION]  
**Project Type**: Native iOS application  
**Performance Goals**: [startup, screen transition, scrolling, sync goals]  
**Constraints**: [offline behavior, background execution, device capability, privacy constraints]  
**Scale/Scope**: [screen count, data volume, target user scenarios]

## Constitution Check

Verify compliance with ShoppingList iOS Constitution (`.specify/memory/constitution.md`):

- ✅ **Native iOS Experience First**
- ✅ **Contract-First Integration**
- ✅ **Test-First Development**
- ✅ **Resilient Client State**
- ✅ **Accessibility, Privacy, and Performance**

If any violations are needed, document them in Complexity Tracking.

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
└── ShoppingList.iOS/

test/
├── ShoppingList.iOS.Tests/
└── ShoppingList.iOS.UITests/
```

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [example] | [reason] | [why] |
