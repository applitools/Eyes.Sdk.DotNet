#!/bin/bash

set -e
DIR=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
ROOT=$( cd $DIR/../ ; pwd -P)
docker build -t dotnet_basic -f $DIR/basic/Dockerfile $ROOT
docker build -t dotnet_ufg -f $DIR/ultrafast/Dockerfile $ROOT