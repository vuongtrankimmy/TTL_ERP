const axios = require('axios');
const { MongoClient } = require('mongodb');

const CORE_API = 'http://localhost:5043/api/v1';
const AUTH_API = 'http://localhost:5272/api/v1';

async function verifyTC5_1() {
    console.log('--- STARTING VERIFICATION TC 5.1: Status Restriction ---');

    try {
        const loginRes = await axios.post(`${AUTH_API}/auth/login`, { username: "admin", password: "admin123" });
        const token = loginRes.data.data.accessToken;
        const authHeader = { headers: { Authorization: `Bearer ${token}` } };
        console.log("Login successful.");

        console.log("Connecting to mongo to get raw ids...");
        const client = new MongoClient('mongodb://127.0.0.1:27030');
        await client.connect();
        const db = client.db('HR');

        let employee = await db.collection('employees').findOne({ Code: 'GD-001' });
        let asset = await db.collection('assets').findOne({ Status: 'Available' });
        if (!asset) {
            console.log("Injecting temp asset...");
            await db.collection('assets').insertOne({ _id: require('mongodb').ObjectId(), Status: 'Available' });
            asset = await db.collection('assets').findOne({ Status: 'Available' });
        }

        let benefit = await db.collection('benefits').findOne();
        await client.close();

        console.log(`Using Employee: ${employee._id.toString()}`);
        console.log(`Using Asset: ${asset._id.toString()}`);
        console.log(`Using Benefit: ${benefit._id.toString()}`);

        const empId = employee._id.toString();
        const astId = asset._id.toString();
        const bnfId = benefit._id.toString();

        console.log("\n-> Setting employee to Inactive using API...");
        await axios.put(`${CORE_API}/employees/${empId}`, { ...employee, id: empId, isAccountActive: false, departureDate: new Date().toISOString() }, authHeader);

        let assetSuccess = false;
        try {
            await axios.post(`${CORE_API}/assets/allocate`, { assetId: astId, employeeId: empId, allocatedDate: new Date().toISOString() }, authHeader);
            console.log("❌ ERROR: Asset allocation succeeded for inactive employee!");
            assetSuccess = true;
        } catch (err) {
            console.log("✅ Asset allocation failed:", err.response?.data?.errors?.[0] || err.message);
        }

        let benefitSuccess = false;
        try {
            await axios.post(`${CORE_API}/benefits/assign`, { benefitId: bnfId, employeeId: empId, startDate: new Date().toISOString() }, authHeader);
            console.log("❌ ERROR: Benefit assignment succeeded for inactive employee!");
            benefitSuccess = true;
        } catch (err) {
            console.log("✅ Benefit assignment failed:", err.response?.data?.errors?.[0] || err.message);
        }

        console.log("\n-> Restoring employee to original status...");
        await axios.put(`${CORE_API}/employees/${empId}`, { ...employee, id: empId, isAccountActive: true, departureDate: null }, authHeader);

        if (!assetSuccess && !benefitSuccess) {
            console.log("\nSUCCESS: TC 5.1 Verified.");
        } else {
            console.log("\nFAILED.");
        }

    } catch (error) {
        console.error("Test error:", error.message || error);
        if (error.response) console.error(JSON.stringify(error.response.data, null, 2));
    }
}
verifyTC5_1();
