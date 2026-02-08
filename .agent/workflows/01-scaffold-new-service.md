---
description: Scaffold a new MCP Service with Clean Architecture
---

This workflow automates the creation of a new Service following the **OrgaX Repository Standard** (Section 2.1).

1. Define the service name (replace `MyService` with the actual name).
2. Create the Clean Architecture folder structure.
// turbo
3. Run the following commands:
```powershell
$serviceName = "MyService"
# Check if we are at root (Api) or inside OrgaX.ERP
$basePath = "src/services"
if (Test-Path "OrgaX.ERP") { $basePath = "OrgaX.ERP/src/services" }

$fullPath = "$basePath/$serviceName"
Write-Host "Scaffolding service at: $fullPath"

mkdir "$fullPath"
mkdir "$fullPath/$serviceName.Api"
mkdir "$fullPath/$serviceName.Application"
mkdir "$fullPath/$serviceName.Domain"
mkdir "$fullPath/$serviceName.Infrastructure"
```

4. Create the initial `README.md` for the service.
// turbo
5. Run command:
```powershell
Set-Content -Path "src/services/MyService/README.md" -Value "# MyService Module`n`nFollows OrgaX Clean Architecture."
```
