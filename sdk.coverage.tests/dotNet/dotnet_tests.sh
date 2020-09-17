RESULT=0
npm run dotnet:generate
if [ $? -ne 0 ]; then
    RESULT=1
    echo "npm run dotnet:generate have failed"
fi
npm run dotnet:run:parallel
if [ $? -ne 0 ]; then
    RESULT=1
    echo "npm run dotnet:run:parallel have failed"
fi
npm run dotnet:report
if [ $? -ne 0 ]; then
    RESULT=1
    echo "npm run dotnet:report have failed"
fi
echo "RESULT = ${RESULT}"
if [ $RESULT -eq 0 ]; then
    exit 0
else
    exit 1
fi