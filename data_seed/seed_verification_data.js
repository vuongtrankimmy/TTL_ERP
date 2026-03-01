const { MongoClient, ObjectId } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const dbName = 'HR';
const client = new MongoClient(url);

async function main() {
    try {
        await client.connect();
        console.log("Connected to MongoDB for final verification seeding...");
        const db = client.db(dbName);

        // 1. Ensure Attendance Period & Data
        console.log("Seeding Attendance for Feb 2026...");
        const attendancePeriodCol = db.collection('attendance_periods');
        const attendanceCol = db.collection('attendances');

        await attendancePeriodCol.updateOne(
            { Month: 2, Year: 2026 },
            { $set: { Status: "Open", IsDeleted: false, UpdatedAt: new Date() } },
            { upsert: true }
        );

        const employees = await db.collection('employees').find({ IsDeleted: false }).toArray();

        // 1.5. Ensure Work Schedules
        console.log("Seeding Work Schedules...");
        const scheduleCol = db.collection('work_schedules');
        await scheduleCol.deleteMany({});
        const shiftCol = db.collection('work_shifts');
        const defaultShift = await shiftCol.findOne({ Name: "Hành chính" });
        const shiftId = defaultShift ? defaultShift._id.toString() : "65fc5b5b0000000000000010";
        const schedules = [];
        const startDates = [new Date(2026, 1, 1), new Date(2026, 2, 1)];
        for (const startDate of startDates) {
            const year = startDate.getFullYear();
            const month = startDate.getMonth() + 1;
            const daysInMonth = month === 2 ? 28 : 31;
            for (const emp of employees) {
                for (let day = 1; day <= daysInMonth; day++) {
                    const date = new Date(year, month - 1, day);
                    if (date.getDay() !== 0) {
                        schedules.push({
                            EmployeeId: emp._id.toString(),
                            EmployeeCode: emp.Code,
                            EmployeeName: emp.FullName,
                            DepartmentId: emp.DepartmentId,
                            ShiftId: shiftId,
                            ShiftName: "Hành chính",
                            Date: date,
                            Status: (date.getDay() === 6) ? "HalfDay" : "Normal",
                            IsDeleted: false,
                            CreatedAt: new Date(),
                            UpdatedAt: new Date()
                        });
                    }
                }
            }
        }
        if (schedules.length > 0) await scheduleCol.insertMany(schedules);

        const attendanceRecords = [];
        for (const emp of employees) {
            for (let d = 1; d <= 28; d++) {
                const date = new Date(2026, 1, d);
                if (date.getDay() === 0) continue; // Skip Sundays

                attendanceRecords.push({
                    EmployeeId: emp._id.toString(),
                    EmployeeName: emp.FullName,
                    EmployeeCode: emp.Code,
                    Date: date,
                    CheckIn: new Date(2026, 1, d, 8, 0, 0),
                    CheckOut: new Date(2026, 1, d, 17, 30, 0),
                    Status: "Normal",
                    WorkingHours: 8.0,
                    ShiftName: "Ca Hành Chính",
                    IsDeleted: false,
                    CreatedAt: new Date(),
                    UpdatedAt: new Date()
                });
            }
        }
        await attendanceCol.deleteMany({ Date: { $gte: new Date(2026, 1, 1), $lte: new Date(2026, 1, 28, 23, 59, 59) } });
        if (attendanceRecords.length > 0) {
            await attendanceCol.insertMany(attendanceRecords);
            console.log(`  Inserted ${attendanceRecords.length} attendance records.`);
        }

        // 2. Ensure Payroll Period & Data
        console.log("Seeding Payroll for Feb 2026...");
        const payrollPeriodCol = db.collection('payroll_periods');
        const payrollCol = db.collection('payrolls');

        const periodId = new ObjectId("65dae2f30000000000000999");
        await payrollPeriodCol.updateOne(
            { _id: periodId },
            {
                $set: {
                    Name: "Bảng lương Tháng 02/2026",
                    Month: 2,
                    Year: 2026,
                    StartDate: new Date(2026, 1, 1),
                    EndDate: new Date(2026, 1, 28),
                    Status: "Draft",
                    EmployeeCount: employees.length,
                    IsDeleted: false,
                    UpdatedAt: new Date()
                }
            },
            { upsert: true }
        );

        const payrolls = employees.map(emp => ({
            PeriodId: periodId,
            EmployeeId: emp._id,
            EmployeeName: emp.FullName,
            EmployeeCode: emp.Code,
            DepartmentName: "Phòng ban", // Simplified for seed
            PositionName: "Nhân viên",
            BasicSalary: emp.Salary || 10000000,
            ActualWorkDays: 24,
            TotalRequiredDays: 24,
            TotalWorkSalary: emp.Salary || 10000000,
            NetSalary: emp.Salary || 10000000,
            Status: "Draft",
            IsConfirmed: false,
            IsDeleted: false,
            CreatedAt: new Date(),
            UpdatedAt: new Date()
        }));

        await payrollCol.deleteMany({ PeriodId: periodId });
        if (payrolls.length > 0) {
            await payrollCol.insertMany(payrolls);
            console.log(`  Inserted ${payrolls.length} payslips.`);
        }

        // 3. Ensure Asset Allocations
        console.log("Seeding Asset Allocations...");
        const assetCol = db.collection('assets');
        const allocationCol = db.collection('asset_allocations');

        const availableAssets = await assetCol.find({ Status: "Available", IsDeleted: false }).toArray();
        const allocations = [];

        // Allocate first 5 assets to first 5 employees
        for (let i = 0; i < Math.min(availableAssets.length, employees.length, 5); i++) {
            const asset = availableAssets[i];
            const emp = employees[i];

            allocations.push({
                AssetId: asset._id.toString(),
                AssetCode: asset.Code,
                AssetName: asset.Name,
                EmployeeId: emp._id.toString(),
                EmployeeCode: emp.Code,
                EmployeeName: emp.FullName,
                AllocatedDate: new Date(2026, 0, 15), // Jan 15, 2026
                Status: "Active",
                Condition: "Tốt",
                Note: "Cấp phát định kỳ đầu năm",
                IsDeleted: false,
                CreatedAt: new Date(),
                UpdatedAt: new Date()
            });

            // Mark asset as InUse
            await assetCol.updateOne({ _id: asset._id }, { $set: { Status: "InUse" } });
        }

        await allocationCol.deleteMany({});
        if (allocations.length > 0) {
            await allocationCol.insertMany(allocations);
            console.log(`  Inserted ${allocations.length} asset allocations.`);
        }

        // 4. Update Payroll Period totals
        const totalNet = payrolls.reduce((sum, p) => sum + (p.NetSalary || 0), 0);
        await payrollPeriodCol.updateOne(
            { _id: periodId },
            { $set: { TotalNetSalary: totalNet, TotalInsurance: 0, TotalTax: 0 } }
        );
        console.log(`  Updated Payroll Period totals: ${totalNet.toLocaleString()} VND`);

        // 5. Seed Benefits
        console.log("Seeding Benefits...");
        const benefitCol = db.collection('benefits');
        const benefitAllocCol = db.collection('benefit_allocations');

        const benefitTypes = [
            { Name: "Phụ cấp Ăn trưa", Amount: 730000, Type: "Monthly", Description: "Hỗ trợ cơm trưa" },
            { Name: "Phụ cấp Xăng xe", Amount: 500000, Type: "Monthly", Description: "Hỗ trợ đi lại" }
        ];

        await benefitCol.deleteMany({});
        const insertedBenefits = await benefitCol.insertMany(benefitTypes.map(b => ({
            ...b,
            IsDeleted: false,
            CreatedAt: new Date(),
            UpdatedAt: new Date()
        })));

        const benefitAllocations = [];
        const benefitIds = Object.values(insertedBenefits.insertedIds);

        for (const emp of employees) {
            for (const bId of benefitIds) {
                benefitAllocations.push({
                    BenefitId: bId.toString(),
                    EmployeeId: emp._id.toString(),
                    Status: "Active",
                    StartDate: new Date(2026, 0, 1),
                    IsDeleted: false,
                    CreatedAt: new Date(),
                    UpdatedAt: new Date()
                });
            }
        }

        await benefitAllocCol.deleteMany({});
        await benefitAllocCol.insertMany(benefitAllocations);
        console.log(`  Inserted ${benefitAllocations.length} benefit allocations.`);

        console.log("\n--- VERIFICATION DATA SEEDING COMPLETED ---");

    } catch (err) {
        console.error("Seeding Error:", err);
    } finally {
        await client.close();
    }
}

main();
