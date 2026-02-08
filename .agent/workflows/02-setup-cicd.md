---
description: Setup Standard GitHub Actions CI Pipeline
---

This workflow automates the creation of the CI pipeline from **OrgaX DevOps Standards** (Section 11.1).

1. Create the `.github/workflows` directory.
// turbo
2. Run command:
```powershell
mkdir -p .github/workflows
```

3. Create the `ci.yml` file with standard configuration.
// turbo
4. Run command:
```powershell
$content = @"
name: MCP CI
on:
  push:
    branches: [ main ]
  pull_request:

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    - uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    - run: dotnet restore OrgaX.ERP/OrgaX.ERP.slnx  # Target the specific solution
    - run: dotnet build OrgaX.ERP/OrgaX.ERP.slnx --no-restore
    - run: dotnet test OrgaX.ERP/OrgaX.ERP.slnx --no-build
"@
Set-Content -Path ".github/workflows/ci.yml" -Value $content
```
