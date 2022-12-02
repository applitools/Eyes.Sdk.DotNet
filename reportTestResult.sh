#!/usr/bin/env bash

find . -name Test_Results_*.json -exec curl -v -X POST -H "Content-Type: application/json" -d @"{}" http://applitools-quality-server.herokuapp.com/result \;