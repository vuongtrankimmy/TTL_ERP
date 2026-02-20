
// Connect to HR DB
db = db.getSiblingDB("HR");

// Departments
var deptId = new ObjectId("65d48721fabcde0001000000");
try {
    db.departments.deleteOne({ _id: deptId });
} catch (e) { }
db.departments.insertOne({
    _id: deptId,
    Name: "Engineering",
    Code: "ENG",
    CreatedAt: new Date()
});

// Positions
var posId1 = new ObjectId("65d48721fabcde0002000001");
try {
    db.positions.deleteOne({ _id: posId1 });
} catch (e) { }
db.positions.insertOne({
    _id: posId1,
    Name: "Software Engineer",
    Code: "SWE",
    DepartmentId: deptId, // Assuming DeptId in Position is properly referenced
    CreatedAt: new Date()
});

// Employees
var employees = [];
var statuses = ["Active", "Probation", "MaternityLeave", "Resigned", "Terminated", "Offboarding", "Inactive"];

for (var i = 1; i <= 100; i++) {
    var code = "EMP-2" + (i < 10 ? "00" + i : (i < 100 ? "0" + i : i)); // EMP-2001 to EMP-2100
    // Fix code logic: i=1 -> 001. i=10 -> 010. i=100 -> 100.
    // Range 2001 to 2100.
    // If i=1, suffix=001 => EMP-2001.
    // If i=100, suffix=100 => EMP-2100.

    var status = statuses[i % statuses.length];

    employees.push({
        Code: code,
        FullName: "Nguyễn Văn " + i,
        Email: "emp" + i + "@tantanloc.com",
        CompanyEmail: "emp" + i + "@tantanloc.com",
        Phone: "090000" + (1000 + i),
        JoinDate: new Date("2023-01-01T08:00:00Z"),
        PersonalDetails: {
            DOB: new Date("1990-01-01T00:00:00Z"),
            Gender: i % 2 == 0 ? "Nam" : "Nữ",
            Nationality: "Việt Nam",
            Ethnicity: "Kinh"
        },
        DepartmentId: deptId,
        PositionId: posId1,
        Status: status,
        AvatarUrl: "",
        Roles: [],
        IsAccountActive: true,
        CreatedAt: new Date()
    });
}

// Clean up old
db.employees.deleteMany({ Code: /^EMP-2/ });
db.employees.deleteMany({ Code: 'EMP-TEST-MONGO' });
db.employees.deleteMany({ Code: 'EMP-2001' });

// Insert new
var res = db.employees.insertMany(employees);
print("Inserted " + res.insertedIds.length + " employees");
