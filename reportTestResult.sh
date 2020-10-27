#!/usr/bin/env bash

find . -name Test_Results_*.json -exec curl -v -X POST -H "Content-Type: application/json" -d @"{}" http://sdk-test-results.herokuapp.com/result \;