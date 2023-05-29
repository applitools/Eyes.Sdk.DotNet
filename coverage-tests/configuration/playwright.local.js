module.exports = {
  name: "eyes_playwright_dotnet",
  emitter: "/home/itaibh/devel/sdk.coverage.tests/DotNet/playwright/emitter.js",
  overrides: [
    "/home/itaibh/devel/sdk.coverage.tests/DotNet/overrides/overrides-playwright.js"
  ],
  template: "/home/itaibh/devel/sdk.coverage.tests/DotNet/playwright/template.hbs",
  tests: "/home/itaibh/devel/sdk.coverage.tests/coverage-tests.js",
  ext: ".cs",
  outPath: './test/Playwright/coverage/generic',
  //emitOnly: ["check window with layout breakpoints", "should extract text regions from image"],
};