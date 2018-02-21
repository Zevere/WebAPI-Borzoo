const $ = require('shelljs');
const fs = require('fs');
const path = require('path');
const {
    logStep,
    logInfo
} = require('../logging');

const rootDir = path.resolve(`${__dirname}/../..`)
$.config.fatal = true
$.cd(rootDir)

logStep('Test')

logInfo('Run SQLite tests')
$.env['SQLite_Migrations_Script'] = `${rootDir}/src/Borzoo.Data.SQLite/scripts/migrations.sql`
$.cd(`${rootDir}/test/Borzoo.Data.Tests.SQLite`)
$.exec(`dotnet test --list-tests`)
$.exec(`dotnet test -c Release`)
// dotnet xunit -configuration Release -stoponfail -verbose
$.env.SQLite_Migrations_Script = ''


logInfo('Run Mongo tests')
$.cd(`${rootDir}/test/Borzoo.Data.Tests.Mongo`)
containerId = $.exec('docker run -d -p 27017:27017 mongo').stdout.trim()
$.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)
$.exec(`docker rm -f ${containerId}`)


logInfo('Run unit tests')
$.cd(`${rootDir}/test/Borzoo.Web.Tests.Unit`)
$.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)


logInfo('Run integration tests (SQLite)')
$.cd(`${rootDir}/test/Borzoo.Web.Tests.Integ`)
fs.writeFileSync('appsettings.Staging.json', JSON.stringify({
    data: {
        use: "sqlite"
    }
}, undefined, 2))
$.exec(`dotnet test --list-tests`)
$.exec(`dotnet test -c Release`)


logInfo('Run integration tests (MongoDb)')
$.cd(`${rootDir}/test/Borzoo.Web.Tests.Integ`)
containerId = $.exec('docker run -d -p 27017:27017 mongo').stdout.trim()
fs.writeFileSync(`appsettings.Staging.json`, JSON.stringify({
    "data": {
        "use": "mongo",
        "mongo": {
            "connection": "mongodb://localhost:27017/borzoo-test-integ"
        }
    }
}, undefined, 2))
$.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)
$.exec(`docker rm -f ${containerId}`)
