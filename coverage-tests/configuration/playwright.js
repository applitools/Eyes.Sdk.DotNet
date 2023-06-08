module.exports = {
  name: "eyes_playwright_dotnet",
  emitter: "https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/DotNet/playwright/emitter.js",
  overrides: [
    "https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/js/overrides.js",
    "https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/DotNet/overrides/overrides-playwright.js"
  ],
  template: "https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/DotNet/playwright/template.hbs",
  tests: "https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/coverage-tests.js",
  ext: ".cs",
  outPath: './test/Playwright/coverage/generic',
};