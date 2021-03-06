#!/usr/bin/env bash
sudo apt-get install mitmproxy
mitmdump &
sleep 1
curl --proxy "http://127.0.0.1:8080" http://mitm.it/
sleep 1
pkill mitmdump
ls -la ~
sudo mkdir /usr/share/ca-certificates/extra
sudo cp ~/.mitmproxy/mitmproxy-ca-cert.cer /usr/share/ca-certificates/extra
echo "extra/mitmproxy-ca-cert.cer" | sudo tee -a /etc/ca-certificates.conf
sudo update-ca-certificates
mitmdump -s ./har_dump.py --set hardump=./network.log.har &