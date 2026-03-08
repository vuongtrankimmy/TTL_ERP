# OrgaX - Backend Engineering Standard

## 4.1 Implementation Checklist (1000–5000 RPS)
- [ ] Async all I/O operations.
- [ ] Output Caching (.NET 8).
- [ ] Redis distributed caching.
- [ ] Specialized Read models (CQRS).
- [ ] RabbitMQ cluster with retry policy.
- [ ] Idempotent consumers.
- [ ] MongoDB ReplicaSet / Index audit.
- [ ] OpenTelemetry distributed tracing.

## 8.1 Backend Coding Standard
- .NET 8 LTS only.
- Clean Architecture mandatory.
- Database per service.
- Event-driven side effects.
- Async/await everywhere.
- CancellationToken used in all asynchronous methods.

## 10.1 Dockerfile Standard (.NET 8)
```dockerfile
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
```
