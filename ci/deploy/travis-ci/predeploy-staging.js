const $ = require('shelljs');
const fs = require('fs');
const path = require('path');
const {
  logStep,
  logInfo
} = require('../../logging');

$.config.fatal = true
const rootDir = path.resolve(`${__dirname}/../../..`)

logStep('Pre-Deploy: Staging')


logInfo('Read Docker settings')
if (!$.env['BORZOO_DOCKER_SETTINGS']) {
  throw 'Environment variable "BORZOO_DOCKER_SETTINGS" is not set.'
}
const dockerSettings = JSON.parse($.env['BORZOO_DOCKER_SETTINGS'])
const dockerCertsDir = `${rootDir}/scripts/deploy/docker-certs`

logInfo('Read app settings')
if (!$.env['BORZOO_APP_SETTINGS']) {
  throw 'Environment variable "BORZOO_APP_SETTINGS" is not set.'
}
const appSettings = JSON.parse($.env['BORZOO_APP_SETTINGS'])

logInfo(`Write Docker certificates to: ${dockerCertsDir}`)
if (!fs.existsSync(dockerCertsDir)) {
  $.mkdir(dockerCertsDir)
}
fs.writeFileSync(dockerCertsDir + "/ca.pem", dockerSettings.ca)
fs.writeFileSync(dockerCertsDir + "/cert.pem", dockerSettings.cert)
fs.writeFileSync(dockerCertsDir + "/key.pem", dockerSettings.key)

logInfo(`Write appsettings.json`)
fs.writeFileSync(`${rootDir}/scripts/deploy/appsettings.json`, JSON.stringify(appSettings.settings, undefined, 2))

logInfo(`Write Docker Compose deployment settings`)
const configs = {
  app: appSettings,
  docker: {
    host: dockerSettings.host,
    certsDir: dockerCertsDir
  }
}
fs.writeFileSync(`${rootDir}/scripts/deploy/deploy-docker-compose-configs.json`, JSON.stringify(configs, undefined, 2))