const fs = require('fs');

const certs = {};
[
    ['ca.pem', 'ca'],
    ['cert.pem', 'cert'], 
    ['key.pem', 'key']
].forEach(tuple => {
    certs[tuple[1]] = fs.readFileSync(tuple[0], 'utf8');
}
);

fs.writeFileSync('docker-certs.json', JSON.stringify(certs));
