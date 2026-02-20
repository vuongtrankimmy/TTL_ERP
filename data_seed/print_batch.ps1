[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$json = Get-Content 'd:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\payrolls_100.json' -Raw
$data = $json | ConvertFrom-Json
$data[0..19] | ConvertTo-Json -Depth 10
