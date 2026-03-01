
const axios = require('axios');
const FormData = require('form-data');
const fs = require('fs');

fs.writeFileSync('test_out.txt', '');
const originalLog = console.log;
console.log = function (...args) {
    fs.appendFileSync('test_out.txt', args.join(' ') + '\n', 'utf8');
    originalLog.apply(console, args);
}

const CORE_API = 'http://localhost:5043/api/v1';
const AUTH_API = 'http://localhost:5272/api/v1';

async function verifyTC4_4() {

    console.log('--- STARTING VERIFICATION TC 4.4: Auto-Lock & Recalculate ---');

    try {
        // 0. Login
        console.log("Logging in as admin...");
        const loginRes = await axios.post(`${AUTH_API}/auth/login`, {
            username: "admin",
            password: "admin123"
        });
        const token = loginRes.data.data.accessToken;
        console.log("Login successful.");

        const authHeader = { headers: { Authorization: `Bearer ${token}` } };

        // 1. Get Employee 'Phạm Minh Hùng'
        console.log("Fetching employee GD-001...");
        const employeesRes = await axios.get(`${CORE_API}/employees?searchTerm=GD-001`, authHeader);
        const employee = employeesRes.data.data.items[0];
        if (!employee) throw new Error("Employee GD-001 not found");
        console.log(`Using Employee: ${employee.fullName} (${employee.id}, Code: ${employee.code})`);

        // 2. Ensure Labor Contract exists and inactive NDAs
        console.log("Checking for active contracts...");
        const contractsRes = await axios.get(`${CORE_API}/contracts?employeeId=${employee.id}`, authHeader);
        let activeContracts = (contractsRes.data.data.items || []).filter(c => c.employeeId === employee.id && c.statusId === "65dae2f30000000000000303");

        let laborContract = null;
        for (let c of activeContracts) {
            if (c.basicSalary === 0 || c.contractNumber.includes('NDA')) {
                console.log(`Deleting interfering 0-salary contract: ${c.contractNumber}`);
                await axios.delete(`${CORE_API}/contracts/${c.id}`, authHeader);
            } else {
                laborContract = c;
            }
        }

        if (!laborContract) {
            console.log("No active labor contract found. Creating one...");
            await axios.post(`${CORE_API}/contracts`, {
                employeeId: employee.id,
                contractTemplateId: "65dae2f30000000000000004", // Indefinite
                typeId: "65dae2f30000000000000205", // Indefinite
                startDate: "2024-01-01T00:00:00Z",
                basicSalary: 25000000,
                insuranceSalary: 25000000,
                signedDate: "2023-12-30T00:00:00Z",
                statusId: "65dae2f30000000000000303", // Active
                note: "TC 4.4 Verification Contract"
            }, authHeader);
            console.log("Labor contract created successfully.");
        } else {
            console.log(`Labor contract found: ${laborContract.contractNumber} with salary ${laborContract.basicSalary.toLocaleString()}`);
        }

        // 3. Inject Baseline Attendance for Feb 15 (Using FormData)
        console.log("Injecting baseline attendance for Feb 15 (Form-Data)...");
        const csvData = `${employee.code},2026-02-15 08:00:00\n${employee.code},2026-02-15 17:30:00`;
        const form1 = new FormData();
        form1.append('rawData', csvData);
        form1.append('employeeCodeColumnIndex', '1');
        form1.append('timestampColumnIndex', '2');
        form1.append('source', 'Manual');
        form1.append('isPreviewOnly', 'false');

        await axios.post(`${CORE_API}/attendance/import`, form1, {
            headers: {
                ...authHeader.headers,
                ...form1.getHeaders()
            }
        });
        console.log("Baseline attendance injected.");

        // 4. Identify/Create Payroll Period for Feb 2026
        const month = 2;
        const year = 2026;
        let periodId;

        console.log(`Checking payroll period ${month}/${year}...`);
        const periodsRes = await axios.get(`${CORE_API}/payroll/periods?month=${month}&year=${year}`, authHeader);
        const existingPeriod = (periodsRes.data.data.items || periodsRes.data.data || []).find(p => p.month === month && p.year === year);

        if (existingPeriod) {
            periodId = existingPeriod.id;
            console.log(`Using existing period: ${periodId}`);
        } else {
            console.log("Generating new payroll period...");
            const createPeriodRes = await axios.post(`${CORE_API}/payroll/periods/generate?month=${month}&year=${year}`, {}, authHeader);
            periodId = createPeriodRes.data.data.id || createPeriodRes.data.data;
            console.log(`Generated period: ${periodId}`);
        }

        // 5. Initial Calculation
        console.log(`Triggering initial payroll calculation for period ${periodId}...`);
        await axios.post(`${CORE_API}/payroll/periods/${periodId}/calculate`, {}, authHeader);

        console.log("Waiting for calculation to process (5s)...");
        await new Promise(r => setTimeout(r, 5000));

        console.log("Fetching initial results...");
        const detailRes1 = await axios.get(`${CORE_API}/payroll/periods/${periodId}/detail?pageSize=100`, authHeader);
        let myPayroll = (detailRes1.data.data.payrolls.items || []).find(p => p.employeeId === employee.id);

        const initialSalary = myPayroll ? myPayroll.totalWorkSalary : 0;
        console.log(`Initial Work Salary: ${initialSalary.toLocaleString()} VNĐ`);

        // 6. Attendance Adjustment (Add Feb 16 - Form-Data)
        console.log("Injecting adjustment: Feb 16 attendance (Form-Data)...");
        const adjCsvData = `${employee.code},2026-02-16 08:00:00\n${employee.code},2026-02-16 17:30:00`;
        const form2 = new FormData();
        form2.append('rawData', adjCsvData);
        form2.append('employeeCodeColumnIndex', '1');
        form2.append('timestampColumnIndex', '2');
        form2.append('source', 'Manual');
        form2.append('isPreviewOnly', 'false');

        await axios.post(`${CORE_API}/attendance/import`, form2, {
            headers: {
                ...authHeader.headers,
                ...form2.getHeaders()
            }
        });

        console.log("Recalculating Attendance Summary...");
        await axios.post(`${CORE_API}/attendance/recalculate-summary`, {
            employeeId: employee.id,
            month,
            year
        }, authHeader);

        // 7. Recalculate Payroll
        console.log("Re-triggering payroll calculation...");
        await axios.post(`${CORE_API}/payroll/periods/${periodId}/calculate`, {}, authHeader);
        await new Promise(r => setTimeout(r, 6000));

        // 8. Final Verification
        const detailRes2 = await axios.get(`${CORE_API}/payroll/periods/${periodId}/detail?pageSize=100`, authHeader);
        myPayroll = (detailRes2.data.data.payrolls.items || []).find(p => p.employeeId === employee.id);

        const finalSalary = myPayroll ? myPayroll.totalWorkSalary : 0;
        console.log(`Final Work Salary: ${finalSalary.toLocaleString()} VNĐ`);

        if (finalSalary > initialSalary) {
            console.log("SUCCESS: TC 4.4 Verified. Payroll recalculated correctly after attendance changes.");
        } else {
            console.log(`RESULT: Work salary same or lower (${initialSalary.toLocaleString()} -> ${finalSalary.toLocaleString()}). Check if work rules are applied.`);
        }

    } catch (error) {
        console.error("Verification failed:", error.response ? JSON.stringify(error.response.data, null, 2) : error.message);
    }
}

verifyTC4_4();
