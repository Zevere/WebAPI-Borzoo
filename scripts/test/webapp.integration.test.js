const $ = require('shelljs');
const path = require('path');
require('../logging')

$.config.fatal = true
const root = path.resolve(`${__dirname}/../..`)

const image = process.env.image_tag || 'borzoo:solution'
console.info(`RUNNING WEB APP INTEGRATION TESTS USING DOCKER IMAGE "${image}"`)

try {
    console.debug('starting test dependencies. docker-compose project: "webapptests"')
    $.cd(`${root}/test/WebAppTests`)
    $.exec(`docker-compose --project-name webapptests up -d --force-recreate --remove-orphans`)

    try {
        console.debug('running tests')

        const settings = JSON.stringify(JSON.stringify({
            Mongo: {
                ConnectionString: "mongodb://mongo/busvbot-telegram-tests"
            }
        }))

        $.exec(
            `docker run --rm --tty ` +
            `--workdir /project/test/WebAppTests/ ` +
            `--env "BORZOO_SETTINGS=${settings}" ` +
            `--network webapptests_borzoo-network ` +
            `"${image}" ` +
            `dotnet test --no-build --configuration Release --verbosity normal`
        )
    } finally {
        console.debug('removing test dependency containers via docker-compose')
        $.exec(`docker-compose --project-name webapptests rm --stop -fv`)
    }
} catch (e) {
    console.error(`❎ AN UNEXPECTED ERROR OCURRED`)
    console.error(e)
    process.exit(1)
}

console.info(`✅ WEB APP INTEGRATION TESTS PASSED`)
