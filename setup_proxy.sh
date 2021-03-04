#!/usr/bin/env bash
openssl req -new -newkey rsa:4096 -days 1 -nodes -x509 -subj "/C=US/ST=Denial/L=Springfield/O=Dis/CN=*.applitools.com" -keyout DO_NOT_TRUST.key  -out DO_NOT_TRUST.crt
cat DO_NOT_TRUST.key DO_NOT_TRUST.crt > DO_NOT_TRUST.pem
sudo mkdir /usr/share/ca-certificates/extra
sudo cp DO_NOT_TRUST.crt /usr/share/ca-certificates/extra
sudo update-ca-certificates
sudo apt-get install mitmproxy
mitmdump --cert=DO_NOT_TRUST.pem -s ./har_dump.py --set hardump=./network.log.har &