#!/usr/bin/env bash

RESULT=0
MESSAGE=""

echo "generating tests - selenium 3"
pushd coverage-tests
# export UFG_ON_EG=true
npm run generate
if [ $? -ne 0 ]; then
    ((RESULT+=1))
    MESSAGE+=$'npm run dotnet:generate have failed'
    echo $MESSAGE
fi

# start eg client and save process id
# commented out if need eg client logs
export APPLITOOLS_SHOW_LOGS=true
yarn universal:eg &
EG_PID="$!"
export EXECUTION_GRID_URL=http://localhost:8080
echo $EXECUTION_GRID_URL

echo "running tests - selenium 3"
npm run run:parallel:selenium
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    npm run run:parallel:selenium
	if [ $? -ne 0 ]; then
      ((RESULT+=1))
      MESSAGE+=$'npm run dotnet:run:parallel:selenium have failed'
      echo $MESSAGE
    fi
fi

# Kill eg client by the process id
echo $EG_PID
kill $EG_PID

echo "RESULT = ${RESULT}"
echo $MESSAGE
if [ $RESULT -eq 0 ]; then
    exit 0
else
    exit 1
fi