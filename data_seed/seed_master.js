const { MongoClient, ObjectId } = require('mongodb');
const fs = require('fs');
const path = require('path');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const dbName = 'HR';
const client = new MongoClient(url);

const SEED_FILES = [
    { file: 'lookups.json', col: 'lookups' },
    { file: 'roles.json', col: 'roles' },
    { file: 'permissions.json', col: 'permissions' },
    { file: 'departments.json', col: 'departments' },
    { file: 'positions.json', col: 'positions' },
    { file: 'work_shifts.json', col: 'work_shifts' },
    { file: 'system_config.json', col: 'system_settings' },
    { file: 'payroll_config.json', col: 'payroll_configs' },
    { file: 'assets.json', col: 'assets' },
    { file: 'employees.json', col: 'employees' },
    { file: 'courses.json', col: 'training_courses' },
    { file: 'job_postings.json', col: 'recruitment_jobs' },
    { file: 'holidays.json', col: 'holidays' },
    { file: 'contract_templates.json', col: 'contract_templates' },
    { file: 'contracts.json', col: 'contracts' },
    { file: 'leave_types.json', col: 'leave_types' }
];

function transform(obj) {
    if (Array.isArray(obj)) {
        return obj.map(transform);
    } else if (obj !== null && typeof obj === 'object') {
        if (obj.$oid) {
            return new ObjectId(obj.$oid);
        } else if (obj.$date) {
            return new Date(obj.$date);
        } else {
            const result = {};
            for (const key in obj) {
                result[key] = transform(obj[key]);
            }
            return result;
        }
    }
    return obj;
}

async function main() {
    try {
        await client.connect();
        console.log("Connected to MongoDB for master seeding...");
        const db = client.db(dbName);

        for (const seed of SEED_FILES) {
            const filePath = path.join(__dirname, seed.file);
            if (!fs.existsSync(filePath)) {
                console.warn(`File not found: ${seed.file}, skipping...`);
                continue;
            }

            console.log(`Seeding collection: ${seed.col} from ${seed.file}...`);
            const data = JSON.parse(fs.readFileSync(filePath, 'utf8'));
            const transformedData = transform(data);

            if (transformedData.length > 0) {
                await db.collection(seed.col).deleteMany({}); // Clear existing to prevent duplicates/conflicts
                await db.collection(seed.col).insertMany(transformedData);
                console.log(`  Inserted ${transformedData.length} docs into ${seed.col}`);
            }
        }

        // Run the specific E2E test data on top
        console.log("\nRunning final E2E data alignment (init_real_data.js)...");
        require('./init_real_data.js');
        // Note: init_real_data.js might close the connection, so we trigger it carefully.
        // Actually it's better to just run it via command line after this.

        console.log("\n--- MASTER SEEDING COMPLETED ---");

    } catch (err) {
        console.error("Master Seeding Error:", err);
    } finally {
        await client.close();
    }
}

main();
