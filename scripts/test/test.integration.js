const $ = require('shelljs');
const fs = require('fs');
const path = require('path');
const {
    logStep,
    logInfo
} = require('../logging');
const usingMongoContainer = require('./container-provider').usingMongoContainer

const rootDir = path.resolve(`${__dirname}/../..`)
$.config.fatal = true
$.cd(rootDir)

logStep('Test')

logInfo('Run integration tests (SQLite)'); {
    $.cd(`${rootDir}/test/Borzoo.Web.Tests.Integ`)
    fs.writeFileSync('appsettings.Staging.json', JSON.stringify({
        data: {
            use: "sqlite"
        }
    }, undefined, 2))
    $.exec(`dotnet test --list-tests`)
    $.exec(`dotnet test -c Release`)
}


logInfo('Run integration tests (MongoDb)'); {
    $.cd(`${rootDir}/test/Borzoo.Web.Tests.Integ`)
    usingMongoContainer(() => {

        fs.writeFileSync(`appsettings.Staging.json`, JSON.stringify({
            "data": {
                "use": "mongo",
                "mongo": {
                    "connection": "mongodb://localhost:27017/borzoo-test-integ"
                }
            }
        }, undefined, 2))
        $.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)

    })
}