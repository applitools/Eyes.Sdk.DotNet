#!/bin/bash

set -e
DIR=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
$DIR/build.sh
docker run -e APPLITOOLS_API_KEY dotnet_basic 
docker run -e APPLITOOLS_API_KEY dotnet_ufg 
$DIR/report.sh