$json = Get-Content 'd:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\batch1.json' -Raw | ConvertFrom-Json
$codes = $json | Select-Object -ExpandProperty EmployeeCode -Unique
$names = $json | Group-Object EmployeeCode | ForEach-Object { @{ Code = $_.Name; Name = $_.Group[0].EmployeeName } }
$deptMap = $json | Group-Object EmployeeCode | ForEach-Object { @{ Code = $_.Name; Dept = "Phòng Ban Mẫu" } }

$header = "log_id,employee_code,employee_name,department,method,action,device_id,device_name,check_time,location,gps_lat,gps_lng,confidence"
$out = @($header)
$date = "2026-02-21"

$counter = 1
foreach ($c in $codes) {
    $empName = ""
    foreach ($n in $names) { if ($n.Code -eq $c) { $empName = $n.Name; break } }
    $dept = "Office"
    
    # Case Logic based on index
    if ($counter % 10 -eq 1) {
        # Case 1: Normal with Multiple scans (Collage)
        $out += "L$counter,$c,$empName,$dept,Fingerprint,CheckIn,D01,Main Gate,$date 07:50:12,HCM,10.7626,106.6602,0.99"
        $out += "L$($counter+100),$c,$empName,$dept,Fingerprint,BreakOut,D01,Main Gate,$date 12:01:05,HCM,10.7626,106.6602,0.98"
        $out += "L$($counter+200),$c,$empName,$dept,Fingerprint,BreakIn,D01,Main Gate,$date 13:02:44,HCM,10.7626,106.6602,0.99"
        $out += "L$($counter+300),$c,$empName,$dept,Fingerprint,CheckOut,D01,Main Gate,$date 17:05:33,HCM,10.7626,106.6602,0.95"
    }
    elseif ($counter % 10 -eq 2) {
        # Case 2: Late + Multiple scans
        $out += "L$counter,$c,$empName,$dept,FaceID,CheckIn,D02,Lobby,$date 08:15:00,HCM,10.7626,106.6602,0.92"
        $out += "L$($counter+100),$c,$empName,$dept,FaceID,CheckIn,D02,Lobby,$date 08:25:00,HCM,10.7626,106.6602,0.94"
        $out += "L$($counter+200),$c,$empName,$dept,FaceID,CheckOut,D02,Lobby,$date 17:15:00,HCM,10.7626,106.6602,0.91"
    }
    elseif ($counter % 10 -eq 3) {
        # Case 3: Early Leave
        $out += "L$counter,$c,$empName,$dept,App,CheckIn,D03,Mobile,$date 07:55:00,HCM,10.7626,106.6602,0.99"
        $out += "L$($counter+100),$c,$empName,$dept,App,CheckOut,D03,Mobile,$date 16:30:00,HCM,10.7626,106.6602,0.99"
    }
    elseif ($counter % 10 -eq 4) {
        # Case 4: Missing Check-Out (Only Check-In)
        $out += "L$counter,$c,$empName,$dept,Fingerprint,CheckIn,D01,Main Gate,$date 07:58:00,HCM,10.7626,106.6602,0.99"
    }
    elseif ($counter % 10 -eq 5) {
        # Case 5: Missing Check-In (Only Check-Out)
        $out += "L$counter,$c,$empName,$dept,Fingerprint,CheckOut,D01,Main Gate,$date 17:10:00,HCM,10.7626,106.6602,0.99"
    }
    elseif ($counter % 10 -eq 6) {
        # Case 6: Overtime (OT)
        $out += "L$counter,$c,$empName,$dept,Card,CheckIn,D04,Side Gate,$date 08:00:00,HCM,10.7626,106.6602,0.99"
        $out += "L$($counter+100),$c,$empName,$dept,Card,CheckOut,D04,Side Gate,$date 21:00:00,HCM,10.7626,106.6602,0.99"
    }
    else {
        # Standard
        $out += "L$counter,$c,$empName,$dept,Fingerprint,CheckIn,D01,Main Gate,$date 08:00:00,HCM,10.7626,106.6602,0.99"
        $out += "L$($counter+100),$c,$empName,$dept,Fingerprint,CheckOut,D01,Main Gate,$date 17:00:00,HCM,10.7626,106.6602,0.99"
    }
    $counter++
}

# Add some completely random/unknown logs
$out += "L9999,UNKNOWN,Chưa cập nhật,External,QR,CheckIn,D99,Guest,$date 09:00:00,HCM,10.7,106.6,0.5"

$out | Out-File -FilePath 'd:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\test_attendance_data.csv' -Encoding utf8
