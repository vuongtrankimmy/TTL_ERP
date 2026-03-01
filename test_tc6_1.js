const axios = require('axios');
const { MongoClient, ObjectId } = require('mongodb');

const CORE_API = 'http://localhost:5043/api/v1';
const AUTH_API = 'http://localhost:5272/api/v1';

async function verifyTC6_1() {
    console.log('--- STARTING VERIFICATION TC 6.1: Sensitive Field Log ---');

    let client;
    try {
        const loginRes = await axios.post(`${AUTH_API}/auth/login`, { username: "admin", password: "admin123" });
        const token = loginRes.data.data.accessToken;
        const authHeader = { headers: { Authorization: `Bearer ${token}` } };
        console.log("Login successful.");

        // 1. Get Employee
        let employeesRes = await axios.get(`${CORE_API}/employees?searchTerm=GD-001`, authHeader);
        let employee = employeesRes.data.data.items[0];
        if (!employee) throw new Error("Employee GD-001 not found");

        console.log(`Using Employee: ${employee.fullName} (${employee.id})`);

        // Cache original values
        const originalSalary = employee.salary;
        const originalBankName = employee.personalDetails?.bankName;

        const newSalary = (originalSalary || 0) + 1000000;
        const newBankName = "Test Bank " + Date.now();

        // 2. Update Employee with new sensitive values
        console.log(`\n-> Updating Employee Salary to ${newSalary} and Bank to ${newBankName}...`);

        const updatePayload = {
            ...employee,
            salary: newSalary,
            personalDetails: {
                ...employee.personalDetails,
                bankName: newBankName
            }
        };

        await axios.put(`${CORE_API}/employees/${employee.id}`, updatePayload, authHeader);
        console.log("Employee updated via API.");

        // Wait a brief moment for async handlers or DB write
        await new Promise(r => setTimeout(r, 1000));

        // 3. Query MongoDB for AuditLog
        console.log("\n-> Querying AuditLogs collection in MongoDB...");
        client = new MongoClient('mongodb://127.0.0.1:27030');
        await client.connect();
        const db = client.db('HR');

        // Find the latest audit log for this employee
        const latestLogs = await db.collection('audit_logs')
            .find({ EntityId: new ObjectId(employee.id), EntityName: "Employee", Action: "Update" })
            .sort({ CreatedAt: -1 })
            .limit(5)
            .toArray();

        if (latestLogs.length > 0) {
            console.log(`\n✅ SUCCESS: Found ${latestLogs.length} Audit Log records!`);

            for (let i = 0; i < latestLogs.length; i++) {
                const log = latestLogs[i];
                console.log(`\n--- Log #${i + 1} ---`);
                console.log(`- CreatedAt: ${log.CreatedAt}`);
                console.log(`- Raw OldValues:`);
                console.log(JSON.parse(log.OldValues));
                console.log(`- Raw NewValues:`);
                console.log(JSON.parse(log.NewValues));
            }

            const targetedLog = latestLogs.find(l => {
                const nv = JSON.parse(l.NewValues || "{}");
                return (nv.Salary === newSalary || nv.salary === newSalary) &&
                    (nv.BankName === newBankName || nv.bankName === newBankName) &&
                    Object.keys(nv).length < 10;
            });

            if (targetedLog) {
                console.log("\n✅ TC 6.1 Verified. Sensitive fields accurately logged and values match expectations.");
            } else {
                console.log("\n❌ FAILED. Could not find the targeted manual audit log matching the updated values.");
            }

        } else {
            console.log("❌ FAILED: No audit logs found for this update.");
        }

        // 4. Restore original values
        console.log("\n-> Restoring original Employee values...");
        const restorePayload = {
            ...employee,
            salary: originalSalary,
            personalDetails: {
                ...employee.personalDetails,
                bankName: originalBankName
            }
        };
        await axios.put(`${CORE_API}/employees/${employee.id}`, restorePayload, authHeader);
        console.log("Employee restored.");

    } catch (error) {
        console.error("Test error:", error.message || error);
        if (error.response) console.error(JSON.stringify(error.response.data, null, 2));
    } finally {
        if (client) await client.close();
    }
}

verifyTC6_1();
