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

logInfo('Run SQLite tests'); {
    $.env['SQLite_Migrations_Script'] = `${rootDir}/src/Borzoo.Data.SQLite/scripts/migrations.sql`
    $.cd(`${rootDir}/test/Borzoo.Data.Tests.SQLite`)
    $.exec(`dotnet test --list-tests`)
    $.exec(`dotnet test -c Release`)
    // dotnet xunit -configuration Release -stoponfail -verbose
    $.env.SQLite_Migrations_Script = ''
}


logInfo('Run Mongo tests'); {
    $.cd(`${rootDir}/test/Borzoo.Data.Tests.Mongo`)
    usingMongoContainer(() => {
        $.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)
    })
}


logInfo('Run unit tests'); {
    $.cd(`${rootDir}/test/Borzoo.Web.Tests.Unit`)
    $.exec(`dotnet xunit -configuration Release -stoponfail -verbose`)
}