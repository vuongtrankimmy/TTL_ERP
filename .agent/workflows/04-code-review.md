---
description: Automated Code Review and Policy Check
---

This workflow automates the **Review Bot Rules** defined in **OrgaX AI Policy** (Section 12.2).
Run this before submitting a PR.

1. **Check for Prohibited Synchronous Calls**
   - Scans for `.Result` and `.Wait()` which cause thread starvation.
// turbo
2. Run command:
```powershell
Write-Host "Scanning for synchronous blocking calls..."
$files = Get-ChildItem -Recurse -Filter "*.cs"
$errors = $files | Select-String -Pattern "\.Result", "\.Wait\(\)"
if ($errors) { 
    Write-Error "Found prohibition synchronous calls: $errors" 
} else { 
    Write-Host "✅ Async checks passed." 
}
```

3. **Check for Hardcoded Secrets**
   - Scans `appsettings.json` for risk patterns.
// turbo
4. Run command:
```powershell
Write-Host "Scanning appsettings.json for secrets..."
$settings = Get-Content "src/**/appsettings.json" -Raw
if ($settings -match "Password=|Secret=|Token=") {
    Write-Warning "⚠️ Potential secrets found in appsettings.json. Use .env instead."
} else {
    Write-Host "✅ Configuration checks passed."
}
```

5. **Verify Clean Architecture Dependencies**
   - Ensures Domain does not reference Infrastructure.
   - *Agent Note: This requires structural analysis of .csproj files.*
