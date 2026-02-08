# OrgaX - Advanced Enterprise Standards

## 13.1 Database Replication (Replica)
- **Policy**: Separate Primary (Write) and Secondary (Read) nodes.
- **MongoDB**: Use `replicaSet=rs0&readPreference=secondaryPreferred`.

## 13.2 Distributed Transactions (Outbox Pattern)
- **Standard**: NO direct event publishing. Must use Outbox.
- **Flow**: Save Business Data + Outbox Message in a single transaction.
- **Library**: MassTransit Entity Framework Outbox.

## 13.3 Message Queue (MassTransit + RabbitMQ)
- **Retry**: Exponential Retry policy (3-5 attempts).
- **DLQ**: Dead Letter Queue for error handling.
- **Reliability**: `auto-ack = false` (explicit acknowledgement).

## 13.4 Distributed Caching (Redis)
- **Standard**: `IDistributedCache` with Redis provider.
- **Policy**: Always set TTL. Key Naming: `{Service}:{Feature}:{Key}`.

## 13.5 Multi-Language (I18n)
- **Default**: `vi-VN`. Supported: `vi-VN`, `en-US`.
- **Rule**: Use `IStringLocalizer`. Resource files located at `Api/Resources`.

## 13.6 API Versioning
- **Strategy**: URL-based (`/api/v1/resource`).
- **Default**: `1.0`.

## 13.7 High-Performance Runtime
- **GC**: ServerGC=true, ConcurrentGC=true, RetainVM=true.
- **ThreadPool**: `MinThreads = [CPU] * 2` or fixed pattern (e.g., 100).

## 13.8 Environment Configuration
- **Storage**: `.env` file at project root.
- **Precedence**: `.env` overrides `appsettings.json`.
- **Secret Management**: NO secrets allowed in `appsettings.json`.
