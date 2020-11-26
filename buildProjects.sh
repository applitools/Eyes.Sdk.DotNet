#!/usr/bin/env bash

cd Eyes.Selenium.DotNet/Properties/NodeResources
npm install

echo "generating tests"

cd ../../../sdk.coverage.tests
npm run dotnet:generate
cd ..

echo "running tests"

dotnet test -f netcoreapp3.0 Eyes.Sdk.DotNet_Travis.sln
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    dotnet test -f netcoreapp3.0 Eyes.Sdk.DotNet_Travis.sln
fi