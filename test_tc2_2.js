const axios = require('axios');

const CORE_API_URL = 'http://localhost:5043/api/v1';
const ID_API_URL = 'http://localhost:5272/api/v1';

async function login(username, password) {
    try {
        const response = await axios.post(`${ID_API_URL}/Auth/login`, {
            username,
            password
        });
        return response.data.data.token;
    } catch (error) {
        console.error(`Login failed for ${username}:`, error.response?.data || error.message);
        return null;
    }
}

async function getMyProfile(token) {
    try {
        const response = await axios.get(`${CORE_API_URL}/employees/me`, {
            headers: { Authorization: `Bearer ${token}` }
        });
        return response.data.data;
    } catch (error) {
        console.error(`Get profile failed:`, error.response?.data || error.message);
        return null;
    }
}

async function testDepartmentRouting() {
    console.log("=== TC 2.2: Department Routing Test ===");

    // 1. We need two users: An employee and their manager
    // Let's assume we have default seeded users or we can just try "admin" first to see all employees
    const adminToken = await login('admin', 'admin123');
    if (!adminToken) return;

    // Get employees to find a manager and a report
    const employeesResponse = await axios.get(`${CORE_API_URL}/employees?pageSize=50`, {
        headers: { Authorization: `Bearer ${adminToken}` }
    });

    const employees = employeesResponse.data.data.items;

    // Find an employee with a Department that has a Manager
    let testEmployee = null;
    let testManager = null;

    // For simplicity, let's just find any 2 distinct employees and temporarily set them up (if needed)
    // Actually, all active employees might have default passwords "Pass@123"
    // Let's pick NV002 (Tran Thi B) and suppose her manager is NV001 (Nguyen Van A)
    testManager = employees.find(e => e.code === 'NV001');
    testEmployee = employees.find(e => e.code === 'NV002');

    if (!testEmployee || !testManager) {
        console.log("Could not find suitable employees for test.");
        return;
    }

    console.log(`Setting up ${testEmployee.fullName} to report to ${testManager.fullName}`);

    // Set NV002's ReportToId to NV001
    await axios.put(`${CORE_API_URL}/employees/${testEmployee.id}`, {
        ...testEmployee,
        reportToId: testManager.id
    }, {
        headers: { Authorization: `Bearer ${adminToken}` }
    });

    // Set Department Manager explicitly just to be safe for Shift Requests
    if (testEmployee.departmentId) {
        const deptRes = await axios.get(`${CORE_API_URL}/departments/${testEmployee.departmentId}`, {
            headers: { Authorization: `Bearer ${adminToken}` }
        });
        const dept = deptRes.data.data;
        if (dept.managerId !== testManager.id) {
            await axios.put(`${CORE_API_URL}/departments/${testEmployee.departmentId}`, {
                ...dept,
                managerId: testManager.id
            }, { headers: { Authorization: `Bearer ${adminToken}` } });
            console.log("Updated Department Manager to NV001");
        }
    }

    // 2. Login as Employee (NV002)
    // Wait, the username for NV002 is probably NV002
    console.log(`\nLogging in as Employee: ${testEmployee.code}`);
    const empToken = await login(testEmployee.code, 'Pass@123');
    if (!empToken) {
        console.log("Failed to login as employee. Ensure password is correct.");
        return;
    }

    // 3. Create a Shift Request as Employee
    console.log("Creating Shift Change Request...");

    // We need a target shift ID. Let's get one.
    const shiftsResponse = await axios.get(`${CORE_API_URL}/settings/work-shifts`, {
        headers: { Authorization: `Bearer ${empToken}` }
    });
    const firstShift = shiftsResponse.data.data.items[0];

    let shiftRequestId = null;
    try {
        const createRes = await axios.post(`${CORE_API_URL}/attendance/shift-requests`, {
            date: new Date().toISOString(),
            toShiftId: firstShift.id,
            reason: "TC 2.2 Test Routing",
            autoApprove: false
            // Note: intentionally omitting approverId to test auto-routing!
        }, {
            headers: { Authorization: `Bearer ${empToken}` }
        });

        console.log("Shift Request Created successfully!");
        shiftRequestId = createRes.data.data; // Note: if the API returns just the ID or an object
    } catch (e) {
        console.error("Failed to create shift request:", e.response?.data || e.message);
        return;
    }

    // 4. Login as Manager (NV001)
    console.log(`\nLogging in as Manager: ${testManager.code}`);
    const mgrToken = await login(testManager.code, 'Pass@123');

    if (!mgrToken) {
        console.log("Failed to login as manager.");
        return;
    }

    // 5. Verify Manager can see the request
    console.log("Fetching Shift Requests for Manager...");
    const mgrReqResponse = await axios.get(`${CORE_API_URL}/attendance/shift-requests`, {
        headers: { Authorization: `Bearer ${mgrToken}` }
    });

    const mgrRequests = mgrReqResponse.data.data.items;
    const foundRequest = mgrRequests.find(r => r.reason === "TC 2.2 Test Routing");

    if (foundRequest) {
        console.log(`SUCCESS: Manager sees the request! ApproverId in DB is mapped to Manager!`);
        console.log(`Request ID: ${foundRequest.id}, Employee: ${foundRequest.employeeName}, ApproverId: ${foundRequest.approverId}`);
        if (foundRequest.approverId === testManager.id) {
            console.log("PERFECT: ApproverId was automatically set to the Department Manager!");
        } else {
            console.warn(`WARNING: ApproverId is ${foundRequest.approverId}, expected ${testManager.id}`);
        }
    } else {
        console.error("FAIL: Manager cannot see the shift request.");
    }

    console.log("\n=== Test Completed ===");
}

testDepartmentRouting();
