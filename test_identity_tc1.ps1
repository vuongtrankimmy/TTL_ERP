$token = Get-Content -Path "token.txt" -Raw
$baseUrl = "http://localhost:5043/api/v1"

$employeeData = @{
    FullName        = "Nguyễn Văn IdentityTest"
    Email           = "test.identity@tantanloc.com"
    CompanyEmail    = "test.identity@tantanloc.com"
    Phone           = "0999888777"
    JoinDate        = "2026-03-01T00:00:00Z"
    IsCreateAccount = $true
    IsAccountActive = $true
    Role            = "USER"
    PersonalDetails = @{
        IdCardNumber  = "099988877799"
        Gender        = "Nam"
        Nationality   = "Việt Nam"
        Ethnicity     = "Kinh"
        Religion      = "Không"
        MaritalStatus = "Độc thân"
    }
}

$jsonBody = $employeeData | ConvertTo-Json -Depth 5

Write-Output ">>> Request JSON Body:"
Write-Output $jsonBody

Write-Output "`n>>> Creating Employee with IsCreateAccount = true"
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/employees" -Method Post -Body $jsonBody -ContentType "application/json" -Headers @{ Authorization = "Bearer $($token.Trim())" }
    Write-Output "API Response:"
    Write-Output $response
}
catch {
    Write-Output "API ERROR:"
    Write-Output $_.Exception.Message
    if ($_.Exception.Response) {
        Write-Output $_.Exception.Response.StatusCode
        $stream = $_.Exception.Response.GetResponseStream()
        if ($stream) {
            $reader = New-Object System.IO.StreamReader($stream)
            $errorJson = $reader.ReadToEnd()
            Write-Output "Detailed Error JSON:"
            Write-Output $errorJson
        }
    }
}

Write-Output "`n>>> Checking TTL_Identity_DB.Users for new account"
mongosh "mongodb://localhost:27030/TTL_Identity_DB" --eval "db.Users.find({Email: 'test.identity@tantanloc.com'}, {PasswordHash: 0}).toArray()"
