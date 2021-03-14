module.exports = {
	// api
	//unknown issue
	'should extract text from regions': { skipEmit: true },   //test not implemented yet. It exists for JS only now
	'should extract text regions from image': {skipEmit: true},   // Not implemented yet
	// location
	// 'should send dom and location when check window': { skipEmit: true },
	'should send dom and location when check window with vg': { skipEmit: true },
	// 'should send dom and location when check window fully': { skipEmit: true },
	// 'should send dom and location when check window fully with vg': { skipEmit: true },
	// 'should send dom and location when check frame': { skipEmit: true },
	// 'should send dom and location when check frame with vg': { skipEmit: true },
	// 'should send dom and location when check frame fully': { skipEmit: true },
	// 'should send dom and location when check frame fully with vg': { skipEmit: true },
	// 'should send dom and location when check region by selector': { skipEmit: true },
	// 'should send dom and location when check region by selector with vg': { skipEmit: true },
	// 'should send dom and location when check region by selector fully': { skipEmit: true },
	// 'should send dom and location when check region by selector fully with vg': { skipEmit: true },
	//'should send dom and location when check region by selector in frame': { skipEmit: true },
	// 'should send dom and location when check region by selector with custom scroll root': { skipEmit: true },
	// 'should send dom and location when check region by selector with custom scroll root with vg': { skipEmit: true },
  'should send dom and location when check region by selector fully with custom scroll root': { skipEmit: true }, // test is wrong!
	'should send dom and location when check region by selector fully with custom scroll root with vg': { skipEmit: true }, // test is wrong!
	// 'should send dom of version 11': { skipEmit: true },
	'should not fail if scroll root is stale on android': {skipEmit: true},
	'check region by selector in frame fully on firefox legacy': { skipEmit: true },
	'should send custom batch properties': {skipEmit: true},
	'adopted styleSheets on chrome': {skipEmit: true},
	'adopted styleSheets on firefox': {skipEmit: true},
}
