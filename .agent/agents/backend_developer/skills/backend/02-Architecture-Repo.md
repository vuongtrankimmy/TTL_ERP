# OrgaX - Architecture & Repository Standards

## 🏗️ 2.5 Real-time Migration Pattern
- ALL system upgrades MUST use the `UpgradeService` for monitoring.
- Migration progress MUST be persisted in **MongoDB** (`MigrationProgress` collection).
- Real-time updates MUST be pushed to either **Firebase Realtime Database** or via **SignalR** for client-side live tracking.
- Progress updates MUST include: `SystemName`, `Progress` (0-100), `Status`, and `Timestamp`.

## 🏗️ 2.6 Notification Standard
- USE `NotificationService` for all cross-platform push notifications.
- MANDATORY use of **Firebase Admin SDK**.
- MUST support Android, iOS, and Web environments.

## 2.1 Repository Structure
```
/mcp-sample
 ├─ gateway/
 │   └─ Mcp.Gateway.Api
  ├─ services/
 │   ├─ UserService/
 │   ├─ OrderService/
 │   ├─ NotificationService/
 │   ├─ UpgradeService/
 │   └─ PaymentService/
 ├─ shared/
 │   ├─ BuildingBlocks
 │   └─ Contracts
 ├─ deploy/
 │   ├─ docker-compose.yml
 │   └─ helm/
 └─ README.md
```

## 2.2 Clean Architecture Rules
- Service Template: Mandatory Clean Architecture logic.
- CQRS (if domain complexity warrants it).
- MongoDB / SQL per service.
- Integration Event publish (Transactional Outbox pattern mandatory).

## 3.1 Gateway Agent (YARP)
- Auth / JWT validation.
- Route MCP services.
- Rate limiting.
- API versioning.
- **NO** business logic.
- **NO** direct database access.

## 3.2 Frontend Agent Rules
- Frontend MUST only call the Gateway.
- NO direct service-to-service calls from frontend.
- Real-time communication via SignalR.
- Push notifications via Firebase.
