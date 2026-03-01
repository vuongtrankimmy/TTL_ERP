const fs = require('fs');
const token = fs.readFileSync('token.txt', 'utf8').trim();

const payload = {
    FullName: "Nguyễn Văn IdentityTest",
    Email: "test.identity@tantanloc.com",
    CompanyEmail: "test.identity@tantanloc.com",
    Phone: "0999888777",
    JoinDate: "2026-03-01T00:00:00Z",
    IsCreateAccount: true,
    IsAccountActive: true,
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

fetch('http://localhost:5043/api/v1/employees', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(payload)
})
    .then(res => res.json().then(data => ({ status: res.status, body: data })).catch(() => ({ status: res.status, body: 'No JSON body' })))
    .then(({ status, body }) => {
        console.log(`API Response (${status}):`, JSON.stringify(body, null, 2));

        // Check MongoDB
        if (status === 200 || status === 201) {
            const { exec } = require('child_process');
            exec(`mongosh "mongodb://localhost:27030/TTL_Identity_DB" --eval "db.Users.find({Email: 'test.identity@tantanloc.com'}, {PasswordHash: 0}).toArray()"`, (err, stdout, stderr) => {
                console.log("\n>>> Checking TTL_Identity_DB.Users for new account");
                console.log(stdout);
                if (stderr) console.error(stderr);
            });
        }
    })
    .catch(err => console.error(err));
