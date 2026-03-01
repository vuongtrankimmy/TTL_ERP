
$mapping = @{}
$files = @("employees.json", "batch1.json", "batch2.json")
foreach ($f in $files) {
    if (Test-Path $f) {
        $json = Get-Content $f -Encoding UTF8 -Raw | ConvertFrom-Json
        foreach ($item in $json) {
            $id = ""
            $code = ""
            
            if ($item.PSObject.Properties["_id"] -and $item._id.PSObject.Properties['$oid']) {
                $id = $item._id.'$oid'
            }
            elseif ($item.PSObject.Properties["EmployeeId"]) {
                $id = $item.EmployeeId
            }
            
            if ($item.PSObject.Properties["Code"]) {
                $code = $item.Code
            }
            elseif ($item.PSObject.Properties["EmployeeCode"]) {
                $code = $item.EmployeeCode
            }
            
            if ($id -and $code) {
                $mapping[$id] = $code
            }
        }
    }
}

$connStr = "mongodb://localhost:27030"
$dbName = "HR"

# Create a temporary JS file for mongosh
$jsPath = "update_employees.js"
$jsContent = "const mapping = " + ($mapping | ConvertTo-Json -Compress) + ";`n"
$jsContent += @"
db.employees.find().forEach(emp => {
    let idStr = emp._id.toString();
    let code = mapping[idStr];
    let update = {};
    
    if (code) {
        if (emp.Code !== code) {
            update.Code = code;
        }
    } else {
        // Generate code if missing or bad (placeholder format)
        if (!emp.Code || emp.Code.startsWith('NV202')) {
            // Find a sequence number or use something else.
            // Since I am doing this in a loop, I can use a global counter or a random one.
            // For simplicity and safety, let's use a random 4-digit number for the 'random' one
            // or just convert it to EMP-9999 for now if it's just one.
            // Actually, let's use EMP- + a hash of the ID or similar.
            let randDigits = Math.floor(1000 + Math.random() * 9000);
            update.Code = 'EMP-' + randDigits;
        }
    }
    
    // Always update TimekeepingCode if empty
    if (!emp.TimekeepingCode || emp.TimekeepingCode === '') {
        // If code is GD-001, TimekeepingCode could be 001 or GD001
        // Let's use the code stripped of non-alphanumeric if required, 
        // but often it is just the same as Code or the numeric part.
        // I will use Code as it is likely unique.
        let finalCode = update.Code || emp.Code;
        if (finalCode) {
            update.TimekeepingCode = finalCode;
        }
    }
    
    if (Object.keys(update).length > 0) {
        db.employees.updateOne({ _id: emp._id }, { `$set: update });
    }
});
"@

Set-Content -Path $jsPath -Value $jsContent -Encoding UTF8

mongosh "$connStr/$dbName" $jsPath

Remove-Item $jsPath
