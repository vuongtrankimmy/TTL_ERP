const crypto = require('crypto');

function base64url(source) {
    let encodedSource = Buffer.isBuffer(source) ? source.toString('base64') : Buffer.from(source).toString('base64');
    return encodedSource.replace(/=+$/, '').replace(/\+/g, '-').replace(/\//g, '_');
}

const header = { alg: 'HS256', typ: 'JWT' };
const payload = {
    sub: '65bf0c300000000000000001',
    email: 'hung.pham@orga-x.com',
    name: 'Phạm Minh Hùng',
    roles: ['ADMIN'],
    iss: 'https://auth.tantanloc.com',
    aud: 'https://gateway.tantanloc.com',
    exp: Math.floor(Date.now() / 1000) + 3600,
    iat: Math.floor(Date.now() / 1000)
};

const encodedHeader = base64url(JSON.stringify(header));
const encodedPayload = base64url(JSON.stringify(payload));
const unsignedToken = `${encodedHeader}.${encodedPayload}`;

const secret = Buffer.from('xJudGcymwyz9Ho24IJZe18gZd423of+cnIDKoFpg9B3pJ9zv+FdkFd6+HEv0pGW6PGxVrqThOy+oXXRAFnu/CA==', 'base64');
const signature = crypto.createHmac('sha256', secret).update(unsignedToken).digest();
const encodedSignature = base64url(signature);

const jwt = `${unsignedToken}.${encodedSignature}`;
require('fs').writeFileSync('token.txt', jwt);
console.log('Token written to token.txt');
