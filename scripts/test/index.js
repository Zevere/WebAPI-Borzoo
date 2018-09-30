const $ = require('shelljs')
require('../logging')

$.config.fatal = true

try {
    const unit_tests = require('./test.unit')
    unit_tests.run_sqlite_data_tests()

} catch (e) {
    console.error(e)
    process.exit(1)
}