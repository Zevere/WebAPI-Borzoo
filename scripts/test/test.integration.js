const $ = require('shelljs');
const path = require('path');
require('../logging')

const root = path.resolve(`${__dirname}/../..`)
$.config.fatal = true

exports.run_integration_tests_using_sqlite = function () {
    console.info(`running Borzoo integration tests using SQLite`)

    /* README:
     * It is important to use "dotnet test" instead of "dotnet xunit" for testing when using SQLite.
     * I don't know the reason yet!
     */

    const commands = [
            `dotnet build --configuration Release`,
            `cd test/IntegrationTests/`,
            `dotnet test --no-build --configuration Release --verbosity normal`,
        ]
        .reduce((prev, curr) => `${prev} && ${curr}`, 'echo')

    $.exec(
        `docker run --rm --tty ` +
        `--volume "${root}:/project" ` +
        `--workdir /project/ ` +
        `microsoft/dotnet:2.1.402-sdk ` +
        `sh -c "${commands}"`
    )
}

exports.run_integration_tests_using_mongo = function () {
    console.info(`running Borzoo integration tests using Mongo`)

    console.debug('starting a Docker container for Mongo')
    const container_id = $.exec(
        `docker run --rm --detach --publish 27017:27017 --name borzoo-mongo-integration-test mongo`
    ).stdout.trim()

    const commands = [
            `dotnet build --configuration Release`,
            `cd test/IntegrationTests/`,
            `dotnet test --no-build --configuration Release --verbosity normal`,
            // `dotnet xunit -configuration Release -stoponfail -verbose --fx-version 2.1.4`
        ]
        .reduce((prev, curr) => `${prev} && ${curr}`, 'echo')

    const settings_json = JSON.stringify({
        data: {
            use: "mongo",
            mongo: {
                connection: "mongodb://borzoo-mongo-integration-test:27017/borzoo-test"
            }
        }
    })

    try {
        $.exec(
            `docker run --rm --tty ` +
            `--volume "${root}:/project" ` +
            `--workdir /project/ ` +
            `--link borzoo-mongo-integration-test ` +
            `--env 'BORZOO_TEST_SETTINGS=${settings_json}' ` +
            `microsoft/dotnet:2.1.402-sdk ` +
            `sh -c "${commands}"`
        )
    } finally {
        console.debug(`removing the Mongo container`)
        $.exec(`docker rm --volumes --force "${container_id}"`)
    }
}