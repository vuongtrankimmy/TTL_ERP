const axios = require('axios');
const { MongoClient, ObjectId } = require('mongodb');

const CORE_URL = 'http://localhost:5043/api/v1';
const IDENTITY_URL = 'http://localhost:5272/api/v1';
const MONGO_URI = 'mongodb://127.0.0.1:27030/';
const DB_NAME = 'HR';

const SHIFT_ID = '65ebf3a40000000000000001'; // Ca Hành Chính
const DEPT_B_ID = '65bf0a1e0000000000000002'; // Phòng Nhân Sự

async function runTest() {
    let client;
    try {
        client = await MongoClient.connect(MONGO_URI);
        const db = client.db(DB_NAME);

        // Fetch required IDs
        const pos = await db.collection('positions').findOne({});
        const status = await db.collection('lookups').findOne({ Type: 'EmployeeStatus' });
        const contract = await db.collection('lookups').findOne({ Type: 'ContractType' });

        console.log(`Using Position ID: ${pos._id}`);
        console.log(`Using Status ID: ${status._id}`);
        console.log(`Using Contract ID: ${contract._id}`);

        console.log('--- Step 1: Pre-setup - Ensure Dept B has default shift ---');
        await db.collection('departments').updateOne(
            { _id: new ObjectId(DEPT_B_ID) },
            { $set: { DefaultShiftId: new ObjectId(SHIFT_ID) } }
        );

        console.log('--- Step 2: Logging in ---');
        const loginRes = await axios.post(`${IDENTITY_URL}/Auth/login`, { username: 'admin', password: 'admin123' });
        const token = loginRes.data.data.accessToken;
        const headers = { Authorization: `Bearer ${token}` };

        console.log('--- Step 3: Creating New Employee in Dept B ---');
        const uniqueSuffix = Date.now().toString().slice(-6);
        const createPayload = {
            fullName: `Test Auto Shift ${uniqueSuffix}`,
            email: `autoshift_${uniqueSuffix}@example.com`,
            departmentId: DEPT_B_ID,
            shiftId: null, // Should be auto-assigned
            positionId: pos._id.toString(),
            statusId: status._id.toString(),
            contractTypeId: contract._id.toString(),
            joinDate: new Date().toISOString(),
            isCreateAccount: false
        };

        const createRes = await axios.post(`${CORE_URL}/Employees`, createPayload, { headers });
        const newEmployeeId = createRes.data.data;
        console.log(`Created Employee ID: ${newEmployeeId}`);

        console.log('Waiting for DB sync...');
        await new Promise(r => setTimeout(r, 1000));

        // 4. Verification
        console.log('--- Step 4: Verifying shift auto-assignment on creation ---');
        const newEmp = await db.collection('employees').findOne({ _id: new ObjectId(newEmployeeId) });

        console.log(`Assigned Shift: ${newEmp.ShiftId}`);

        if (newEmp.ShiftId && newEmp.ShiftId.toString() === SHIFT_ID) {
            console.log('✅ PASS: Shift auto-assigned on creation successfully!');
        } else {
            console.log('❌ FAIL: Shift was not auto-assigned on creation.');
        }

        // Cleanup
        await db.collection('employees').deleteOne({ _id: new ObjectId(newEmployeeId) });

    } catch (e) {
        console.error('Error:', e.response?.data || e.message);
    } finally {
        if (client) await client.close();
    }
}

runTest();
