# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

## Summary

[Summarize the infrastructure capability and the chosen technical approach.]

## Technical Context

**Language/Tooling**: [Terraform, Bicep, Pulumi, or NEEDS CLARIFICATION]  
**Primary Dependencies**: chosen IaC toolchain and validation tooling  
**Target Platform**: [cloud, runtime, or environment targets]  
**Testing**: formatting, static validation, plan/preview review, policy or smoke validation as applicable  
**State Management**: [remote state, locking, or N/A]  
**Project Type**: Infrastructure as code  
**Performance Goals**: [provisioning or rollout expectations]  
**Constraints**: [compliance, permissions, network, cost, rollback, drift constraints]  
**Scale/Scope**: [environment count, resource count, tenancy, or blast radius]

## Constitution Check

Verify compliance with ShoppingList IaC Constitution (`.specify/memory/constitution.md`):

- ✅ **Reproducible Infrastructure First**
- ✅ **Environment Isolation and Explicit Inputs**
- ✅ **Validate Before Apply**
- ✅ **Security, Least Privilege, and Observability**
- ✅ **Safe Rollout and Drift Awareness**

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
modules/
environments/
test/
```

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [example] | [reason] | [why] |
