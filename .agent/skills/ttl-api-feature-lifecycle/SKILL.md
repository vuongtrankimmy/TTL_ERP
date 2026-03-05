---
name: ttl-api-feature-lifecycle
description: End-to-end implementation pattern for API features including CRUD, Validation, and Transactional Outbox.
---

# TTL API Feature Lifecycle

This skill covers the full lifecycle of a feature from Query to Command.

## 1. Read Operations (LIST/GET)
- Follow the `list-feature-implementation` pattern.
- Always use `PagedResult<T>` for lists.
- Implement Search and Filter in the MediatR Handler.

## 2. Write Operations (CREATE/UPDATE)
- **Commands**: Implement `IRequest<Result<{{EntityName}}Dto>>`.
- **Validation**: Use **FluentValidation** for property checks.
- **Transactional Outbox**: 
    - When modifying state, publish an **IntegrationEvent** (e.g., `{{EntityName}}CreatedEvent`).
    - Store the event in the `OutboxMessages` collection within the same transaction.
- **Identity**: Inject `ICurrentUserService` to populate `CreatedBy` / `ModifiedBy`.

## 3. Delete Operations
- **Soft Delete**: Default to setting `IsDeleted = true`.
- Use the **Transactional Outbox** to notify related services (e.g., Audit or Cache clear).

## 4. Mapping
- Use **AutoMapper** with specific Profiles in the `Application` layer.
- Avoid exposing Domain Entities directly to the API.
