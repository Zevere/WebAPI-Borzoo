const $ = require('shelljs');
const path = require('path');
require('../logging')
// const usingMongoContainer = require('./container-provider').usingMongoContainer

const root = path.resolve(`${__dirname}/../..`)
$.config.fatal = true

exports.run_sqlite_data_tests = function () {
    console.info(`running SQLite data tests`)

    /* README:
     * It is important to use "dotnet test" instead of "dotnet xunit" for testing.
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


// logInfo('Run Mongo tests'); {
//     $.cd(`${root}/test/Borzoo.Data.Tests.Mongo`)
//     usingMongoContainer(() => {
//         $.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)
//     })
// }


// logInfo('Run unit tests'); {
//     $.cd(`${root}/test/Borzoo.Web.Tests.Unit`)
//     $.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)
// }