const $ = require('shelljs');
const path = require('path');
const {
    logStep,
    logInfo
} = require('../logging');

const rootDir = path.resolve(`${__dirname}/../..`)
$.config.fatal = true

$.cd(rootDir)

logStep('Build')

logInfo(`Restore NuGet packages`)
$.exec(`dotnet restore --disable-parallel --no-cache`)

logInfo(`Build solution`)
$.exec(`dotnet build --configuration Release --no-restore`)