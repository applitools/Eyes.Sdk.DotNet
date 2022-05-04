#!/usr/bin/env bash

pushd Eyes.Selenium4.DotNet/Properties/NodeResources
npm install
popd
pushd Eyes.Selenium.DotNet/Properties/NodeResources
npm install
popd
RESULT=0

echo "generating tests"
pushd coverage-tests
npm run generate
if [ $? -ne 0 ]; then
    RESULT=1
    echo "npm run dotnet:generate have failed"
    exit 1
fi
popd

echo "running tests selenium"
pushd coverage-tests
npm run run:parallel:selenium4
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    npm run run:parallel:selenium4
	if [ $? -ne 0 ]; then
      RESULT=1
      echo "npm run dotnet:run:parallel:selenium have failed"
      exit 1
    fi
fi
popd

echo "RESULT = ${RESULT}"
if [ $RESULT -eq 0 ]; then
    exit 0
else
    exit 1
fi