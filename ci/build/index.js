const $ = require('shelljs')
const path = require('path')
require('../logging')

$.config.fatal = true
const root = path.join(__dirname, '..', '..')
const dist_dir = path.join(root, 'dist')

const publish_script = require('./build_web_api')
const docker_script = require('./build_docker_image')

try {
    publish_script.clear()
    publish_script.build_asp_net_core_app()

    docker_script.build_image()
} catch (e) {
    console.error(e)
    process.exit(1)
}

console.info(`Build succeeded: "${path.join(dist_dir, 'app')}"`)