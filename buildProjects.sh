#!/usr/bin/env bash

cd Eyes.Selenium.DotNet/Properties/NodeResources
npm install

echo "running tests"

dotnet test -f netcoreapp3.1 Eyes.Sdk.DotNet_Travis.sln --filter $1
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    dotnet test -f netcoreapp3.1 Eyes.Sdk.DotNet_Travis.sln --filter $1
fi