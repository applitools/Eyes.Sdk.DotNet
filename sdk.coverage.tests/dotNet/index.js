const supportedTests = require('./supported-tests')
const initialize = require('./initialize')
const testFrameworkTemplate = require('./template')

module.exports = {
    name: 'eyes_selenium_dotnet',
    initialize: initialize,
    supportedTests,
    testFrameworkTemplate: testFrameworkTemplate,
    ext: '.cs',
    out: './test/coverage/generic'
}