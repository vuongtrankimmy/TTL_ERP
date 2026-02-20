$employees = @(
    @{"id" = "65bf0c300000000000000001"; "name" = "Phạm Minh Hùng"; "code" = "GD-001"; "base" = 50000000 },
    @{"id" = "65bf0c300000000000000002"; "name" = "Nguyễn Thị Mai"; "code" = "NS-001"; "base" = 25000000 },
    @{"id" = "65bf0c300000000000000003"; "name" = "Lê Văn Tuấn"; "code" = "IT-001"; "base" = 30000000 },
    @{"id" = "65bf0c300000000000000004"; "name" = "Trần Thanh Bình"; "code" = "IT-002"; "base" = 20000000 },
    @{"id" = "65bf0c300000000000000005"; "name" = "Hoàng Thu Phương"; "code" = "IT-003"; "base" = 12000000 }
)

for ($i = 6; $i -le 100; $i++) {
    $employees += @{"id" = "65bf0c300000000000000" + $i.ToString("000"); "name" = "Nhân viên " + $i; "code" = "NV-" + $i.ToString("000"); "base" = (Get-Random -Minimum 8000000 -Maximum 45000000) }
}

$month = 2
$year = 2026
$periodId = "65dae2f30000000000000999"

$payrolls = @()
$counter = 1

foreach ($emp in $employees) {
    $totalRequired = 22
    $actualWork = (Get-Random -InputObject @(20..22))
    if ($emp.code -like "*010*") { $actualWork = 15 } # Some case with many absences
    $unpaidLeave = $totalRequired - $actualWork
    $otHours = (Get-Random -InputObject @(0, 0, 5, 10, 15, 20))
    
    $basicSalary = $emp.base
    $workSalary = [math]::Round($basicSalary * ($actualWork / $totalRequired))
    $otSalary = [math]::Round(($basicSalary / $totalRequired / 8) * $otHours * 1.5)
    
    $allowances = @(
        @{"Name" = "Phụ cấp cơm"; "Amount" = 730000 },
        @{"Name" = "Phụ cấp điện thoại"; "Amount" = (Get-Random -InputObject @(200000, 300000, 500000)) }
    )
    if ($basicSalary -gt 25000000) {
        $allowances += @{"Name" = "Phụ cấp trách nhiệm"; "Amount" = 2000000 }
    }
    
    $totalAllowance = 0
    foreach ($a in $allowances) { $totalAllowance += $a.Amount }
    
    $bonus = (Get-Random -InputObject @(0, 0, 500000, 1000000, 2000000))
    
    $insurance = [math]::Round($basicSalary * 0.105)
    
    $taxable = $workSalary + $otSalary + $totalAllowance + $bonus - 11000000 - $insurance
    if ($taxable -lt 0) { $taxable = 0 }
    $tax = [math]::Round($taxable * 0.1)
    
    $deductions = @()
    if ($counter % 10 -eq 0) {
        $deductions += @{"Name" = "Phạt đi muộn"; "Amount" = 200000 }
    }
    
    $totalDeduction = 0
    foreach ($d in $deductions) { $totalDeduction += $d.Amount }
    
    $statusList = @("Draft", "Pending", "Approved", "Released")
    $status = $statusList[(Get-Random -Minimum 0 -Maximum 4)]
    
    $payroll = @{
        "EmployeeId"        = $emp.id;
        "EmployeeName"      = $emp.name;
        "EmployeeCode"      = $emp.code;
        "Month"             = $month;
        "Year"              = $year;
        "PeriodId"          = $periodId;
        "BasicSalary"       = [decimal]$basicSalary;
        "ActualWorkDays"    = [double]$actualWork;
        "TotalRequiredDays" = [double]$totalRequired;
        "OvertimeHours"     = [double]$otHours;
        "UnpaidLeaveDays"   = [double]$unpaidLeave;
        "TotalWorkSalary"   = [decimal]$workSalary;
        "OvertimeSalary"    = [decimal]$otSalary;
        "Allowance"         = [decimal]$totalAllowance;
        "AllowanceDetails"  = $allowances;
        "Bonus"             = [decimal]$bonus;
        "InsuranceAmount"   = [decimal]$insurance;
        "TaxAmount"         = [decimal]$tax;
        "Deduction"         = [decimal]$totalDeduction;
        "DeductionDetails"  = $deductions;
        "Status"            = $status;
        "IsConfirmed"       = ($status -eq "Approved" -or $status -eq "Released");
        "CreatedAt"         = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
    }
    
    $payrolls += $payroll
    $counter++
}

$payrolls | ConvertTo-Json -Depth 10 | Set-Content -Path "d:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\payrolls_100.json" -Encoding UTF8

