const { MongoClient, ObjectId } = require('mongodb');
const fs = require('fs');
const path = require('path');

async function main() {
    let connectionString = "mongodb://localhost:27017";
    let databaseName = "HR";

    // Try to load .env
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

        // 1. Insert Period
        const periodsCol = db.collection('payroll_periods');
        const period = {
            _id: "65dae2f30000000000000999",
            Name: "Lương tháng 02/2026",
            Month: 2,
            Year: 2026,
            StartDate: new Date("2026-02-01T00:00:00Z"),
            EndDate: new Date("2026-02-28T00:00:00Z"),
            PaymentDate: new Date("2026-03-05T00:00:00Z"),
            Status: "Open",
            TotalNetSalary: 2500000000,
            TotalInsurance: 250000000,
            TotalTax: 150000000,
            EmployeeCount: 100,
            Note: "Dữ liệu mẫu cho kiểm thử hệ thống",
            IsDeleted: false
        };

        await periodsCol.deleteOne({ _id: period._id });
        await periodsCol.insertOne(period);
        console.log("Inserted payroll period.");

        // 2. Insert Payrolls
        const payrollsCol = db.collection('payrolls');
        let json = fs.readFileSync('d:/MONEY/2026/TAN_TAN_LOC/TTL_ERP/data_seed/payrolls_100.json', 'utf8');
        if (json.charCodeAt(0) === 0xFEFF) {
            json = json.slice(1);
        }
        const payrolls = JSON.parse(json);


        for (let p of payrolls) {
            p.IsDeleted = false;
            // Ensure numbers are numbers
            p.BasicSalary = parseFloat(p.BasicSalary);
            p.TotalWorkSalary = parseFloat(p.TotalWorkSalary);
            p.OvertimeSalary = parseFloat(p.OvertimeSalary);
            p.Allowance = parseFloat(p.Allowance);
            p.Bonus = parseFloat(p.Bonus);
            p.InsuranceAmount = parseFloat(p.InsuranceAmount);
            p.TaxAmount = parseFloat(p.TaxAmount);
            p.Deduction = parseFloat(p.Deduction);
            p.CreatedAt = new Date(p.CreatedAt);
        }

        await payrollsCol.deleteMany({ PeriodId: "65dae2f30000000000000999" });
        await payrollsCol.insertMany(payrolls);
        console.log(`Inserted ${payrolls.length} payroll records.`);

    } finally {
        await client.close();
    }
}

main().catch(err => {
    console.error("IMPORT FAILED:");
    console.error(err);
    process.exit(1);
});

