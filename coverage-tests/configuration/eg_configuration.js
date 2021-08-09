module.exports = {
    name: 'eyes_selenium_dotnet',
    emitter: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/eg_dotnet/DotNet/emitter.js',
    overrides: [
        'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/eg_dotnet/DotNet/overrides.js',
        'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/eg_dotnet/eg.overrides.js',
    ],
    template: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/eg_dotnet/DotNet/template.hbs',
    ext: '.cs',
    outPath: './test/Selenium/coverage/generic'
  };