# Microservices Pattern

**Use case:**
Systems requiring high scale, independent deployment, and strong physical isolation of domains.

**Benefits:**
- Independent deployment cycles
- Scalable at the component level
- Team autonomy and polyglot persistence

**Components:**
- **API Gateway**: Entry point, routing, auth mapping (e.g., Ocelot / YARP).
- **Service Discovery**: Identifying nodes (optional in k8s).
- **Event Bus / Message Broker**: For inter-service async communication (Kafka, RabbitMQ).
- **Database per Service**: Ensuring loose coupling.

**When NOT to use:**
- Small traffic, tight startup budget, tightly coupled entities requiring distributed transactions.
