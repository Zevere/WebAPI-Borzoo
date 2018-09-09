const path = require('path')
require('../logging')

const root = path.join(__dirname, '..', '..')
const dist_dir = path.join(root, 'dist')


function get_environment_name() {
    console.info('Verifying environment name')

    const environment_name = process.argv[1]
    if (arg && arg.length) {
        console.debug(`Deploying to environment "${environment_name}".`)
        return environment_name
    } else {
        throw `No environment name is passed.\n` +
            `\tExample: node ci/deploy Staging`
    }
}

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


console.info('Choosing deployment strategy')
try {
    const environment_name = get_environment_name()
    const heroku_options = verify_heroku_env_vars(environment_name)
    if (heroku_options) {
        console.debug('deploying to Heroku')
        require('./deploy-heroku')
    } else {
        console.error('No deployment option is defined')
        process.exit(1)
    }
} catch (e) {
    console.error(e)
    process.exit(1)
}