const { MongoClient, ObjectId } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const dbName = 'HR';
const client = new MongoClient(url);

async function main() {
    try {
        await client.connect();
        const db = client.db(dbName);
        console.log("Connected to MongoDB for Deep Fix...");

        // --- 1. Align Lookups ---
        console.log("\nAligning Lookups...");
        const lookups = await db.collection('lookups').find({}).toArray();
        const lookupMap = {}; // Code_Type -> ID
        lookups.forEach(l => {
            lookupMap[`${l.Code}_${l.Type}`] = l._id.toString();
        });

        const lookupTranslations = await db.collection('lookups_translate').find({}).toArray();
        let lookupUpdates = 0;

        for (const trans of lookupTranslations) {
            // We need to find which lookup this translation belongs to.
            // Since the current LookupId might be wrong, we need to match by something else.
            // In our case, the lookups_translate doesn't have Code/Type.
            // BUT, we can try to find if the current LookupId matches ANY lookup.
            const currentLookup = lookups.find(l => l._id.toString() === trans.LookupId);
            if (!currentLookup) {
                // If it doesn't match any lookup, it's a "ghost" translation from a previous seed.
                // We should probably delete it or try to find a match by Name.
                // To be safe, let's just log it for now.
                console.log(`  Ghost lookup translation found: ${trans.Name} (ID: ${trans.LookupId})`);
            }
        }

        // Actually, the best way to align is to RE-SEED the translations using the CURRENT lookup IDs.
        // I already did this with seed_contract_lookups_translate.js for contract types.
        // Let's do it for EVERYTHING.

        // --- 2. Align Contract Templates ---
        console.log("\nAligning Contract Templates...");
        const templates = await db.collection('contract_templates').find({}).toArray();
        const templateTranslations = await db.collection('contract_templates_translate').find({}).toArray();

        for (const trans of templateTranslations) {
            const currentTemplate = templates.find(t => t._id.toString() === trans.ContractTemplateId);
            if (!currentTemplate) {
                // Ghost template translation. Let's try to match by Name.
                const matchedTemplate = templates.find(t => t.Name === trans.Name || t.Code === trans.Name); // Name in trans might match Code or Name in template
                if (matchedTemplate) {
                    await db.collection('contract_templates_translate').updateOne(
                        { _id: trans._id },
                        { $set: { ContractTemplateId: matchedTemplate._id.toString() } }
                    );
                    console.log(`  Aligned template translation: ${trans.Name} -> ${matchedTemplate.Code}`);
                } else {
                    console.log(`  Could not find template match for translation: ${trans.Name}`);
                }
            }
        }

        // --- 3. Fix most common issue: VN_STANDARD ---
        const vnTemplate = templates.find(t => t.Code === 'VN_STANDARD');
        if (vnTemplate) {
            await db.collection('contract_templates_translate').updateMany(
                { Name: "Hợp đồng lao động tiêu chuẩn Việt Nam" },
                { $set: { ContractTemplateId: vnTemplate._id.toString() } }
            );
            console.log("  Forced alignment for VN_STANDARD template.");
        }

        console.log("\n--- DEEP FIX COMPLETED ---");

    } catch (err) {
        console.error("Deep Fix Error:", err);
    } finally {
        await client.close();
    }
}

main();
