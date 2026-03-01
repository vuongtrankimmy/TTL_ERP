const { MongoClient, ObjectId } = require('mongodb');

async function run() {
    const client = new MongoClient('mongodb://127.0.0.1:27030');
    try {
        await client.connect();
        console.log("Connected to MongoDB.");
        const db = client.db('HR');
        const res = await db.collection('contracts').deleteMany({
            _id: { $in: [new ObjectId('65dae2f30000000000000102'), new ObjectId('69a3033179786592c5f2e3ed')] }
        });
        console.log("Deleted interfering contracts count:", res.deletedCount);
    } catch (err) {
        console.error("Error cleaning up contracts:", err);
    } finally {
        await client.close();
    }
}

run();
