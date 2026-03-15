# Android

This folder is reserved for the native Android client and its local Spec-Driven Development assets.

## Contents

- `.specify/`: Android-local Speckit templates, scripts, and constitution
- `specs/`: Android feature specifications and plans

Implementation code has not been added yet. New Android features should be specified and planned from this folder so they do not inherit backend-specific rules.

## Working Here

Run Android build and test commands from the `src/` folder:

```bash
cd src

# Build the debug variant
./gradlew assembleDebug

# Run JVM unit tests
./gradlew test

# Run instrumented tests (requires a connected device or running emulator)
./gradlew connectedAndroidTest
```