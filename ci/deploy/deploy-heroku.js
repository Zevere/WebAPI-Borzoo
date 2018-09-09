const $ = require('shelljs')
const path = require('path')
require('../logging')

$.config.fatal = true
const deploy_dir = __dirname
const dist_dir = path.join(deploy_dir, '/../../dist')
let heroku_app_name;


// function verify_settings() {
//     console.info('Verifying settings...')
//     const script = require('./verify-settings')
//     try {
//         script.verifyDistDirectory()
//         heroku_app_name = script.verify_heroku_app()
//         script.verifyDockerConnection()
//     } catch (e) {
//         console.error('Settings are invalid!')
//         console.error(e)
//         process.exit(1)
//     }
// }

// function read_app_settings() {
//     console.info('Reading app settings...')
//     const script = require('./verify-settings')
//     try {
//         script.verifyAppSettings(`${dist_dir}/app/appsettings.Production.json`)
//     } catch (e) {
//         console.error('App settings are invalid!')
//         console.error(e)
//         process.exit(1)
//     }
// }

function build_docker_image() {
    console.info('Building Docker image...')

    $.cp(`${deploy_dir}/Heroku.Dockerfile`, `${dist_dir}/Dockerfile`)
    $.exec(`docker build --tag app ${dist_dir}`)
}

function push_image_to_heroku() {
    console.info('Pushing Docker image to Heroku...')

    console.debug('connecting to Heroku Docker registry')
    $.exec(`docker login --username "$HEROKU_DOCKER_USERNAME" --password "$HEROKU_AUTH_TOKEN" registry.heroku.com`)

    console.debug('tagging the image...')
    $.exec(`docker tag app:latest registry.heroku.com/${heroku_app_name}/web`)

    console.debug('pushing the image...')
    $.exec(`docker push registry.heroku.com/${heroku_app_name}/web`)
}

function release_heroku_app() {
    console.info('Deploying the image to Heroku dyno...')

    console.debug(`Getting docker image ID`)
    const image_id = $.exec(`docker inspect app:latest --format={{.Id}}`).stdout.trim()

    console.debug(`Upgrading to new release`)
    const post_data = JSON.stringify({
        updates: [{
            type: 'web',
            docker_image: image_id
        }]
    })
    $.exec(
        `curl -X PATCH "https://api.heroku.com/apps/${heroku_app_name}/formation" ` +
        `-H "Authorization: Bearer $HEROKU_AUTH_TOKEN" ` +
        `-H 'Content-Type: application/json' ` +
        `-H 'Accept: application/vnd.heroku+json; version=3.docker-releases' ` +
        `-d '${post_data}'`
    )
}


// verify_settings()
// read_app_settings()
build_docker_image()
// push_image_to_heroku()
// release_heroku_app()

console.info("App is deployed successfully")