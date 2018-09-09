const fs = require('fs');

const options = JSON.parse(process.env['BORZOO_DOCKER_CERTS']);
const certsDir = process.env['DOCKER_CERT_PATH'];

fs.writeFile(certsDir + "/ca.pem", options.ca);
fs.writeFile(certsDir + "/cert.pem", options.cert);
fs.writeFile(certsDir + "/key.pem", options.key);
