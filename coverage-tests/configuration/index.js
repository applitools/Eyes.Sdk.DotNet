module.exports = {
  name: 'eyes_selenium_dotnet',
  emitter: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/moveDotNetToMaster/DotNet/emitter.js',
  overrides: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/moveDotNetToMaster/DotNet/overrides.js',
  template: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/moveDotNetToMaster/DotNet/template.hbs',
  ext: '.cs',
  outPath: './test/Selenium/coverage/generic'
}