const $ = require('shelljs');
const path = require('path');
require('../logging')

$.config.fatal = true
const root = path.resolve(`${__dirname}/../..`)

const image = process.env.image_tag || 'borzoo:solution'
console.info(`RUNNING BORZOO UNIT TESTS USING DOCKER IMAGE "${image}"`)

try {
    $.exec(
        `docker run --rm --tty ` +
        `--workdir /project/test/UnitTests/ ` +
        `"${image}" ` +
        `dotnet test --no-build --configuration Release --verbosity normal`
    )
} catch (e) {
    console.error(`❎ AN UNEXPECTED ERROR OCURRED`)
    console.error(e)
    process.exit(1)
}

console.info(`✅ UNIT TESTS PASSED`)
