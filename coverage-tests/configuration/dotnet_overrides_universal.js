module.exports = {
    // add tests here to skip if they fail and fixes are postponded. To skip a test add it like in next string
    //'check window after manual scroll on safari 11': {skip: true},
    'check region by selector in frame fully on firefox legacy': {skipEmit: true},
    'should extract text from regions': {skipEmit: true},
    'should extract text regions from image': {skipEmit: true},
    'should not fail if scroll root is stale on android': {skipEmit: true},
    'adopted styleSheets on firefox': {skipEmit: true},
}
