const { MongoClient } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const dbName = 'HR';
const client = new MongoClient(url);

async function main() {
    try {
        await client.connect();
        const db = client.db(dbName);
        const count = await db.collection('attendances').countDocuments();
        console.log(`Total attendance records: ${count}`);
    } finally {
        await client.close();
    }
}

main();
