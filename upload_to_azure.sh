#!/usr/bin/env bash

DATE_NOW=$(date -Ru | sed 's/\+0000/GMT/')
AZ_VERSION="2018-03-28"
AZ_BLOB_URL="https://sdksstorage.blob.core.windows.net"
AZ_BLOB_CONTAINER="itai"
AZ_BLOB_TARGET="${AZ_BLOB_URL}/${AZ_BLOB_CONTAINER}/"
AZ_SAS_TOKEN="sp=rc&st=2021-03-04T08:56:00Z&se=2023-03-04T16:56:00Z&spr=https&sv=2020-02-10&sr=c&sig=$1"

curl -v -X PUT -H "Content-Type: application/octet-stream" -H "x-ms-date: ${DATE_NOW}" -H "x-ms-version: ${AZ_VERSION}" -H "x-ms-blob-type: BlockBlob" --data-binary "@network.log.zhar" "${AZ_BLOB_TARGET}${TRAVIS_JOB_NUMBER}.har?${AZ_SAS_TOKEN}"
