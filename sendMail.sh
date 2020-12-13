#!/usr/bin/env bash
COMMITTER_EMAIL=$(git log -1 $TRAVIS_COMMIT --pretty="%cE")
echo "$COMMITTER_EMAIL"

if [[ $APPLITOOLS_FULL_COVERAGE = true ]]; then
	curl http://sdk-test-results.herokuapp.com/send_full_regression/sdks -X POST -H "Content-Type: application/json" -d @SEND_MAIL.json
else
	curl http://sdk-test-results.herokuapp.com/send_mail/sdks -X POST -H "Content-Type: application/json" -d @SEND_MAIL.json
fi