const $ = require('shelljs')
require('../logging')

$.config.fatal = true
const root = `${__dirname}/../..`


module.exports.build_image = function () {
    const image_name = 'webapi-borzoo:latest'
    console.info(`building Docker Image "${image_name}"`)

    console.debug('copying SQLite migrations file')
    $.cp(`${root}/src/Borzoo.Data.SQLite/scripts/migrations.sql`, `${root}/dist/`)

    console.debug('copying Dockerfile')
    $.cp(`${root}/scripts/build/borzoo-dev.Dockerfile`, `${root}/dist/Dockerfile`)

    console.debug('building docker image')
    $.exec(`docker build -t ${image_name} ${root}/dist/`)
}
