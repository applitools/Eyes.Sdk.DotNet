module.exports = {
    name: 'eyes_selenium_dotnet',
    emitter: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/DotNet/emitter.js',
    overrides: [
        'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/js/overrides.js',
        'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/DotNet/overrides/overrides-selenium.js',
    ],
    template: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/master/DotNet/template.hbs',
    tests: 'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/coverage-tests.js',
    ext: '.cs',
    outPath: './test/Selenium/coverage/generic'
};