const { MongoClient, ObjectId } = require('mongodb');

// Connection details from existing seeds
const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const dbName = 'HR';
const client = new MongoClient(url);

async function main() {
    try {
        await client.connect();
        console.log("Connected to MongoDB");
        const db = client.db(dbName);

        // 1. System Settings
        console.log("Setting system config...");
        const settingsCol = db.collection('system_settings');
        await settingsCol.updateOne(
            { _id: "65bf0a1e0000000000000999" },
            { 
                $set: {
                    BaseSalary: 2340000,
                    MinimumSalaryV1: 4680000,
                    TaxExemptionDefault: 4400000,
                    CompanyName: "CÔNG TY TNHH TÂN TẤN LỘC",
                    UpdatedAt: new Date()
                }
            },
            { upsert: true }
        );

        // 2. Departments (Using existing IDs if possible or creating specific for E2E)
        const deptCol = db.collection('departments');
        const itDeptId = new ObjectId("65bf0a1e0000000000000003"); // From departments.json
        const hrDeptId = new ObjectId("65bf0a1e0000000000000002");

        // 3. Employees
        const empCol = db.collection('employees');
        
        // Employee 1: IT Manager (Approver)
        const itMgrId = new ObjectId("65bf0c300000000000000101");
        await empCol.updateOne(
            { _id: itMgrId },
            {
                $set: {
                    Code: "IT-MGR-01",
                    FullName: "Trần Thế Anh (Manager)",
                    Email: "it.manager@tantanloc.com",
                    CompanyEmail: "it.manager@tantanloc.com",
                    Phone: "0901234567",
                    DepartmentId: itDeptId.toString(),
                    PositionId: "65bf0b2f0000000000000003", // Position: DeptManager
                    StatusId: 1, // Status: Active
                    JoinDate: new Date("2020-01-01"),
                    Salary: 55000000,
                    Roles: ["65fc5b5b0000000000000005"], // Dept Manager role
                    IsAccountActive: true,
                    IsDeleted: false,
                    PersonalDetails: {
                        Gender: "Male",
                        IdCardNumber: "001080000999",
                        TaxCode: "8000000001",
                        DependentsCount: 2
                    }
                }
            },
            { upsert: true }
        );

        // Employee 2: IT Staff (Requester)
        const itStaffId = new ObjectId("65bf0c300000000000000102");
        await empCol.updateOne(
            { _id: itStaffId },
            {
                $set: {
                    Code: "IT-STF-01",
                    FullName: "Lê Văn Tuấn (Staff)",
                    Email: "it.staff@tantanloc.com",
                    CompanyEmail: "it.staff@tantanloc.com",
                    Phone: "0907654321",
                    DepartmentId: itDeptId.toString(),
                    PositionId: "65bf0b2f0000000000000004", // Position: IT Staff
                    ReportToId: itMgrId.toString(),
                    StatusId: 1,
                    JoinDate: new Date("2024-01-01"),
                    Salary: 20000000,
                    Roles: ["65fc5b5b0000000000000004"], // Employee role
                    IsAccountActive: true,
                    IsDeleted: false,
                    PersonalDetails: {
                        Gender: "Male",
                        IdCardNumber: "001090000888",
                        TaxCode: "8000000002",
                        DependentsCount: 0
                    }
                }
            },
            { upsert: true }
        );

        // Employee 3: Accountant (Transition Test)
        const accId = new ObjectId("65bf0c300000000000000103");
        await empCol.updateOne(
            { _id: accId },
            {
                $set: {
                    Code: "ACC-01",
                    FullName: "Nguyễn Kim Ngân (Acc)",
                    Email: "accountant@tantanloc.com",
                    CompanyEmail: "accountant@tantanloc.com",
                    Phone: "0908887776",
                    DepartmentId: hrDeptId.toString(),
                    PositionId: "65bf0b2f0000000000000002", 
                    StatusId: 1,
                    JoinDate: new Date("2022-06-01"),
                    Salary: 15000000,
                    IsAccountActive: true,
                    IsDeleted: false
                }
            },
            { upsert: true }
        );

        // 4. Attendance Logs for IT Staff
        console.log("Seeding attendance logs...");
        const attendanceCol = db.collection('attendance_logs');
        await attendanceCol.deleteMany({ EmployeeId: itStaffId });
        
        const logs = [
            {
                EmployeeId: itStaffId,
                LogTime: new Date("2026-03-01T08:00:00Z"), // Day 1: On-time
                DeviceCode: "MOCK_DEVICE",
                Type: "CheckIn"
            },
            {
                EmployeeId: itStaffId,
                LogTime: new Date("2026-03-01T17:00:00Z"), 
                DeviceCode: "MOCK_DEVICE",
                Type: "CheckOut"
            },
            {
                EmployeeId: itStaffId,
                LogTime: new Date("2026-03-02T08:35:00Z"), // Day 2: Late 35m
                DeviceCode: "MOCK_DEVICE",
                Type: "CheckIn"
            },
            {
                EmployeeId: itStaffId,
                LogTime: new Date("2026-03-02T17:05:00Z"), 
                DeviceCode: "MOCK_DEVICE",
                Type: "CheckOut"
            }
        ];
        await attendanceCol.insertMany(logs);

        // 5. Leave Request (Pending Approval)
        console.log("Seeding leave request...");
        const leaveCol = db.collection('leave_requests');
        await leaveCol.deleteMany({ EmployeeId: itStaffId });
        await leaveCol.insertOne({
            EmployeeId: itStaffId,
            EmployeeName: "Lê Văn Tuấn (Staff)",
            EmployeeCode: "IT-STF-01",
            DepartmentName: "Phòng Công Nghệ (IT)",
            LeaveTypeId: new ObjectId("65ebf3a40000000000000101"), // Annual Leave
            LeaveTypeName: "Nghỉ phép năm",
            StartDate: new Date("2026-03-10T00:00:00Z"),
            EndDate: new Date("2026-03-11T23:59:59Z"),
            TotalDays: 2,
            Reason: "Giải quyết việc gia đình (E2E Test)",
            Status: "Pending",
            CurrentLevel: 0,
            TotalLevelsRequired: 2,
            IsDeleted: false,
            CreatedAt: new Date(),
            UpdatedAt: new Date()
        });

        // 6. Assets for Accountant
        console.log("Seeding assets...");
        const assetCol = db.collection('assets');
        await assetCol.updateOne(
            { Code: "LAP-ACC-01" },
            {
                $set: {
                    Name: "MacBook Air M2",
                    Category: "Laptop",
                    SerialNumber: "SN-TRANS-01",
                    PurchaseDate: new Date("2024-01-01"),
                    Value: 30000000,
                    AssigneeId: accId.toString(),
                    AssigneeName: "Nguyễn Kim Ngân (Acc)",
                    Status: "Assigned",
                    IsDeleted: false
                }
            },
            { upsert: true }
        );

        console.log("--- SEEDING COMPLETED SUCCESSFULLY ---");
        console.log("Use IT-STF-01 (Lê Văn Tuấn) to view attendance/leave.");
        console.log("Use IT-MGR-01 (Trần Thế Anh) to approve leave.");
        console.log("Update ACC-01 (Nguyễn Kim Ngân) to 'Resigned' status to test asset recovery alert.");

    } catch (err) {
        console.error("Seeding Error:", err);
    } finally {
        await client.close();
    }
}

main();
