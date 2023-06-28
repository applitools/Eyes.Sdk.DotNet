#!/usr/bin/env pwsh

$RESULT=0

echo "generating tests - playwright"
pushd coverage-tests
npm run generate:playwright
if ($? -eq $False) {
    echo "npm run dotnet:generate:playwright have failed"
    exit 1
}

# start eg client and save process id
# commented out if need eg client logs
$APPLITOOLS_SHOW_LOGS="true"
yarn universal:eg &
$EG_PID="$!"
$EXECUTION_GRID_URL="http://localhost:8080"
echo $EXECUTION_GRID_URL
echo "building playwright tests"

dotnet build ./test/Playwright
./test/Playwright/bin/Debug/net6.0/playwright.ps1 install

echo "running tests playwright"
npm run run:parallel:playwright
$runResult=$?
echo $runResult
if ($runResult -eq $False) {
    echo "Not all tests passed... Retrying."
    npm run run:parallel:playwright
	if ($? -eq $False) {
      $RESULT=1
      echo "npm run dotnet:run:parallel:playwright have failed"
    }
}

# Kill eg client by the process id
echo $EG_PID
kill $EG_PID

echo "RESULT = $RESULT"
if ($RESULT -eq 0) {
    exit 0
} else {
    exit 1
}
