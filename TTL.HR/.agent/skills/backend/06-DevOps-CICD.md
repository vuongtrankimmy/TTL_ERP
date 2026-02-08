# OrgaX - DevOps & CI/CD Standards

## 11.1 GitHub Actions CI (.github/workflows/ci.yml)
```yaml
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
    - run: dotnet restore
    - run: dotnet build --no-restore
    - run: dotnet test --no-build
```

## 11.2 Docker-compose (Local Stack)
- Internal network for inter-service communication.
- Containerized RabbitMQ, Redis, and MongoDB.
- Hot-reload support for development environments.
