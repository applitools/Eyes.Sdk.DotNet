module.exports = {
  name: 'eyes_selenium_dotnet',
  emitter: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/images-sdk/DotNet/emitter.js',
  overrides: [
      'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/images-sdk/js/overrides.js',
      'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/images-sdk/DotNet/overrides.js',
    ],
  template: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/master/DotNet/template.hbs',
  tests: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/images-sdk/coverage-tests.js',
  ext: '.cs',
  outPath: './test/Selenium/coverage/generic'
};