const $ = require('shelljs');
const path = require('path');
require('../logging')

const root = path.resolve(`${__dirname}/../..`)
$.config.fatal = true

exports.run_sqlite_data_tests = function () {
    console.info(`running SQLite data tests`)

    /* README:
     * It is important to use "dotnet test" instead of "dotnet xunit" for testing when using SQLite.
     * I don't know the reason yet!
     */

    const commands = [
            `dotnet build`,
            `cd test/SQLiteTests/`,
            `dotnet test --no-build --verbosity normal`
        ]
        .reduce((prev, curr) => `${prev} && ${curr}`, 'echo')

    $.exec(
        `docker run --rm --tty ` +
        `--volume "${root}:/project" ` +
        `--workdir /project/ ` +
        `--env "SQLite_Migrations_Script=/project/src/Borzoo.Data.SQLite/scripts/migrations.sql" ` +
        `microsoft/dotnet:2.1.402-sdk ` +
        `sh -c "${commands}"`
    )
}

exports.run_mongo_data_tests = function () {
    console.info(`running Mongo DB data tests`)

    console.debug('starting a Docker container for Mongo')
    const container_id = $.exec(`docker run --rm --detach --publish 27017:27017 --name borzoo-mongo-test mongo`).stdout.trim()

    const commands = [
            `dotnet build`,
            `dotnet xunit -configuration Release -stoponfail -verbose --fx-version 2.1.4`
        ]
        .reduce((prev, curr) => `${prev} && ${curr}`, 'echo')

    try {
        $.exec(
            `docker run --rm --tty ` +
            `--volume "${root}:/project" ` +
            `--workdir /project/test/Borzoo.Data.Tests.Mongo/ ` +
            `--link borzoo-mongo-test ` +
            `--env "MONGO_CONNECTION=mongodb://borzoo-mongo-test:27017/borzoo-test" ` +
            `microsoft/dotnet:2.1.402-sdk ` +
            `sh -c "${commands}"`
        )
    } finally {
        console.debug(`removing the Mongo container`)
        $.exec(`docker rm --volumes --force "${container_id}"`)
    }
}