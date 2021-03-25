#!/usr/bin/env bash
latestChromeDriverURL=$(wget http://chromedriver.storage.googleapis.com/LATEST_RELEASE -q -O -)
wget "http://chromedriver.storage.googleapis.com/${latestChromeDriverURL}/chromedriver_linux64.zip"
unzip chromedriver_linux64.zip -d /home/travis/build/
export CHROME_BIN=chromium-browser