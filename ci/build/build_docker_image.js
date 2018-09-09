const $ = require('shelljs')
require('../logging')

$.config.fatal = true
const root = `${__dirname}/../..`


module.exports.build_image = function () {
    console.info('Build Docker Image')

    console.debug('copying SQLite migrations file')
    $.cp(`${root}/src/Borzoo.Data.SQLite/scripts/migrations.sql`, `${root}/dist/`)

    console.debug('copying Dockerfile')
    $.cp(`${root}/ci/build/borzoo-dev.Dockerfile`, `${root}/dist/Dockerfile`)

    console.debug('building docker image')
    $.exec(`docker build -t webapi-borzoo:latest ${root}/dist/`)
}
