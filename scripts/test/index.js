const $ = require('shelljs')
require('../logging')

$.config.fatal = true

try {
    require('./mongo.integration.test')
    // const data_integration_tests = require('./test.integration.data')
    // data_integration_tests.run_sqlite_data_tests()
    // data_integration_tests.run_mongo_data_tests()

    // const unit_tests = require('./test.unit')
    // unit_tests.run_borzoo_unit_tests()

    // const integration_tests = require('./test.integration')
    // integration_tests.run_integration_tests_using_sqlite()
    // integration_tests.run_integration_tests_using_mongo()
} catch (e) {
    console.error(`❎ AN UNEXPECTED ERROR OCURRED`)
    console.error(e)
    process.exit(1)
}
