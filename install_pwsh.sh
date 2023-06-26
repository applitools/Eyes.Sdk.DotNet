#!/usr/bin/env bash

echo "installing powershell"
wget --no-verbose https://github.com/PowerShell/PowerShell/releases/download/v7.3.4/powershell_7.3.4-1.deb_amd64.deb
sudo dpkg -i powershell_7.3.4-1.deb_amd64.deb
sudo apt-get install -f