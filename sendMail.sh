#!/usr/bin/env bash
COMMITTER_EMAIL=$(git log -1 $TRAVIS_COMMIT --pretty="%cE")
echo "$COMMITTER_EMAIL"
curl http://applitools-quality-server.herokuapp.com/send_mail -X POST -H "Content-Type: application/json" -d @SEND_MAIL.json