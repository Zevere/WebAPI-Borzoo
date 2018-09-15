const path = require('path')
require('../logging')

const root = path.join(__dirname, '..', '..')


function get_environment_name() {
    console.info('verifying environment name')

    const environment_name = process.argv[2]
    if (environment_name && environment_name.length) {
        console.debug(`environment is "${environment_name}".`)
        return environment_name
    } else {
        throw `No environment name is passed.\n` +
        `\tExample: node ci/deploy Staging`
    }
}

function get_deployments_for_env(environment_name) {
    console.info(`finding deployments for environment "${environment_name}".`)

    const jsonValue = process.env['DEPLOY_SETTINGS_JSON']
    let deployment_map;
    try {
        deployment_map = JSON.parse(jsonValue)
    } catch (e) {
        throw `Value of "DEPLOY_SETTINGS_JSON" environment variable is not valid JSON.`
    }

    const env_deployments = deployment_map[environment_name];
    if (!env_deployments) {
        throw `There are no field for environemnt "${environment_name}" in "DEPLOY_SETTINGS_JSON" value.`
    }
    if (!(Array.isArray(env_deployments) && env_deployments.length)) {
        console.warn(`There are deployments specified for environemnt "${environment_name}".`)
    }

    console.debug(`"${env_deployments.length || 0}" deployments found.`)

    return env_deployments
}

function deploy(environment_name, deployoment) {
    console.info(`deploying to "${deployoment.type}" for environment "${environment_name}".`)
    const docker = require('./deploy_docker_registry')
    // const heorku = require('./deploy-heroku')

    if (deployoment.type === 'docker') {
        docker.deploy(
            deployoment.options.source,
            deployoment.options.target,
            deployoment.options.user,
            deployoment.options.pass
        )
    } else if (deployoment.type === 'heroku') {

    } else {
        throw `Invalid deployment type "${deployoment.type}".`
    }
}

/*
function verify_heroku_env_vars(environment_name) {
    const heroku_env_vars = [
        process.env['HEROKU_APP_NAME_JSON'],
        process.env['HEROKU_USERNAME'],
        process.env['HEROKU_AUTH_TOKEN'],
    ]

    const are_all_heroku_vars_set = heroku_env_vars.every(v => v && v.length)
    if (!are_all_heroku_vars_set && heroku_env_vars.some(v => !!v)) {
        throw `All of the Heroku environment variables must be set.`
    } else {
        return
    }

    let app_name_map;
    try {
        app_name_map = JSON.parse(heroku_env_vars[0])
    } catch (e) {
        throw `Value of "HEROKU_APP_NAME_JSON" environment variable is not valid JSON.`
    }

    const heroku_app_name = app_name_map[environment_name]
    if (heroku_app_name && heroku_app_name.length) {

    } else {
        throw `Heroku app name is not specified for environment "${environment_name}".\n` +
            `\tExample: HEROKU_APP_NAME_JSON='{"${environment_name}":"foo"}'`
    }

    return {
        app: heroku_app_name,
        email: heroku_env_vars[1],
        token: heroku_env_vars[2],
    }
}

function deploy_heroku(options) {
    console.info('deploying to Heroku')

    const heroku_env_vars = [
        process.env['HEROKU_APP_NAME_JSON'],
        process.env['HEROKU_USERNAME'],
        process.env['HEROKU_AUTH_TOKEN'],
    ]

    const are_all_heroku_vars_set = heroku_env_vars.every(v => v)
    if (!are_all_heroku_vars_set && heroku_env_vars.some(v => !!v)) {
        throw `All of the Heroku environment variables must be set.`
    } else {
        return
    }

    let app_name_map;
    try {
        app_name_map = JSON.parse(heroku_env_vars[0])
    } catch (e) {
        throw `Value of "HEROKU_APP_NAME_JSON" environment variable is not valid JSON.`
    }

    return {
        map: app_name_map,
        email: heroku_env_vars[1],
        token: heroku_env_vars[2],
    }
}
*/

try {
    const environment_name = get_environment_name()
    get_deployments_for_env(environment_name)
        .forEach(d => deploy(environment_name, d))
} catch (e) {
    console.error(e)
    process.exit(1)
}