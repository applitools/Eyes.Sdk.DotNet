#!/usr/bin/env bash

PACKAGE=$(echo "$1" | cut -d '@' -f 1)
VERSION=$(echo "$1" | cut -d '@' -f 2)

awk -v version="$VERSION]" -v package="[$PACKAGE" '
/^## \[/ { if (p) { exit}; if ($2 == package && $3 == version) { p=1; next} } p && NF
' "$2"
