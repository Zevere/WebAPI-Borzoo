const $ = require('shelljs')
require('../logging')

$.config.fatal = true

try {
    const data_integration_tests = require('./test.integration.data')
    data_integration_tests.run_sqlite_data_tests()
    data_integration_tests.run_mongo_data_tests()

    const unit_tests = require('./test.unit')
    unit_tests.run_borzoo_unit_tests()
} catch (e) {
    console.error(e)
    process.exit(1)
}