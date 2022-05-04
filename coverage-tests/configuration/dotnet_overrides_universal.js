module.exports = {
    'check region by selector in frame fully on firefox legacy': {skipEmit: true},
    'check region by selector within shadow dom with vg': {skipEmit: true},
    'check region by element within shadow dom with vg': {skipEmit: true},
    'should handle check of stale element if selector is preserved': {skip: true},
    'should handle check of stale element in frame if selector is preserved': {skip: true},
    'check window after manual scroll on safari 11': {skip: true},
    'should extract text from regions': {skipEmit: true},
    'should extract text regions from image': {skipEmit: true},
    'should fail check of stale element': {skip: true},
    'should set viewport size': {skip: true},
    'should set viewport size on edge legacy': {skip: true},

    // Generation errors:
	'check window on mobile web android': {skipEmit: true},
	'check window on mobile web ios': {skipEmit: true},
	'should extract text from regions without a hint': {skipEmit: true},
	'should abort after close': {skipEmit: true},
	'should abort after close with vg': {skipEmit: true},
	'should return aborted tests in getAllTestResults': {skipEmit: true},
	'should return aborted tests in getAllTestResults with vg': {skipEmit: true},
	'should send agentRunId': {skipEmit: true},
	'appium android landscape mode check window on android 7': {skipEmit: true},
	'appium android landscape mode check window on android 10': {skipEmit: true},
	'appium android landscape mode check region on android 7': {skipEmit: true},
	'appium android landscape mode check region on android 10': {skipEmit: true},
	// Compilation errors:
	'should waitBeforeCapture with breakpoints in check': {skipEmit: true},
	'should waitBeforeCapture with breakpoints in open': {skipEmit: true},
	'should return browserInfo in getAllTestResults': {skipEmit: true},
	// // Page coverage feature
	'pageCoverage data is correct': {skipEmit: true},
	'pageCoverage data is correct with vg': {skipEmit: true},
	'appium iOS check fully window with scroll and pageCoverage': {skipEmit: true},
	'appium iOS check window region with scroll and pageCoverage': {skipEmit: true},

	// Flaky tests
	'should be empty if page delayed by 1500': {skip: true},
}
