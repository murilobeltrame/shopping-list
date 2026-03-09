#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
REPORT_DIR="$ROOT_DIR/coveragereport"
REPORT_INDEX="$REPORT_DIR/index.html"

echo "Running all tests with coverage..."
dotnet test "$ROOT_DIR/ShoppingList.sln" \
	/p:CollectCoverage=true \
	/p:CoverletOutputFormat=cobertura

echo "Generating coverage report..."
if command -v reportgenerator >/dev/null 2>&1; then
	reportgenerator \
		-reports:"$ROOT_DIR/**/coverage.cobertura.xml" \
		-targetdir:"$REPORT_DIR" \
		-reporttypes:Html
else
	dotnet tool run reportgenerator \
		-reports:"$ROOT_DIR/**/coverage.cobertura.xml" \
		-targetdir:"$REPORT_DIR" \
		-reporttypes:Html
fi

if [[ -f "$REPORT_INDEX" ]]; then
	echo "Opening coverage report: $REPORT_INDEX"
	open "$REPORT_INDEX"
else
	echo "Coverage report was not generated at $REPORT_INDEX"
	exit 1
fi