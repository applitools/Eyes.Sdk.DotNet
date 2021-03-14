#!/usr/bin/env bash

pushd Eyes.Selenium.DotNet/Properties/NodeResources
npm install
popd

RESULT=0

echo "generating tests"
pushd coverage-tests
npm run dotnet:generate
if [ $? -ne 0 ]; then
    RESULT=1
    echo "npm run dotnet:generate have failed"
fi
popd

echo "running tests selenium"
pushd coverage-tests
npm run dotnet:run:parallel:selenium
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    npm run dotnet:run:parallel:selenium
	if [ $? -ne 0 ]; then
      RESULT=1
      echo "npm run dotnet:run:parallel:selenium have failed"
    fi
fi
popd

echo "running tests appium"
pushd coverage-tests
npm run dotnet:run:parallel:appium
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    npm run dotnet:run:parallel:appium
	if [ $? -ne 0 ]; then
      RESULT=1
      echo "npm run dotnet:run:parallel:appium have failed"
    fi
fi
popd

echo "merge reports"
pushd coverage-tests
npm run dotnet:report:merge
if [ $? -ne 0 ]; then
    RESULT=1
    echo "npm run dotnet:report:merge have failed"
fi
popd

echo "tests reporting"
pushd coverage-tests
npm run dotnet:report
if [ $? -ne 0 ]; then
    RESULT=1
    echo "npm run dotnet:report have failed"
fi
popd

echo "RESULT = ${RESULT}"
if [ $RESULT -eq 0 ]; then
    exit 0
else
    exit 1
fi