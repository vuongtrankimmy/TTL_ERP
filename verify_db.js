const { MongoClient } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const dbName = 'HR';
const client = new MongoClient(url);

async function main() {
    try {
        await client.connect();
        console.log("Connected successfully to MongoDB");
        const db = client.db(dbName);

        const collections = ['system_settings', 'departments', 'employees', 'attendance_logs', 'leave_requests', 'assets', 'attendance_summaries', 'payrolls', 'payroll_periods'];

        console.log("\n--- DATABASE COLLECTION SUMMARY ---");
        for (const colName of collections) {
            const count = await db.collection(colName).countDocuments();
            console.log(`${colName}: ${count} docs`);

            if (count > 0 && (colName === 'employees' || colName === 'leave_requests')) {
                const samples = await db.collection(colName).find({}).limit(1).toArray();
                console.log(`  Sample ${colName} ID: ${samples[0]._id}`);
            }
        }
        console.log("-----------------------------------\n");

    } catch (err) {
        console.error("Error connecting to MongoDB:", err);
    } finally {
        await client.close();
    }
}

main();
