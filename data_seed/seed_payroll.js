const { MongoClient, ObjectId } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const client = new MongoClient(url);
const dbName = 'HR';

async function main() {
    try {
        await client.connect();
        const db = client.db(dbName);
        const periodColl = db.collection('payroll_periods');
        const payrollColl = db.collection('payrolls');

        // Clear existing test data
        await periodColl.deleteMany({});
        await payrollColl.deleteMany({});

        const periodId = new ObjectId("65dae2f30000000000000999");
        const period = {
            _id: periodId,
            Name: "Bảng lương Tháng 02/2026",
            Month: 2,
            Year: 2026,
            StartDate: new Date(2026, 1, 1),
            EndDate: new Date(2026, 1, 28),
            Status: "Draft",
            TotalNetSalary: 0,
            TotalInsurance: 0,
            TotalTax: 0,
            EmployeeCount: 15,
            IsDeleted: false,
            CreatedAt: new Date(),
            UpdatedAt: new Date()
        };

        await periodColl.insertOne(period);

        const employees = [
            { id: "69912b1b13b75cbbfcbbb5d1", name: "Đặng Văn G 7", dept: "Kỹ thuật", salary: 15000000 },
            { id: "69912b1b13b75cbbfcbbb5d2", name: "Bùi Thị H 8", dept: "Marketing", salary: 12000000 },
            { id: "69912b1b13b75cbbfcbbb5d4", name: "Lý Thị K 10", dept: "Sản xuất", salary: 10000000 },
            { id: "69912b1b13b75cbbfcbbb5d5", name: "Trịnh Văn L 11", dept: "Kế toán", salary: 14000000 },
            { id: "69912b1b13b75cbbfcbbb5d6", name: "Đỗ Thị M 12", dept: "Nhân sự", salary: 13000000 },
            { id: "69912b1b13b75cbbfcbbb5d7", name: "Hồ Văn N 13", dept: "Kho", salary: 9000000 },
            { id: "69912b1b13b75cbbfcbbb5d8", name: "Huỳnh Thị O 14", dept: "Kỹ thuật", salary: 16000000 },
            { id: "69912b1b13b75cbbfcbbb5d9", name: "Dương Văn P 15", dept: "Marketing", salary: 11000000 },
            { id: "69912b1b13b75cbbfcbbb5da", name: "Võ Thị Q 16", dept: "Sản xuất", salary: 10500000 },
            { id: "69912b1b13b75cbbfcbbb5db", name: "Ngô Văn R 17", dept: "IT", salary: 18000000 },
            { id: "69912b1b13b75cbbfcbbb5dc", name: "Cao Thị S 18", dept: "Dịch vụ", salary: 9500000 },
            { id: "69912b1b13b75cbbfcbbb5dd", name: "Đinh Văn T 19", dept: "Kỹ thuật", salary: 15500000 },
            { id: "69912b1b13b75cbbfcbbb5de", name: "Phạm Văn U 20", dept: "IT", salary: 17000000 },
            { id: "69912b1b13b75cbbfcbbb5df", name: "Lê Thị V 21", dept: "Kế toán", salary: 14500000 },
            { id: "69912b1b13b75cbbfcbbb5e0", name: "Hoàng Văn X 22", dept: "Sản xuất", salary: 10000000 }
        ];

        const payrolls = employees.map((emp, i) => {
            const basicSalary = emp.salary;
            const actualDays = 22 + (i % 3);
            const requiredDays = 24;
            const workSalary = (basicSalary / requiredDays) * actualDays;
            const allowance = 1000000 + (i * 200000);
            const deduction = 500000 + (i * 50000);
            const netSalary = workSalary + allowance - deduction;

            return {
                PeriodId: periodId,
                EmployeeId: new ObjectId(emp.id),
                EmployeeName: emp.name,
                EmployeeCode: `EMP-200${i + 7}`,
                DepartmentName: emp.dept,
                PositionName: "Nhân viên",
                BasicSalary: basicSalary,
                ActualWorkDays: actualDays,
                TotalRequiredDays: requiredDays,
                OvertimeHours: i % 5,
                UnpaidLeaveDays: i % 2,
                TotalWorkSalary: workSalary,
                OvertimeSalary: (i % 5) * 100000,
                Allowance: allowance,
                AllowanceDetails: [{ Name: "Phụ cấp ăn trưa", Amount: allowance, Note: "Cố định tháng" }],
                Bonus: i % 4 === 0 ? 500000 : 0,
                InsuranceAmount: basicSalary * 0.105,
                TaxAmount: netSalary > 11000000 ? (netSalary - 11000000) * 0.05 : 0,
                Deduction: deduction,
                DeductionDetails: [{ Name: "Tạm ứng", Amount: deduction, Note: "Khấu trừ lương" }],
                TotalSalary: workSalary + allowance,
                NetSalary: netSalary,
                Status: "Draft",
                IsConfirmed: false,
                IsDeleted: false,
                CreatedAt: new Date(),
                UpdatedAt: new Date()
            };
        });

        await payrollColl.insertMany(payrolls);

        // Update total calculations for period
        const totalNet = payrolls.reduce((sum, p) => sum + p.NetSalary, 0);
        const totalTax = payrolls.reduce((sum, p) => sum + p.TaxAmount, 0);
        const totalIns = payrolls.reduce((sum, p) => sum + p.InsuranceAmount, 0);

        await periodColl.updateOne({ _id: periodId }, {
            $set: {
                TotalNetSalary: totalNet,
                TotalTax: totalTax,
                TotalInsurance: totalIns,
                EmployeeCount: payrolls.length
            }
        });

        console.log(`Seeded payroll period and ${payrolls.length} payslips.`);

    } catch (err) {
        console.error(err);
    } finally {
        await client.close();
    }
}

main();
