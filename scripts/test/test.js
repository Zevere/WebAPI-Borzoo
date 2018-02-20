const $ = require('shelljs');
const {
    logStep,
    logInfo
} = require('../logging');

const rootDir = `${__dirname}/../..`
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
const containerId = $.exec('docker run -d -p 27017:27017 mongo').stdout.trim()
$.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)
$.exec(`docker rm -f ${containerId}`)


logInfo('Run unit tests')
$.cd(`${rootDir}/test/Borzoo.Web.Tests.Unit`)
$.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)


logInfo('Run integration tests')
$.cd(`${rootDir}/test/Borzoo.Web.Tests.Integ`)
$.exec(`dotnet test --list-tests`)
$.exec(`dotnet test -c Release`)
// dotnet xunit -configuration Release -stoponfail -verbose
