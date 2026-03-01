const fs = require('fs');
const token = fs.readFileSync('token.txt', 'utf8').trim();

// The previously created Employee ID
const employeeId = "69a2dca55971fd2a1fa343b8";

const updatePayload = {
    Id: employeeId,
    FullName: "Nguyễn Văn IdentityTest",
    Email: "updated.identity@tantanloc.com",
    CompanyEmail: "updated.identity@tantanloc.com",
    Phone: "0111222333",
    JoinDate: "2026-03-01T00:00:00Z",
    IsCreateAccount: true, // Must be true for sync to trigger
    IsAccountActive: false, // Testing Deactivation
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
        MaritalStatus: "Độc thân"
    }
};

fetch(`http://localhost:5043/api/v1/employees/${employeeId}`, {
    method: 'PUT',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(updatePayload)
})
    .then(res => res.json().then(data => ({ status: res.status, body: data })).catch(() => ({ status: res.status, body: 'No JSON body' })))
    .then(({ status, body }) => {
        console.log(`API Response (${status}):`, JSON.stringify(body, null, 2));

        // Check MongoDB Identity
        if (status === 200 || status === 204) {
            const { exec } = require('child_process');
            exec(`mongosh "mongodb://localhost:27030/TTL_Identity_DB" --eval "db.Users.find({EmployeeId: ObjectId('${employeeId}')}, {PasswordHash: 0, ConfirmationToken: 0}).toArray()"`, (err, stdout, stderr) => {
                console.log("\n>>> Checking TTL_Identity_DB.Users after Update");
                console.log(stdout);
                if (stderr) console.error(stderr);
            });
        }
    })
    .catch(err => console.error(err));
