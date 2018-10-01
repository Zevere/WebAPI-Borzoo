const $ = require('shelljs');
const path = require('path');
require('../logging')

const root = path.resolve(`${__dirname}/../..`)
$.config.fatal = true

exports.run_borzoo_unit_tests = function () {
    console.info(`running Borzoo unit tests`)

    const commands = [
            `dotnet build`,
            `dotnet xunit -configuration Release -stoponfail -verbose --fx-version 2.1.4`
        ]
        .reduce((prev, curr) => `${prev} && ${curr}`, 'echo')

    $.exec(
        `docker run --rm --tty ` +
        `--volume "${root}:/project" ` +
        `--workdir /project/test/UnitTests/ ` +
        `microsoft/dotnet:2.1.402-sdk ` +
        `sh -c "${commands}"`
    )
}