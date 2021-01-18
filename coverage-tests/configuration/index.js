module.exports = {
  name: 'eyes_selenium_dotnet',
  emitter: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/master/DotNet/emitter.js',
  overrides: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/master/DotNet/overrides.js',
  template: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/master/DotNet/template.hbs',
  ext: '.cs',
  outPath: './test/Selenium/coverage/generic'
}