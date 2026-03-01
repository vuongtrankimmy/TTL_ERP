$json = Get-Content 'd:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\batch1.json' -Raw | ConvertFrom-Json
$codes = $json.EmployeeCode | Select-Object -Unique
$out = @("EmployeeCode,Timestamp,Method,DeviceName")
foreach ($c in $codes) {
    if ($null -ne $c -and $c -ne "") {
        $out += "$c,2026-02-21 07:55:00,Fingerprint,Main Gate"
        $out += "$c,2026-02-21 17:05:00,Fingerprint,Main Gate"
    }
}
$out | Out-File -FilePath 'd:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\test_attendance_data.csv' -Encoding utf8
