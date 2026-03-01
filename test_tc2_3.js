const axios = require('axios');
const { MongoClient, ObjectId } = require('mongodb');

const CORE_URL = 'http://localhost:5043/api/v1';
const IDENTITY_URL = 'http://localhost:5272/api/v1';
const MONGO_URI = 'mongodb://127.0.0.1:27030/';
const DB_NAME = 'HR';

const EMPLOYEE_ID = '65bf0c300000000000000001'; // Phạm Minh Hùng
const SHIFT_ID = '65ebf3a40000000000000001'; // Ca Hành Chính
const DEPT_B_ID = '65bf0a1e0000000000000002'; // Phòng Nhân Sự

async function runTest() {
    let client;
    try {
        client = await MongoClient.connect(MONGO_URI);
        const db = client.db(DB_NAME);

        console.log('--- Step 1: Pre-setup - Configure Department Default Shift ---');
        await db.collection('departments').updateOne(
            { _id: new ObjectId(DEPT_B_ID) },
            { $set: { DefaultShiftId: new ObjectId(SHIFT_ID) } }
        );

        // Reset employee to Dept A and NO shift first
        await db.collection('employees').updateOne(
            { _id: new ObjectId(EMPLOYEE_ID) },
            { $set: { DepartmentId: new ObjectId('65bf0a1e0000000000000001'), ShiftId: null } }
        );

        console.log('--- Step 2: Logging in ---');
        const loginRes = await axios.post(`${IDENTITY_URL}/Auth/login`, { username: 'admin', password: 'admin123' });
        const token = loginRes.data.data.accessToken;
        const headers = { Authorization: `Bearer ${token}` };

        // 3. Get Employee details to build update request
        console.log('--- Step 3: Fetching Employee Data ---');
        const empRes = await axios.get(`${CORE_URL}/Employees/${EMPLOYEE_ID}`, { headers });
        const employeeData = empRes.data.data;

        // 4. Update Department to Dept B
        console.log('--- Step 4: Changing Department to Dept B (Triggering Auto-Shift) ---');
        const updatePayload = {
            ...employeeData,
            departmentId: DEPT_B_ID,
            shiftId: null // We want it to be auto-assigned
        };

        await axios.put(`${CORE_URL}/Employees/${EMPLOYEE_ID}`, updatePayload, { headers });

        console.log('Waiting for DB sync...');
        await new Promise(r => setTimeout(r, 1000));

        // 5. Verification
        console.log('--- Step 5: Verifying shift auto-assignment ---');
        const updatedEmp = await db.collection('employees').findOne({ _id: new ObjectId(EMPLOYEE_ID) });

        console.log(`Current Department: ${updatedEmp.DepartmentId}`);
        console.log(`Current Shift: ${updatedEmp.ShiftId}`);

        if (updatedEmp.ShiftId && updatedEmp.ShiftId.toString() === SHIFT_ID) {
            console.log('✅ PASS: Shift auto-assigned successfully!');
        } else {
            console.log('❌ FAIL: Shift was not auto-assigned.');
        }

    } catch (e) {
        console.error('Error:', e.response?.data || e.message);
    } finally {
        if (client) await client.close();
    }
}

runTest();
