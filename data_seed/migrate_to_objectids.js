const { MongoClient, ObjectId } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const dbName = 'HR';
const client = new MongoClient(url);

async function migrate() {
    try {
        await client.connect();
        const db = client.db(dbName);
        console.log("Connected to MongoDB for ObjectId Migration...");

        const collections = [
            {
                name: 'lookups',
                fields: [] // Only _id is ObjectId here usually, which is already handled
            },
            {
                name: 'lookups_translate',
                fields: ['LookupId']
            },
            {
                name: 'contract_templates',
                fields: ['StatusId', 'TypeId']
            },
            {
                name: 'contract_templates_translate',
                fields: ['ContractTemplateId']
            },
            {
                name: 'contracts',
                fields: ['EmployeeId', 'ContractTemplateId', 'TypeId', 'StatusId']
            }
        ];

        for (const colInfo of collections) {
            const collection = db.collection(colInfo.name);
            console.log(`\nProcessing collection: ${colInfo.name}`);

            const docs = await collection.find({}).toArray();
            let updatedCount = 0;

            for (const doc of docs) {
                const updates = {};
                let hasChanges = false;

                for (const field of colInfo.fields) {
                    const value = doc[field];
                    if (typeof value === 'string' && value.length === 24 && /^[0-9a-fA-F]+$/.test(value)) {
                        updates[field] = new ObjectId(value);
                        hasChanges = true;
                    }
                }

                if (hasChanges) {
                    await collection.updateOne(
                        { _id: doc._id },
                        { $set: updates }
                    );
                    updatedCount++;
                }
            }
            console.log(`  Updated ${updatedCount} documents in ${colInfo.name}`);
        }

        console.log("\n--- MIGRATION COMPLETED ---");

    } catch (err) {
        console.error("Migration Error:", err);
    } finally {
        await client.close();
    }
}

migrate();
