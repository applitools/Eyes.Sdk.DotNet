#!/usr/bin/env bash
sudo apt-get install mitmproxy
mitmdump &
curl --proxy "http://127.0.0.1:8080" https://mitm.it
pkill mitmdump
sudo mkdir /usr/share/ca-certificates/extra
sudo cp ~/.mitmproxy/mitmproxy-ca-cert.cer /usr/share/ca-certificates/extra
sudo echo "extra/mitmproxy-ca-cert.cer" >> /etc/ca-certificates.conf
sudo update-ca-certificates
mitmdump -s ./har_dump.py --set hardump=./network.log.har &