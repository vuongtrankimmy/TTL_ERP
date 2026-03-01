const axios = require('axios');
const { MongoClient } = require('mongodb');

const CORE_API_URL = 'http://localhost:5043/api/v1';
const ID_API_URL = 'http://localhost:5272/api/v1';
const MONGO_URI = 'mongodb://127.0.0.1:27030/';

async function login(username, password) {
    try {
        const response = await axios.post(`${ID_API_URL}/Auth/login`, { username, password });
        return response.data.data.token;
    } catch (error) {
        console.error(`Login failed for ${username}:`, error.response?.data || error.message);
        return null;
    }
}

async function testLeaveAccrual() {
    console.log("=== TC 3.1: Auto Leave Accrual Test ===");

    // 1. Connect to MongoDB to set up two test employees
    const client = new MongoClient(MONGO_URI);
    await client.connect();
    const db = client.db('TTL_Core_DB');
    const employeesCol = db.collection('employees');
    const balancesCol = db.collection('employee_leave_balances');

    // Find two active employees
    let emps = await employeesCol.find({ IsDeleted: { $ne: true } }).limit(2).toArray();
    if (emps.length < 2) {
        console.log("No employees found. Creating two dummy employees...");
        const res = await employeesCol.insertMany([
            { Code: "DUMMY01", FullName: "Past Employee", Email: "past@test.com", IsDeleted: false, StatusId: null, JoinDate: new Date("2020-01-01T00:00:00Z") },
            { Code: "DUMMY02", FullName: "Future Employee", Email: "future@test.com", IsDeleted: false, StatusId: null, JoinDate: new Date("2030-01-01T00:00:00Z") }
        ]);
        emps = await employeesCol.find({ _id: { $in: Object.values(res.insertedIds) } }).toArray();
    }

    const pastEmp = emps[0];
    const futureEmp = emps[1];

    // Set JoinDate for pastEmp to 1 year ago
    const pastDate = new Date();
    pastDate.setFullYear(pastDate.getFullYear() - 1);
    await employeesCol.updateOne({ _id: pastEmp._id }, { $set: { JoinDate: pastDate } });

    // Set JoinDate for futureEmp to 1 month in the future
    const futureDate = new Date();
    futureDate.setMonth(futureDate.getMonth() + 1);
    await employeesCol.updateOne({ _id: futureEmp._id }, { $set: { JoinDate: futureDate } });

    // Clear existing leave balances for these two to start fresh
    const currentYear = new Date().getFullYear();
    await balancesCol.deleteMany({
        EmployeeId: { $in: [pastEmp._id.toString(), futureEmp._id.toString()] },
        Year: currentYear
    });

    console.log(`Setup: Set ${pastEmp.FullName}'s JoinDate to past (${pastDate.toISOString()})`);
    console.log(`Setup: Set ${futureEmp.FullName}'s JoinDate to future (${futureDate.toISOString()})`);

    // 2. Login as admin
    const adminToken = await login('admin', 'admin123');
    if (!adminToken) {
        await client.close();
        return;
    }

    // 3. Trigger AccrueLeave API
    console.log("Triggering AccrueLeave API...");
    try {
        const res = await axios.post(`${CORE_API_URL}/employees/accrue-leave`, {
            year: currentYear
        }, {
            headers: { Authorization: `Bearer ${adminToken}` }
        });
        console.log("AccrueLeave result:", res.data.message);
    } catch (err) {
        console.error("AccrueLeave API failed:", err.response?.data || err.message);
    }

    // 4. Verify in MongoDB
    console.log("Verifying balances...");
    const pastBalance = await balancesCol.findOne({ EmployeeId: pastEmp._id.toString(), Year: currentYear });
    const futureBalance = await balancesCol.findOne({ EmployeeId: futureEmp._id.toString(), Year: currentYear });

    if (pastBalance && pastBalance.TotalEntitled > 0) {
        console.log(`✅ SUCCESS: Past employee (${pastEmp.FullName}) received ${pastBalance.TotalEntitled} day(s) of accrued leave.`);
    } else {
        console.log(`❌ FAIL: Past employee (${pastEmp.FullName}) did not receive accrued leave. (Found: ${pastBalance?.TotalEntitled || 0})`);
    }

    if (!futureBalance || futureBalance.TotalEntitled === 0) {
        console.log(`✅ SUCCESS: Future employee (${futureEmp.FullName}) correctly did NOT receive accrued leave.`);
    } else {
        console.log(`❌ FAIL: Future employee (${futureEmp.FullName}) received ${futureBalance.TotalEntitled} day(s) incorrectly!`);
    }

    await client.close();
}

testLeaveAccrual().catch(console.error);
