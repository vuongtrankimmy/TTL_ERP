# Event Driven Architecture

**Flow:**
`Producer -> Event Bus (Kafka/RabbitMQ) -> Consumers`

**Use for:**
- Decoupling operations
- High throughput
- Delayed or background jobs

**Pattern Types:**
- **Event Notification**: Light payloads, consumer refetches data.
- **Event-Carried State Transfer**: Heavy payloads, consumer updates local read model.
- **Event Sourcing**: Log of events is the single source of truth.
