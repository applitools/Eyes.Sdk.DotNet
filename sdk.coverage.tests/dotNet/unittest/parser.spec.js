'use strict'
const assert = require('assert')
const {checkSettingsParser} = require('../parser')

describe('Check settings parser tests', () => {

    it('Window', () => {
        assert.deepStrictEqual(checkSettingsParser(undefined), `Target.Window()`)
    })

    it('Window fully', () => {
        assert.deepStrictEqual(checkSettingsParser({isFully:true}), `Target.Window().Fully()`)
    })

    it('Region element', () => {
        assert.deepStrictEqual(checkSettingsParser({region:'#name'}), `Target.Region(By.CssSelector("#name"))`)
    })

    it('Region rectangle', () => {
        assert.deepStrictEqual(checkSettingsParser({region: {left: 10, top: 20, width: 30, height: 40}}), `Target.Region(new Rectangle(10, 20, 30, 40))`)
    })

    it('Frames 1', () => {
        assert.deepStrictEqual(checkSettingsParser({frames: ['[name="frame1"]']}), `Target.Frame("frame1")`)
    })

    it('Frames 2', () => {
        assert.deepStrictEqual(checkSettingsParser({frames: ['[name="frame1"]', '[name="frame2"]']}), `Target.Frame("frame1").Frame("frame2")`)
    })

    it('Region in frame', () => {
        assert.deepStrictEqual(checkSettingsParser({frames: ['[name="frame1"]'], region: '#name'}), `Target.Frame("frame1").Region(By.CssSelector("#name"))`)
    })

    it('Ignore region', () => {
        assert.deepStrictEqual(checkSettingsParser({ignoreRegions: ['#name']}), `Target.Window().Ignore(By.CssSelector("#name"))`)
    })


})