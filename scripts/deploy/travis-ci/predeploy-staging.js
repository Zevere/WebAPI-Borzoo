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


logInfo('Read Docker configurations')
const dockerHost = $.env['BORZOO_DOCKER_HOST']
if (!dockerHost) {
  throw 'Environment variable "BORZOO_DOCKER_HOST" is not set.'
}

if (!$.env['BORZOO_DOCKER_CERTS']) {
  throw 'Environment variable "BORZOO_DOCKER_CERTS" is not set.'
}
const dockerCerts = JSON.parse($.env['BORZOO_DOCKER_CERTS'])

const dockerCertsDir = `${rootDir}/scripts/deploy/docker-certs`
if (!fs.existsSync(dockerCertsDir)) {
  $.mkdir(dockerCertsDir)
}

logInfo(`Write Docker certificates to: ${dockerCertsDir}`)
fs.writeFileSync(dockerCertsDir + "/ca.pem", dockerCerts.ca);
fs.writeFileSync(dockerCertsDir + "/cert.pem", dockerCerts.cert);
fs.writeFileSync(dockerCertsDir + "/key.pem", dockerCerts.key);


logInfo(`Write Docker certificates to: ${dockerCertsDir}`)
const configs = {
  dockerHost: dockerHost,
  dockerCertsDir: dockerCertsDir
}
fs.writeFileSync(`${rootDir}/scripts/deploy/deploy-docker-compose-configs.json`, JSON.stringify(configs, undefined, 2))