const fs = require('fs');
const token = fs.readFileSync('token.txt', 'utf8').trim();

const employeeId = "69a2dca55971fd2a1fa343b8";

async function runTest() {
    console.log("=== TC 2.1: TimekeepingCode mapping ===");

    // Step 1: Update Employee with new TimekeepingCode
    console.log("1. Updating Employee TimekeepingCode to DEV-123...");
    const updatePayload = {
        Id: employeeId,
        FullName: "Nguyễn Văn IdentityTest",
        TimekeepingCode: "DEV-123",
        Email: "updated.identity@tantanloc.com",
        CompanyEmail: "updated.identity@tantanloc.com",
        Phone: "0111222333",
        JoinDate: "2026-03-01T00:00:00Z",
        IsCreateAccount: true,
        IsAccountActive: true,
        Username: "0999888777",
        Role: "USER",
        DepartmentId: "65bf0a1e0000000000000001",
        PositionId: "65bf0b2f0000000000000001",
        StatusId: "65dae2f30000000000000501",
        ContractTypeId: "65dae2f30000000000000202",
        PersonalDetails: {
            IdCardNumber: "099988877799",
            Gender: "Nam",
            Nationality: "Việt Nam",
            Ethnicity: "Kinh",
            Religion: "Không",
            MaritalStatus: "Độc thân",
            Dependents: []
        },
        Education: [],
        Experience: []
    };

    const updateRes = await fetch(`http://localhost:5043/api/v1/employees/${employeeId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(updatePayload)
    });

    if (updateRes.status !== 200) {
        console.error("Update failed:", await updateRes.text());
        return;
    }
    console.log("-> Employee updated.");

    // Step 2: Import Attendance using TimekeepingCode
    console.log("\n2. Importing Check-in via DEV-123 TimekeepingCode...");

    const formData = new FormData();
    formData.append('RawData', JSON.stringify([
        { EmployeeCode: "DEV-123", Timestamp: "2026-02-28T01:00:00.000Z" } // 8 AM VN time
    ]));
    formData.append('IsPreviewOnly', 'false');
    formData.append('Source', 'TEST_SCRIPT');

    const importRes = await fetch(`http://localhost:5043/api/v1/attendance/import`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`
            // Don't set Content-Type! FormData will generate the boundary automatically
        },
        body: formData
    });

    const importData = await importRes.json();
    console.log(`-> Import Response (${importRes.status}):`, JSON.stringify(importData, null, 2));

    if (importData.success && importData.data.successCount > 0) {
        // Step 3: Verify the Attendance record in Mongo
        const { exec } = require('child_process');
        exec(`mongosh "mongodb://localhost:27030/HR" --eval "db.attendances.find({EmployeeId: '${employeeId}', Date: ISODate('2026-02-27T17:00:00Z')}, {EmployeeName: 1, EmployeeCode: 1, Method: 1, CheckIn: 1}).toArray()"`, (err, stdout, stderr) => {
            console.log("\n>>> Checking HR.attendances");
            console.log(stdout);
            if (stderr) console.error(stderr);
        });
    } else {
        console.error("-> Import failed or no records imported.");
    }
}

runTest();
