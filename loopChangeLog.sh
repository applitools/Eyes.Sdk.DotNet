#!/usr/bin/env bash

IFS=";" read -ra RELEASE_PACKAGE <<< "$1"

for i in "${RELEASE_PACKAGE[@]}"; do
  if [ $i != "RELEASE_CANDIDATE" ]; then
    PACKAGE=$(echo "$i" | cut -d '@' -f 1)
    VERSION=$(echo "$i" | cut -d '@' -f 2)
    echo "## [$PACKAGE $VERSION]";
    ./extractChangeLog.sh $i $2
  fi
done
