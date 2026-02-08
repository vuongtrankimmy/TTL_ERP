---
description: Add Standard Docker Support to a Project
---

This workflow adds a standard `Dockerfile` complying with **OrgaX Backend Standards** (Section 10.1).

1. Ensure you are in the project root where the `Dockerfile` should be created.
2. Create the `Dockerfile` with multi-stage build optimization.

// turbo
3. Run command:
```powershell
$content = @"
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "App.dll"]
"@
Set-Content -Path "Dockerfile" -Value $content
```
