#!/usr/bin/env bash

pushd Eyes.Selenium.DotNet/Properties/NodeResources
npm install
popd

echo "running tests"

dotnet test -f netcoreapp3.1 Eyes.Sdk.DotNet_Travis.sln --filter $1
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    dotnet test -f netcoreapp3.1 Eyes.Sdk.DotNet_Travis.sln --filter $1
fi

echo "running tutorials"
chmod +x ./tutorials/run.sh
chmod +x ./tutorials/build.sh
chmod +x ./tutorials/report.sh
./tutorials/run.sh
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tutorials passed, the build have failed"
    exit 1
fi
