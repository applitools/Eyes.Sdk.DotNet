#!/usr/bin/env bash
echo "DEPLOY"
git remote set-url origin git@github.com:${TRAVIS_REPO_SLUG}.git
git add */*.csproj CHANGELOG.md
git commit -m 'Updated CHANGELOG and bumped versions.'
git push origin HEAD:$RELEASE_BRANCH
while read p; do  echo $p; git tag $p; done <NEW_TAGS.txt
git push origin HEAD:$RELEASE_BRANCH --tags
