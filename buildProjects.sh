#!/usr/bin/env bash

cd Eyes.Selenium.DotNet/Properties/NodeResources
npm install

echo "copying resource files..."
mkdir ../Resources
cp node_modules/@applitools/dom-capture/dist/captureDomAndPoll.js ../Resources/
cp node_modules/@applitools/dom-capture/dist/captureDomAndPollForIE.js ../Resources/
cp node_modules/@applitools/dom-snapshot/dist/pollResult.js ../Resources/
cp node_modules/@applitools/dom-snapshot/dist/pollResultForIE.js ../Resources/
cp node_modules/@applitools/dom-snapshot/dist/processPagePoll.js ../Resources/
cp node_modules/@applitools/dom-snapshot/dist/processPagePollForIE.js ../Resources/
echo $?
echo "listing copied resource files..."

ls ../Resources

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