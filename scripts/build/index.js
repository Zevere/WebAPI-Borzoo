const $ = require('shelljs')
const path = require('path')
require('../logging')

$.config.fatal = true
const root = path.resolve(`${__dirname}/../..`)

function get_docker_options() {
    const settings_script = require('./../deploy/deploy_settings')
    let settings
    try {
        settings = settings_script.get_deployment_settings()
    } catch (e) {
        return
    }

    let docker_options;
    for (const prop in settings) {
        for (const deployment of settings[prop]) {
            if (deployment.type === 'docker') {
                docker_options = deployment.options;
            }
        }
    }
    return docker_options
}

try {
    console.info(`building Docker images`)

    $.cd(root)

    console.debug('building the solution with "Release" configuration and "borzoo:solution" tag')
    $.exec(
        `docker build --tag borzoo:solution --no-cache --target solution-build --build-arg "configuration=Release" .`
    )

    console.debug('building the final web app with "borzoo:latest" tag')
    $.exec(`docker build --tag borzoo --target final .`)


    console.debug('reading Docker deployment options')
    docker_options = get_docker_options()
    if (docker_options) {
        console.debug('pushing images to the Docker hub')
        const docker_deploy_script = require('./../deploy/deploy_docker_registry')

        docker_deploy_script.deploy(
            'borzoo:solution', 'zevere/webapi-borzoo:unstable-solution', docker_options.user, docker_options.pass
        )

        docker_deploy_script.deploy(
            'borzoo:latest', 'zevere/webapi-borzoo:unstable', docker_options.user, docker_options.pass
        )
    } else {
        console.warn('Docker deployment options not found. skipping Docker image push...')
    }
} catch (e) {
    console.error(e)
    process.exit(1)
}

console.info(`✅ Build succeeded`)