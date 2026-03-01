
# Connect to MongoDB and get current employee mapping
$connStr = "mongodb://localhost:27030"
$dbName = "HR"

# Get all employees
$jsPath = "get_employees.js"
$jsContent = "const emps = db.employees.find({}, { Code: 1, FullName: 1 }).toArray(); print(JSON.stringify(emps));"
Set-Content -Path $jsPath -Value $jsContent

$jsonEmps = mongosh "$connStr/$dbName" --quiet --eval $jsContent
$employees = $jsonEmps | ConvertFrom-Json
Remove-Item $jsPath

if (!$employees) {
    Write-Error "Could not fetch employees from database."
    exit
}

$header = "log_id,employee_code,employee_name,method,action,device_id,device_name,check_time,location"
$out = @($header)

$year = 2026
$month = 1
$daysInMonth = [DateTime]::DaysInMonth($year, $month)

$counter = 1
foreach ($day in 1..$daysInMonth) {
    $currentDate = Get-Date -Year $year -Month $month -Day $day
    $dateStr = $currentDate.ToString("yyyy-MM-dd")
    $dayOfWeek = $currentDate.DayOfWeek

    # Skip Sundays
    if ($dayOfWeek -eq "Sunday") { continue }

    foreach ($emp in $employees) {
        $c = $emp.Code
        $empName = $emp.FullName
        
        # Randomize behavior
        $rand = Get-Random -Minimum 0 -Maximum 100
        
        # Saturday is half day or skip
        if ($dayOfWeek -eq "Saturday" -and $rand -lt 30) { continue }

        # Case: Absent (5%)
        if ($rand -lt 5) { continue }

        # Times
        $checkInTime = "08:00:00"
        $checkOutTime = "17:00:00"

        # Case: Late (10%)
        if ($rand -ge 5 -and $rand -lt 15) {
            $lateMinutes = Get-Random -Minimum 1 -Maximum 60
            $checkInTime = (Get-Date "08:00:00").AddMinutes($lateMinutes).ToString("HH:mm:ss")
        }

        # Case: Early Leave (10%)
        if ($rand -ge 15 -and $rand -lt 25) {
            $earlyMinutes = Get-Random -Minimum 1 -Maximum 60
            $checkOutTime = (Get-Date "17:00:00").AddMinutes(-$earlyMinutes).ToString("HH:mm:ss")
        }

        # Add Jitter
        $inJitter = Get-Random -Minimum -10 -Maximum 5
        $outJitter = Get-Random -Minimum 0 -Maximum 15
        
        $actualIn = (Get-Date $checkInTime).AddMinutes($inJitter).ToString("HH:mm:ss")
        $actualOut = (Get-Date $checkOutTime).AddMinutes($outJitter).ToString("HH:mm:ss")

        # Case: Missing Check-Out (2%)
        if ($rand -ge 90 -and $rand -lt 92) {
            $out += "L$counter,$c,$empName,Fingerprint,CheckIn,D01,Main Gate,$dateStr $actualIn,HCM"
            $counter++
            continue
        }

        # Case: Missing Check-In (2%)
        if ($rand -ge 92 -and $rand -lt 94) {
            $out += "L$counter,$c,$empName,Fingerprint,CheckOut,D01,Main Gate,$dateStr $actualOut,HCM"
            $counter++
            continue
        }

        # Normal pair
        $out += "L$counter,$c,$empName,Fingerprint,CheckIn,D01,Main Gate,$dateStr $actualIn,HCM"
        $counter++
        $out += "L$counter,$c,$empName,Fingerprint,CheckOut,D01,Main Gate,$dateStr $actualOut,HCM"
        $counter++
    }
}

$outputPath = 'd:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\attendance_jan_2026.csv'
$out | Out-File -FilePath $outputPath -Encoding utf8
Write-Host "SUCCESS: Generated $outputPath with $($out.Count - 1) logs for January 2026."
