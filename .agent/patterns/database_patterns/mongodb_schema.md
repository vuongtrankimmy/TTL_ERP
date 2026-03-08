# MongoDB Big-Data Schema Pattern

**Use case:**
Flexible schema, large document data, catalog systems, unstructured logs, or high-write scenarios.

**Pros:**
- Horizontally scalable (Sharding)
- No complex joins (Data is denormalized)
- Document model fits JSON perfectly

**Rules:**
1. **Denormalization:** Prefer embedding when the child data size is finite and queried together.
2. **References (ObjectIds):** Use when the data grows unbounded and needs to be accessed independently.
3. **Compound Indexes:** Crucial for multi-field queries.
4. **Time Series Data:** For logs or metrics, use MongoDB's dedicated Time Series collections.
