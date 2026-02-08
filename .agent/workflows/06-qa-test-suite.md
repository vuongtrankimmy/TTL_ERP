---
description: Run Partial or Full QA Test Suite
---

This workflow automates the **Quality Assurance** process and **Engineering Handbook** requirements.

1. **Run Unit Tests (Fast)**
   - Executes all tests with no restore (assumes build is done).
// turbo
2. Run command:
```powershell
dotnet test --no-restore --verbosity normal
```

3. **Generate Coverage Report**
   - Requires `coverlet.collector` package.
// turbo
4. Run command:
```powershell
dotnet test --collect:"XPlat Code Coverage"
```

5. **Verify Integration Test Environment**
   - Checks if Docker containers (DB, Redis) are running.
// turbo
6. Run command:
```powershell
$containers = docker ps --format "{{.Names}}"
if ($containers -match "postgres|mongo" -and $containers -match "redis") {
    Write-Host "✅ Dependent containers are running."
} else {
    Write-Warning "⚠️ Some dependencies might be missing. Check docker-compose."
}
```
