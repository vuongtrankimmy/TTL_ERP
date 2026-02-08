---
description: Active Development Validation Loop (.NET Backend)
---

This workflow automates the **Validate during Coding** process.
Run this frequently while developing to ensure code quality and standard compliance.

1. **Fast Build Verification**
   - Ensures the solution compiles without errors.
// turbo
2. Run command:
```powershell
Write-Host "🚀 Starting Build Verification..."
dotnet build --nologo --verbosity quiet
if ($LASTEXITCODE -eq 0) { Write-Host "✅ Build Succeeded." } else { Write-Error "❌ Build Failed." }
```

3. **Code Style & Formatting Check**
   - Verifies code follows `.editorconfig` rules.
// turbo
4. Run command:
```powershell
Write-Host "🎨 Checking Code Style..."
dotnet format --verify-no-changes --verbosity diagnostic
if ($LASTEXITCODE -eq 0) { Write-Host "✅ Code Style is compliant." } else { Write-Warning "⚠️ formatting issues found. Run 'dotnet format' to fix." }
```

5. **Sanitization Check (Debug Artifacts)**
   - Scans for leftover `Console.WriteLine` or `TODO` comments.
// turbo
6. Run command:
```powershell
Write-Host "🧹 Scanning for Debug Artifacts..."
$files = Get-ChildItem -Recurse -Filter "*.cs" -Exclude "*Test*"
$issues = $files | Select-String -Pattern "Console\.WriteLine", "TODO:"
if ($issues) {
    Write-Warning "⚠️ Found potential debug artifacts:"
    $issues | ForEach-Object { Write-Host "  $($_.Filename):$($_.LineNumber) - $($_.Line.Trim())" }
} else {
    Write-Host "✅ No debug artifacts found."
}
```

7. **NuGet Vulnerability Audit**
   - Checks for known security issues in dependencies.
// turbo
8. Run command:
```powershell
Write-Host "🛡️ Auditing NuGet Packages..."
dotnet list package --vulnerable
```
