const fs = require('fs');
const crypto = require('crypto');

function generateObjectId() {
    const timestamp = Math.floor(Date.now() / 1000).toString(16);
    const randomHex = crypto.randomBytes(8).toString('hex');
    return timestamp + randomHex;
}

const file = 'd:/MONEY/2026/TAN_TAN_LOC/TTL_ERP/data_seed/holidays.json';
let data = JSON.parse(fs.readFileSync(file, 'utf8'));

data = data.map(h => {
    delete h.Id;
    h._id = { "$oid": generateObjectId() };
    return h;
});

fs.writeFileSync(file, JSON.stringify(data, null, 2));
console.log('Fixed IDs');
