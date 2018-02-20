const $ = require('shelljs');
const {
    logStep,
    logInfo
} = require('../logging');

const rootDir = `${__dirname}/../..`
$.cd(rootDir)

logStep('Build')

logInfo(`Restore NuGet packages`)
$.exec(`dotnet restore --disable-parallel --no-cache`)

logInfo(`Build solution`)
$.exec(`dotnet build --configuration Release --no-restore`)