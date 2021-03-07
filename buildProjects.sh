#!/usr/bin/env bash

pushd Eyes.Selenium.DotNet/Properties/NodeResources
npm install
popd

echo "running tests: $1 $2 $3"

dotnet test -f net5.0 -c Release $1 $2 $3
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    dotnet test -f net5.0 -c Release $1 $2 $3
fi