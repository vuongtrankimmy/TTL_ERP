# System Design Prompt 1

Design a distributed backend system for a Ride-Hailing Platform.

**Traffic:**
- Active Users: 10 Million
- Throughput: 100k requests/second

**Requirements:**
- High availability
- Low latency for location matching
- Scalable payment processing
- Resilient messaging between rider and driver

**Output expected:**
1. Architecture style & Diagram description
2. Microservices list (e.g., UserService, DriverService, TripService, PaymentService)
3. Databases (MongoDB for trips, PostgreSQL for payments, Redis for matching)
4. Event system setup (Kafka)
5. Scaling strategy
