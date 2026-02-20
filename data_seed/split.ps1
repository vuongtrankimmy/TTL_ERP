[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$json = Get-Content 'd:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\payrolls_100.json' -Raw
$data = $json | ConvertFrom-Json
$batch1 = $data[0..49] | ConvertTo-Json -Depth 10
$batch1 | Out-File -FilePath 'd:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\batch1.json' -Encoding utf8
$batch2 = $data[50..99] | ConvertTo-Json -Depth 10
$batch2 | Out-File -FilePath 'd:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\batch2.json' -Encoding utf8
