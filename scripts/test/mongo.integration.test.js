const $ = require('shelljs');
const path = require('path');
require('../logging')

$.config.fatal = true
const root = path.resolve(`${__dirname}/../..`)

const image = process.env.image_tag || 'borzoo:solution'
console.info(`RUNNING MONGODB INTEGRATION TESTS USING DOCKER IMAGE "${image}"`)

try {
    console.debug('starting test dependencies. docker-compose project: "mongotests"')
    $.cd(`${root}/test/MongoTests`)
    $.exec(`docker-compose --project-name mongotests up -d --force-recreate --remove-orphans`)

    try {
        console.debug('running tests')

        const settings = JSON.stringify(JSON.stringify({
            ConnectionString: "mongodb://mongo/borzoo-mongo-tests"
        }))

        $.exec(
            `docker run --rm --tty ` +
            `--workdir /project/test/MongoTests/ ` +
            `--env "BORZOO_TEST_SETTINGS=${settings}" ` +
            `--network mongotests_borzoo-network ` +
            `"${image}" ` +
            `dotnet test --no-build --configuration Release --verbosity normal`
        )
    } finally {
        console.debug('removing test dependency containers via docker-compose')
        $.exec(`docker-compose --project-name mongotests rm -fv`)
    }
} catch (e) {
    console.error(`❎ AN UNEXPECTED ERROR OCURRED`)
    console.error(e)
    process.exit(1)
}

console.info(`✅ MONGODB INTEGRATION TESTS PASSED`)
