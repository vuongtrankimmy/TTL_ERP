
$employees = @(
    @("EMP-2001", "Nguyễn Văn A 1 (Test)", "Engineering"),
    @("EMP-2002", "Trần Thị B 2", "Engineering"),
    @("EMP-2003", "Lê Văn C 3", "Engineering"),
    @("EMP-2004", "Phạm Thị D 4", "Engineering"),
    @("EMP-2007", "Đặng Văn G 7", "Engineering"),
    @("EMP-2008", "Bùi Thị H 8", "Engineering"),
    @("EMP-2010", "Lý Thị K 10", "Engineering"),
    @("EMP-2011", "Trịnh Văn L 11", "Engineering"),
    @("EMP-2012", "Đỗ Thị M 12", "Engineering"),
    @("EMP-2013", "Hồ Văn N 13", "Engineering"),
    @("EMP-2014", "Huỳnh Thị O 14", "Engineering"),
    @("EMP-2015", "Dương Văn P 15", "Engineering")
)

$year = 2026
$monthNames = @("jan", "feb", "mar")

foreach ($month in 1..3) {
    $mName = $monthNames[$month - 1]
    $fileName = "attendance_$($mName)_2026.csv"
    $filePath = Join-Path "d:\MONEY\2026\TAN_TAN_LOC\TTL_ERP" $fileName
    
    $header = "log_id,employee_code,employee_name,department,method,action,device_id,device_name,check_time,location,gps_lat,gps_lng,confidence"
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    $sw = New-Object System.IO.StreamWriter($filePath, $false, $utf8NoBom)
    $sw.WriteLine($header)
    
    $daysInMonth = [DateTime]::DaysInMonth($year, $month)
    $logId = 1
    $random = New-Object System.Random($month * 100)

    for ($day = 1; $day -le $daysInMonth; $day++) {
        $date = [DateTime]::new($year, $month, $day)
        $isWeekend = ($date.DayOfWeek -eq [DayOfWeek]::Saturday -or $date.DayOfWeek -eq [DayOfWeek]::Sunday)

        foreach ($emp in $employees) {
            if ($isWeekend -and $random.Next(100) -gt 15) { continue }
            if (-not $isWeekend -and $random.Next(100) -lt 2) { continue }

            $checkInMin = $random.Next(15, 105)
            $checkIn = $date.AddHours(7).AddMinutes($checkInMin)
            $idStr = "LOG-$($month)-$($logId.ToString('0000'))"
            
            $method = if ($random.Next(100) -lt 20) { "Ứng dụng Di động" } else { "Nhận diện Khuôn mặt" }
            $actionIn = "Giờ vào"
            $deviceName = if ($method -eq "Ứng dụng Di động") { "Điện thoại cá nhân" } else { "Máy quét cổng chính" }

            $lineIn = """$idStr"",""$($emp[0])"",""$($emp[1])"",""$($emp[2])"",""$method"",""$actionIn"",""DEV-01"",""$deviceName"",""$($checkIn.ToString('dd/MM/yyyy HH:mm:ss'))"",""Văn phòng chính"",""10.7626"",""106.6601"",""0.99"""
            $sw.WriteLine($lineIn)
            $logId++

            $checkOutMin = $random.Next(0, 150)
            $checkOut = $date.AddHours(17).AddMinutes($checkOutMin)
            $actionOut = "Giờ ra"
            $lineOut = """LOG-$($month)-$($logId.ToString('0000'))"",""$($emp[0])"",""$($emp[1])"",""$($emp[2])"",""$method"",""$actionOut"",""DEV-01"",""$deviceName"",""$($checkOut.ToString('dd/MM/yyyy HH:mm:ss'))"",""Văn phòng chính"",""10.7626"",""106.6601"",""0.98"""
            $sw.WriteLine($lineOut)
            $logId++
        }
    }
    $sw.Close()
    Write-Host "Done: $fileName"
}
