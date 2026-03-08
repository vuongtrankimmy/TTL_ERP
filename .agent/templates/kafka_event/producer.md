# Kafka Producer Strategy

```csharp
// Example using Confluent.Kafka
var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

using (var producer = new ProducerBuilder<Null, string>(config).Build())
{
    var result = await producer.ProduceAsync("topic_name", new Message<Null, string> { Value = "a payload" });
}
```

## Pattern
1. Generate unique Request Identifier.
2. Publish with Idempotency Key.
3. Handle failure to publish (Outbox Pattern).
