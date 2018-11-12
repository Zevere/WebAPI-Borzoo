const $ = require('shelljs')
require('../logging')

$.config.fatal = true

try {
    require('./borzoo.unit.test')
    require('./mongo.integration.test')
    require('./webapp.integration.test')
} catch (e) {
    console.error(`❎ AN UNEXPECTED ERROR OCURRED`)
    console.error(e)
    process.exit(1)
}
