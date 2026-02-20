const { MongoClient, ObjectId } = require('mongodb');
const fs = require('fs');

async function main() {
    let connectionString = "mongodb://localhost:27017";
    let databaseName = "HR";

    try {
        const envContent = fs.readFileSync('d:/MONEY/2026/TAN_TAN_LOC/TTL_API/.env', 'utf8');
        const lines = envContent.split('\n');
        for (let line of lines) {
            if (line.includes('MONGODB_CONNECTION_STRING=')) {
                connectionString = line.substring(line.indexOf('=') + 1).trim().replace(/"/g, '');
            }
            if (line.includes('MONGODB_DATABASE_NAME=')) {
                databaseName = line.substring(line.indexOf('=') + 1).trim().replace(/"/g, '');
            }
        }
    } catch (err) {
        console.log("Could not load .env, using defaults.");
    }

    console.log(`Connecting to ${connectionString}...`);
    const client = new MongoClient(connectionString);

    try {
        await client.connect();
        const db = client.db(databaseName);

        // 1. Get Employees to seed data for
        const employeesCol = db.collection('employees');
        const employees = await employeesCol.find({
            $or: [
                { IsDeleted: false },
                { IsDeleted: { $exists: false } }
            ]
        }).limit(20).toArray();

        if (employees.length === 0) {
            console.log("No employees found to seed (check if IsDeleted field exists).");
            // Fallback: try finding ANY employees
            const allEmps = await employeesCol.find({}).limit(20).toArray();
            if (allEmps.length > 0) {
                console.log("Found employees without IsDeleted filter, continuing...");
                employees.push(...allEmps);
            } else {
                console.log("Wait, still no employees found at all in 'employees' collection.");
                return;
            }
        }

        const month = 2;
        const year = 2026;
        const daysInMonth = 28;

        // 2. Insert Attendance Period
        const periodsCol = db.collection('attendance_periods');
        const period = {
            Month: month,
            Year: year,
            Status: "Open",
            IsDeleted: false,
            CreatedAt: new Date(),
            UpdatedAt: new Date()
        };
        await periodsCol.deleteMany({ Month: month, Year: year });
        await periodsCol.insertOne(period);

        // 3. Generate Daily Attendances
        const attendancesCol = db.collection('attendances');
        await attendancesCol.deleteMany({ Date: { $gte: new Date(year, month - 1, 1), $lte: new Date(year, month - 1, daysInMonth, 23, 59, 59) } });

        const attendanceRecords = [];
        const statuses = ["Normal", "Late", "EarlyLeave", "Normal", "Normal"]; // Bias towards Normal

        for (const emp of employees) {
            for (let d = 1; d <= daysInMonth; d++) {
                const date = new Date(year, month - 1, d);
                if (date.getDay() === 0) continue; // Skip Sundays

                const status = statuses[Math.floor(Math.random() * statuses.length)];
                let checkIn = new Date(year, month - 1, d, 8, 0, 0);
                let checkOut = new Date(year, month - 1, d, 17, 30, 0);

                if (status === "Late") {
                    checkIn.setMinutes(15 + Math.floor(Math.random() * 30));
                } else if (status === "EarlyLeave") {
                    checkOut.setHours(16);
                    checkOut.setMinutes(30 + Math.floor(Math.random() * 20));
                }

                attendanceRecords.push({
                    EmployeeId: emp._id.toString(),
                    EmployeeName: emp.FullName,
                    EmployeeCode: emp.Code,
                    Date: date,
                    CheckIn: checkIn,
                    CheckOut: checkOut,
                    Status: status,
                    WorkingHours: status === "EarlyLeave" ? 7.5 : (status === "Late" ? 7.7 : 8.0),
                    ShiftName: "Ca Hành Chính",
                    IsDeleted: false,
                    CreatedAt: new Date(),
                    UpdatedAt: new Date()
                });
            }
        }

        if (attendanceRecords.length > 0) {
            await attendancesCol.insertMany(attendanceRecords);
            console.log(`Inserted ${attendanceRecords.length} daily attendance records.`);
        }

        // 4. Generate Monthly Summary (Required for the List view if it uses it, but GetMonthlyTimesheetQueryHandler calculates from daily)
        // However, the frontend Timesheet.razor expects data that maps to AttendanceModel which has properties like StandardWork, ActualWork, LateCount etc.
        // Looking at GetMonthlyTimesheetQueryHandler.cs, it actually returns PagedResult<TimesheetDto>.
        // BUT the frontend service `AttendanceService.GetTimesheetsAsync()` returns `ApiResponse<PagedResult<AttendanceModel>>`.
        // AND the frontend Timesheet.razor loops over `AttendanceModel`.
        // THERE IS A MISMATCH BETWEEN API AND FRONTEND.

        console.log("Seeding completed.");

    } finally {
        await client.close();
    }
}

main().catch(err => {
    console.error(err);
    process.exit(1);
});
