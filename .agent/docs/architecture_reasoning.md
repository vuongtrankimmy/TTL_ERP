# Architecture Reasoning Engine

AI compares different architectures based on the requirements:

## Criteria
- **Traffic**: Expected RPS (Requests Per Second)
- **Latency**: Needed response time (e.g. < 50ms)
- **Cost**: Budget implications
- **Team Size**: Number of domain bounds

## Options:
1. **Monolith**: Best for simple domains, unified DB.
2. **Microservices (Sync)**: Best for complex teams, distinct scaling. Over HTTP/gRPC.
3. **Event-driven (Async)**: High performance, loosely coupled, eventual consistency. Kafka/RabbitMQ.
4. **CQRS**: Used when read-heavy vs write-heavy disparity is high.

## Steps for Decision:
- Identify if the system has clear bounded contexts.
- Check scalability pressure.
- Propose an architecture style, listing **Pros**, **Cons**, and **Why it fits**.
