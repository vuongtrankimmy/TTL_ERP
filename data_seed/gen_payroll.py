import json
import random
from datetime import datetime

employees = [
    {"id": "65bf0c300000000000000001", "name": "Phạm Minh Hùng", "code": "GD-001", "base": 50000000},
    {"id": "65bf0c300000000000000002", "name": "Nguyễn Thị Mai", "code": "NS-001", "base": 25000000},
    {"id": "65bf0c300000000000000003", "name": "Lê Văn Tuấn", "code": "IT-001", "base": 30000000},
    {"id": "65bf0c300000000000000004", "name": "Trần Thanh Bình", "code": "IT-002", "base": 20000000},
    {"id": "65bf0c300000000000000005", "name": "Hoàng Thu Phương", "code": "IT-003", "base": 12000000},
]

# Add dummy employees to reach 100
for i in range(6, 101):
    employees.append({
        "id": f"65bf0c300000000000000{i:03d}",
        "name": f"Employee {i}",
        "code": f"EMP-{i:03d}",
        "base": random.randint(10000000, 40000000)
    })

period_id = "65dae2f30000000000000999"
month = 2
year = 2026

payrolls = []

for emp in employees:
    total_required = 22
    actual_work = random.choice([20, 21, 22, 22, 22]) # Mostly full
    unpaid_leave = total_required - actual_work
    ot_hours = random.choice([0, 0, 2, 5, 8, 10])
    
    basic_salary = emp["base"]
    work_salary = int(basic_salary * (actual_work / total_required))
    ot_salary = int((basic_salary / total_required / 8) * ot_hours * 1.5)
    
    allowances = [
        {"Name": "Phụ cấp cơm", "Amount": 730000},
        {"Name": "Phụ cấp xăng xe", "Amount": random.choice([500000, 1000000])},
        {"Name": "Phụ cấp điện thoại", "Amount": random.choice([200000, 500000])}
    ]
    total_allowance = sum(a["Amount"] for a in allowances)
    
    bonus = random.choice([0, 0, 0, 500000, 1000000, 2000000])
    
    insurance = int(basic_salary * 0.105) # 8% BHXH, 1.5% BHYT, 1% BHTN
    
    # Simple tax calculation for demo
    taxable = max(0, work_salary + ot_salary + total_allowance + bonus - 11000000 - insurance)
    tax = int(taxable * 0.1) if taxable > 0 else 0
    
    deductions = []
    if random.random() < 0.1: # 10% chance of fine
        deductions.append({"Name": "Phạt đi muộn", "Amount": 100000})
    total_deduction = sum(d["Amount"] for d in deductions)
    
    status = random.choice(["Draft", "Pending", "Approved", "Released"])
    
    payrolls.append({
        "_id": {"$oid": f"65dae2f3000000000000{str(len(payrolls)+1).zfill(4)}"},
        "EmployeeId": emp["id"],
        "EmployeeName": emp["name"],
        "EmployeeCode": emp["code"],
        "Month": month,
        "Year": year,
        "PeriodId": period_id,
        "BasicSalary": basic_salary,
        "ActualWorkDays": actual_work,
        "TotalRequiredDays": total_required,
        "OvertimeHours": ot_hours,
        "UnpaidLeaveDays": unpaid_leave,
        "TotalWorkSalary": work_salary,
        "OvertimeSalary": ot_salary,
        "Allowance": total_allowance,
        "AllowanceDetails": allowances,
        "Bonus": bonus,
        "InsuranceAmount": insurance,
        "TaxAmount": tax,
        "Deduction": total_deduction,
        "DeductionDetails": deductions,
        "Status": status,
        "IsConfirmed": status in ["Approved", "Released"],
        "IsDeleted": False,
        "CreatedAt": {"$date": "2026-02-15T00:00:00Z"}
    })

with open("d:/MONEY/2026/TAN_TAN_LOC/TTL_ERP/data_seed/payrolls.json", "w", encoding="utf-8") as f:
    json.dump(payrolls, f, indent=4, ensure_ascii=False)

print(f"Generated {len(payrolls)} payroll rows.")
