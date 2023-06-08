#!/usr/bin/env bash
wget --no-verbose -N https://github.com/mozilla/geckodriver/releases/download/v0.30.0/geckodriver-v0.30.0-linux64.tar.gz -P ~/
tar -xzf ~/geckodriver-v0.30.0-linux64.tar.gz -C ~/
rm ~/geckodriver-v0.30.0-linux64.tar.gz
sudo mv -f ~/geckodriver /home/travis/build/
sudo chmod +x /home/travis/build/geckodriver