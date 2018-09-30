const $ = require('shelljs');
const {
    logInfo
} = require('../logging');

module.exports.usingMongoContainer = function (f) {
    const containerId = $.exec('docker run --rm --detach --publish 27017:27017 mongo').stdout.trim()
    logInfo(`Running "mongo" container: ${containerId}`)

    try {
        f()
    } finally {
        $.exec(`docker stop ${containerId}`)
        logInfo(`Stopped "mongo" container: ${containerId}`)
    }

}