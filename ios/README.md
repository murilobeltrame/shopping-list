# iOS

This folder is reserved for the native iOS client and its local Spec-Driven Development assets.

## Contents

- `.specify/`: iOS-local Speckit templates, scripts, and constitution
- `specs/`: iOS feature specifications and plans

Implementation code has not been added yet. New iOS features should be specified and planned from this folder so they do not inherit backend-specific rules.

## Working Here

Run iOS build and test commands from this folder:

```bash
# Build for the iOS Simulator
xcodebuild build \
  -project "src/Shopping List.xcodeproj" \
  -scheme "Shopping List" \
  -sdk iphonesimulator

# Run tests on the iOS Simulator
xcodebuild test \
  -project "src/Shopping List.xcodeproj" \
  -scheme "Shopping List" \
  -sdk iphonesimulator \
  -destination "platform=iOS Simulator,name=iPhone 16"
```