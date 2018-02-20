const chalk = require('chalk');

exports.logStep = m => console.log(chalk.white.bgMagenta.bold(`\n# Step: ${m}\n`))
exports.logInfo = m => console.log(chalk.green.bold(`\n## ${m}\n`))
exports.logWarn = m => console.log(chalk.yellow.bold(`\n## ${m}\n`))
exports.logError = m => console.log(chalk.red.bold(`\n## ${m}\n`))
