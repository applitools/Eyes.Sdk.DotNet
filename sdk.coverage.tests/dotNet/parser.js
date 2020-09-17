'use strict'

function checkSettings(cs) {
    let target = `Target`
    if(cs === undefined){
        return target + '.Window()'
    }
    let element = ''
    let options = ''
    if (cs.frames === undefined && cs.region === undefined) element = '.Window()'
    else {
        if (cs.frames) element += frames(cs.frames)
        if (cs.region) element += region(cs.region)
    }
    if(cs.ignoreRegions) options += ignoreRegions(cs.ignoreRegions)
    if(cs.isFully) options += '.Fully()'
    return target + element + options
}

function frames(arr) {
    return arr.reduce((acc, val) => acc + `.Frame(\"${getVal(val)}\")`, '')
}

function region(region) {
    return `.Region(${regionParameter(region)})`
}

function ignoreRegions(arr) {
    return arr.reduce((acc, val) => acc + ignore(val), '')
}

function ignore(region){
    return `.Ignore(${regionParameter(region)})`
}

function regionParameter (region) {
    let string
    switch (typeof region) {
        case 'string':
            string = `By.CssSelector(\"${region}\")`
            break;
        case "object":
            string = `new Rectangle(${region.left}, ${region.top}, ${region.width}, ${region.height})`
    }
    return string
}

function getVal (val) {
    let nameAndValue = val.toString().split("\"")
    return nameAndValue[1]
}

module.exports = {
    checkSettingsParser: checkSettings
}