---
name: ttl-project-scaffolding
description: Standardized instructions for creating a new service/module following the TTL Clean Architecture pattern.
---

# TTL Project Scaffolding

Follow these steps to scaffold a new microservice or module in the TTL ecosystem.

## 1. Directory Structure
Create a new project folder under `/services` with the following sub-projects:

- **{{ProjectName}}.Domain**: Core entities, value objects, and repository interfaces.
- **{{ProjectName}}.Application**: MediatR Commands/Queries, DTOs, Mapping profiles, and Business logic.
- **{{ProjectName}}.Infrastructure**: Database context (EF/Mongo), Repository implementations, External service clients.
- **{{ProjectName}}.API**: ASP.NET Core Web API, Controllers, Middleware, and `appsettings.json`.

## 2. Dependency Rules
- **Domain**: NO dependencies.
- **Application**: Depends on **Domain**.
- **Infrastructure**: Depends on **Application** and **Domain**.
- **API**: Depends on **Infrastructure** and **Application**.

## 3. Mandatory Setup
For every new API project:
- [ ] Install **MediatR**, **AutoMapper**, **FluentValidation**.
- [ ] Configure `Program.cs` for standard TTL services (Auth, Logging, Swagger).
- [ ] Create `BuildingBlocks` reference if shared logic is needed.
- [ ] Setup `Dockerile` and `docker-compose` entry.

## 4. Database Initialization
- Use **MongoDB** for flexible audit logs or **PostgreSQL** for relational data.
- Ensure `_translate` collections are planned if labels are translatable.
- Implement the `BaseEntity` with `Id`, `CreatedAt`, `CreatedBy`, `IsDeleted`.
