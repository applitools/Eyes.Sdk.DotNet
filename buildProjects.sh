#!/usr/bin/env bash

echo "running tests: $1 $2 $3"

dotnet test -c Release $1 $2 $3
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    dotnet test -c Release $1 $2 $3
fi