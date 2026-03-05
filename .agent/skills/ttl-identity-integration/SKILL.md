---
name: ttl-identity-integration
description: Implementing Authentication and Authorization across TTL services.
---

# TTL Identity Integration

Standardized way to handle Security and User Context.

## 1. Backend Authorization
- Apply `[Authorize]` attribute at the Controller or Action level.
- Use `Authorize(Roles = "Admin,Manager")` for granular access.
- Use `ICurrentUserService` to fetch:
    - `UserId` (Guid/ObjectId).
    - `LanguageCode`.
    - `Roles`.

## 2. Permissions System
- Permissions are string-based keys (e.g., `Employees.View`, `Contracts.Edit`).
- Use the `HasPermission` requirement or custom Policy-based authorization.

## 3. Frontend Auth State
- Wrap protected components in `<AuthorizeView>`.
- Use `IdentityService` in Blazor to check `IsAuthenticated`.
- Ensure JWT is passed in the `Authorization: Bearer [Token]` header for all API calls.

## 4. Audit Trail
- Every Command must record the `AuditContext`.
- Changes must be captured in the `Audits` collection/table.
