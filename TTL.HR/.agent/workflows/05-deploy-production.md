---
description: Production Deployment Pipeline (Docker & Helm)
---

This workflow automates the **Production Packaging & Deployment** standards (Section 13).

1. **Authenticate to Container Registry**
   - *Agent will prompt for registry credentials if not found.*

2. **Build and Tag Docker Image**
   - Uses the standard Dockerfile.
   - Tags with both `latest` and git commit hash.
// turbo
3. Run command:
```powershell
$commitHash = git rev-parse --short HEAD
$buildContext = "."
if (Test-Path "OrgaX.ERP/Dockerfile") { $buildContext = "OrgaX.ERP" }

Write-Host "Building Docker image from context: $buildContext"
docker build -t orgax-service:$commitHash -t orgax-service:latest $buildContext
```

4. **Verify Image Vulnerabilities**
   - Uses `docker scan` (if available) or standard reporting.

5. **Deploy via Helm (Kubernetes)**
   - Updates the Helm values with the new image tag.
   - Performs a rolling update.
// turbo
6. Run command:
```powershell
# Example: helm upgrade --install my-service ./deploy/helm --set image.tag=$commitHash
Write-Host "Ready to deploy image: orgax-service:$commitHash"
```
