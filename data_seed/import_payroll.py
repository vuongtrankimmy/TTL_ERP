import json
import os
from pymongo import MongoClient

def import_payroll():
    # Get connection details from environment or use defaults
    mongo_uri = "mongodb://localhost:27017" # Default, will be overridden by environment if available
    db_name = "HR"
    
    # Try to load from .env if possible
    try:
        with open('d:/MONEY/2026/TAN_TAN_LOC/TTL_API/.env', 'r') as f:
            for line in f:
                if line.startswith('MONGODB_CONNECTION_STRING='):
                    mongo_uri = line.split('=')[1].strip().strip('"')
                if line.startswith('MONGODB_DATABASE_NAME='):
                    db_name = line.split('=')[1].strip().strip('"')
    except:
        pass

    client = MongoClient(mongo_uri)
    db = client[db_name]
    
    # 1. Insert Payroll Period
    period = {
        "_id": "65dae2f30000000000000999",
        "Name": "Lương tháng 02/2026",
        "Month": 2,
        "Year": 2026,
        "StartDate": "2026-02-01T00:00:00Z",
        "EndDate": "2026-02-28T00:00:00Z",
        "PaymentDate": "2026-03-05T00:00:00Z",
        "Status": "Open",
        "TotalNetSalary": 2500000000,
        "TotalInsurance": 250000000,
        "TotalTax": 150000000,
        "EmployeeCount": 100,
        "Note": "Dữ liệu mẫu cho kiểm thử hệ thống",
        "IsDeleted": False
    }
    
    db.payroll_periods.delete_many({"_id": period["_id"]})
    db.payroll_periods.insert_one(period)
    print("Inserted payroll period.")

    # 2. Insert Payroll Records
    with open('d:/MONEY/2026/TAN_TAN_LOC/TTL_ERP/data_seed/payrolls_100.json', 'r', encoding='utf-8') as f:
        payrolls = json.load(f)
    
    # Add IsDeleted flag if missing
    for p in payrolls:
        p["IsDeleted"] = False
        # Ensure numeric types are correct
        p["BasicSalary"] = float(p.get("BasicSalary", 0))
        p["TotalWorkSalary"] = float(p.get("TotalWorkSalary", 0))
        p["OvertimeSalary"] = float(p.get("OvertimeSalary", 0))
        p["Allowance"] = float(p.get("Allowance", 0))
        p["Bonus"] = float(p.get("Bonus", 0))
        p["InsuranceAmount"] = float(p.get("InsuranceAmount", 0))
        p["TaxAmount"] = float(p.get("TaxAmount", 0))
        p["Deduction"] = float(p.get("Deduction", 0))

    db.payrolls.delete_many({"PeriodId": "65dae2f30000000000000999"})
    db.payrolls.insert_many(payrolls)
    print(f"Inserted {len(payrolls)} payroll records.")

if __name__ == "__main__":
    import_payroll()
