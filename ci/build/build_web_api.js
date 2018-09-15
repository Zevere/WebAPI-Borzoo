const $ = require('shelljs')
const path = require('path')
require('../logging')

$.config.fatal = true
const root = path.join(__dirname, '..', '..')


module.exports.clear = function () {
    console.info('Clearing dist directory')

    $.rm('-rf', `${root}/dist`)
    $.mkdir('-p', `${root}/dist/app/`)
}

module.exports.build_asp_net_core_app = function () {
    console.info('Building ASP.NET Core web app')

    console.debug('Publishing web app project')
    $.exec(
        `docker run --rm ` +
        `--volume "${root}/src:/project/src" ` +
        `--volume "${root}/deps:/project/deps" ` +
        `--volume "${root}/dist/app:/app" ` +
        `--workdir /project/src/Borzoo.Web/ ` +
        `microsoft/dotnet:2.1.402-sdk ` +
        `dotnet publish --configuration Release --output /app/`
    )
}
