#!/usr/bin/env bash
echo "DEPLOY"
git remote set-url origin https://${GITHUB_TOKEN}@github.com/${TRAVIS_REPO_SLUG}
git add */*.csproj CHANGELOG.md
git commit -m 'Updated CHANGELOG and bumped versions.'
git push origin HEAD:$RELEASE_BRANCH
while read p; do  echo $p; git tag $p; done <NEW_TAGS.txt
git push origin HEAD:$RELEASE_BRANCH --tags
while read p; do  echo $p; dotnet pack $p.DotNet/$p.DotNet.csproj; done <UPDATED_PROJECTS.txt
dotnet nuget push PackagesOutput/*.nupkg --source https://api.nuget.org/v3/index.json --api-key ${NUGET_API_KEY} --skip-duplicate
