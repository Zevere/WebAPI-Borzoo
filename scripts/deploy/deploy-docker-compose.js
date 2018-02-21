const $ = require('shelljs');
const fs = require('fs');
const path = require('path');
const {
    logInfo,
    logWarn
} = require('../logging');

$.config.fatal = true
const rootDir = path.resolve(`${__dirname}/../..`)


function tryLoadConfigs() {
    logInfo('Load deployment configurations')
    const configsFile = `${rootDir}/scripts/deploy/deploy-docker-compose-configs.json`
    if (!fs.existsSync(configsFile)) {
        logWarn(`Deployment configurations not found`)
        return
    }
    const configs = JSON.parse(fs.readFileSync(configsFile))

    $.env['DOCKER_HOST'] = configs.docker.host
    $.env['DOCKER_CERT_PATH'] = configs.docker.certsDir
    $.env['DOCKER_TLS_VERIFY'] = 1

    if (configs.app && configs.app.env) {
        
    }
}


function checkDockerConnection() {
    logInfo('Check Docker connection')
    if ($.exec(`docker version --format '{{.Server.Version}}'`).code != 0) {
        throw "Unable to connect to Docker daemon"
    }

    logInfo('Check Docker Compose installation')
    if ($.exec('docker-compose version').code != 0) {
        throw "Docker Compose is not installed"
    }
}


function publishWebApp() {
    logInfo('Publish app')
    $.cd(`${rootDir}/src/Borzoo.Web`)
    const publishDir = `${rootDir}/src/Borzoo.Web/bin/publish/Release`
    $.rm(`${publishDir}/*`)
    $.exec(`dotnet publish --configuration Release --output "${publishDir}"`)
}


function publishDockerContainers() {
    logInfo('Remove Docker compose containers')
    $.cd(`${rootDir}/scripts/deploy`)
    $.exec(`docker-compose rm -f`)


    logInfo('Build Docker compose containers')
    $.cp([
        `${rootDir}/src/Borzoo.Data.SQLite/scripts/migrations.sql`,
        `${rootDir}/scripts/deploy/Borzoo.Web.Dockerfile`
    ], `${rootDir}/src/Borzoo.Web/bin/publish/`)
    $.exec('docker-compose build --force-rm --no-cache')


    logInfo('Start Docker compose containers')
    $.exec('docker-compose up -d')
}

tryLoadConfigs()
checkDockerConnection()
publishWebApp()
publishDockerContainers()