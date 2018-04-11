const $ = require('shelljs');
const {
    logInfo
} = require('../logging');

module.exports.usingMongoContainer = function (f) {
    const containerId = $.exec('docker run -d -p 27017:27017 mongo').stdout.trim()
    logInfo(`Running "mongo" container: ${containerId}`)
    
    // Delay for 3 seconds until Mongo is up and ready
    setTimeout(() => {
        try {
            f()
        } finally {
            $.exec(`docker rm -f ${containerId}`)
            logInfo(`Stopped "mongo" container: ${containerId}`)
        }
    }, 3000)

}