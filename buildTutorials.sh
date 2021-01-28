#!/usr/bin/env bash

pushd Eyes.Selenium.DotNet/Properties/NodeResources
npm install
popd

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
