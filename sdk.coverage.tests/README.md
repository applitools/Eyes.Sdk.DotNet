# DotNet Coverege Tests
#### Pre-requisites:

1. [.NET Core SDK](https://docs.microsoft.com/en-us/dotnet/core/install/)

2. [node_js and npm](https://docs.npmjs.com/downloading-and-installing-node-js-and-npm)

3. Environment variable APPLITOOLS_BATCH_ID should be created with any value.

   

## Steps to run the tests

1. DotNet Coverege Tests are included in repo Eyes.Sdk.DotNet like git submodule. So to clone repo use commands:

   - git clone --recursive https://github.com/applitools/Eyes.Sdk.DotNet.git 

   - cd Eyes.Sdk.DotNet

   - git submodule update --remote

2. Change dir to  'sdk.coverage.tests' - cd sdk.coverage.tests/

3. Run tests: 

   3.1 On Windows run  `dotnet_tests_Win.bat`

   3.2 On Linux run  `npm run dotnet:tests`

4. These commands generate and run tests, send report if all the tests are passed



#### There command to run tests on the CI `npm run dotnet:tests  --prefix ./sdk.coverage.tests`

Also it's possible to execute separate commands of generate tests, run the tests, send report. To perform this cande dir to sdk.coverage.tests and perform:

-  npm run dotnet:generate
- npm run dotnet:run:parallel
- npm run dotnet:report