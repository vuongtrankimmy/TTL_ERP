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
        const totalNet = payrolls.reduce((sum, p) => sum + Number(p.NetSalary || 0), 0);
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

        // 6. Seed Lookups
        console.log("Seeding Lookups...");
        const lookupCol = db.collection('lookups');
        const lookupTransCol = db.collection('lookups_translate');

        const rawLookups = [
            // Gender
            { Type: "Gender", Code: "Male", vi: "Nam", en: "Male", Order: 1 },
            { Type: "Gender", Code: "Female", vi: "Nữ", en: "Female", Order: 2 },
            { Type: "Gender", Code: "Other", vi: "Khác", en: "Other", Order: 3 },

            // MaritalStatus
            { Type: "MaritalStatus", Code: "Single", vi: "Độc thân", en: "Single", Order: 1 },
            { Type: "MaritalStatus", Code: "Married", vi: "Đã kết hôn", en: "Married", Order: 2 },
            { Type: "MaritalStatus", Code: "Divorced", vi: "Ly hôn", en: "Divorced", Order: 3 },
            { Type: "MaritalStatus", Code: "Widowed", vi: "Góa", en: "Widowed", Order: 4 },

            // EmployeeStatus
            { Type: "EmployeeStatus", Code: "Active", vi: "Đang làm việc", en: "Active", Order: 1, Color: "success" },
            { Type: "EmployeeStatus", Code: "Probation", vi: "Thử việc", en: "Probation", Order: 2, Color: "warning" },
            { Type: "EmployeeStatus", Code: "Resigned", vi: "Đã nghỉ việc", en: "Resigned", Order: 3, Color: "danger" },

            // ContractType
            { Type: "ContractType", Code: "Probation", vi: "Thử việc", en: "Probation", Order: 1, Color: "warning" },
            { Type: "ContractType", Code: "Definite", vi: "Xác định thời hạn", en: "Definite", Order: 2, Color: "primary" },
            { Type: "ContractType", Code: "Indefinite", vi: "Không xác định thời hạn", en: "Indefinite", Order: 3, Color: "success" },

            // Workplace
            { Type: "Workplace", Code: "HQ", vi: "Trụ sở chính", en: "Headquarters", Order: 1 },
            { Type: "Workplace", Code: "HN", vi: "Chi nhánh Hà Nội", en: "Hanoi Branch", Order: 2 },
            { Type: "Workplace", Code: "DN", vi: "Chi nhánh Đà Nẵng", en: "Da Nang Branch", Order: 3 },

            // AttendanceStatus
            { Type: "AttendanceStatus", Code: "Normal", vi: "Đúng giờ", en: "On-time", Order: 1 },
            { Type: "AttendanceStatus", Code: "Late", vi: "Đi muộn", en: "Late", Order: 2 },
            { Type: "AttendanceStatus", Code: "EarlyLeave", vi: "Về sớm", en: "Early Leave", Order: 3 },
            { Type: "AttendanceStatus", Code: "Absent", vi: "Nghỉ / Không phép", en: "Absent", Order: 4 },
            { Type: "AttendanceStatus", Code: "Holiday", vi: "Lễ / Nghỉ chế độ", en: "Holiday", Order: 5 },

            // TemplateStatus (For Contract Templates)
            { Type: "TemplateStatus", Code: "Draft", vi: "Bản nháp", en: "Draft", Order: 1 },
            { Type: "TemplateStatus", Code: "Active", vi: "Hoạt động", en: "Active", Order: 2 },
            { Type: "TemplateStatus", Code: "Archived", vi: "Lưu trữ", en: "Archived", Order: 3 },

            // ContractStatus (For Employee Contracts)
            { Type: "ContractStatus", Code: "Draft", vi: "Bản nháp", en: "Draft", Order: 1 },
            { Type: "ContractStatus", Code: "Signed", vi: "Đã ký", en: "Signed", Order: 2 },
            { Type: "ContractStatus", Code: "Active", vi: "Đang hiệu lực", en: "Active", Order: 3 },
            { Type: "ContractStatus", Code: "Expired", vi: "Hết hạn", en: "Expired", Order: 4 },
            { Type: "ContractStatus", Code: "Terminated", vi: "Đã chấm dứt", en: "Terminated", Order: 5 },

            // Ethnicity
            { Type: "Ethnicity", Code: "Kinh", vi: "Kinh", en: "Kinh", Order: 1 },
            { Type: "Ethnicity", Code: "Tay", vi: "Tày", en: "Tay", Order: 2 },
            { Type: "Ethnicity", Code: "Thai", vi: "Thái", en: "Thai", Order: 3 },
            { Type: "Ethnicity", Code: "Muong", vi: "Mường", en: "Muong", Order: 4 },
            { Type: "Ethnicity", Code: "Khmer", vi: "Khmer", en: "Khmer", Order: 5 },
            { Type: "Ethnicity", Code: "Other", vi: "Khác", en: "Other", Order: 99 },

            // Religion
            { Type: "Religion", Code: "None", vi: "Không", en: "None", Order: 1 },
            { Type: "Religion", Code: "Buddhism", vi: "Phật giáo", en: "Buddhism", Order: 2 },
            { Type: "Religion", Code: "Catholicism", vi: "Công giáo", en: "Catholicism", Order: 3 },
            { Type: "Religion", Code: "Protestantism", vi: "Tin lành", en: "Protestantism", Order: 4 },
            { Type: "Religion", Code: "Other", vi: "Khác", en: "Other", Order: 99 },

            // Nationality
            { Type: "Nationality", Code: "VN", vi: "Việt Nam", en: "Vietnam", Order: 1 },
            { Type: "Nationality", Code: "US", vi: "Mỹ", en: "USA", Order: 2 },
            { Type: "Nationality", Code: "JP", vi: "Nhật Bản", en: "Japan", Order: 3 },
            { Type: "Nationality", Code: "KR", vi: "Hàn Quốc", en: "South Korea", Order: 4 },
            { Type: "Nationality", Code: "CN", vi: "Trung Quốc", en: "China", Order: 5 }
        ];

        await lookupCol.deleteMany({});
        await lookupTransCol.deleteMany({});

        const typeCounters = {};

        for (const rl of rawLookups) {
            // Assign sequential LookupID per Type
            if (!typeCounters[rl.Type]) typeCounters[rl.Type] = 1;
            const seqId = typeCounters[rl.Type]++;

            const lookupId = new ObjectId();
            await lookupCol.insertOne({
                _id: lookupId,
                Type: rl.Type,
                Code: rl.Code,
                Name: rl.vi, // Legacy support
                Order: rl.Order,
                LookupID: seqId,
                Color: rl.Color || "",
                IsActive: true,
                IsDeleted: false,
                CreatedAt: new Date(),
                UpdatedAt: new Date()
            });

            const translations = [
                { LookupId: lookupId.toString(), LanguageCode: "vi-VN", Name: rl.vi },
                { LookupId: lookupId.toString(), LanguageCode: "en-US", Name: rl.en }
            ];

            await lookupTransCol.insertMany(translations.map(t => ({
                ...t,
                IsDeleted: false,
                CreatedAt: new Date(),
                UpdatedAt: new Date()
            })));
        }

        console.log(`  Inserted ${rawLookups.length} lookups with translations.`);

        console.log("\n--- VERIFICATION DATA SEEDING COMPLETED ---");

    } catch (err) {
        console.error("Seeding Error:", err);
    } finally {
        await client.close();
    }
}

main();
