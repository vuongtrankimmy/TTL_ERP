# PostgreSQL Schema Pattern

**Use case:**
Transactions, financial data, relational data, reporting, strong ACID requirements.

**Pros:**
- Powerful SQL and JOINS
- ACID compliance Out-of-the-Box
- JSONB support

**Rules:**
1. **Normalization:** Generally aim for 3NF unless read performance dictates caching or slight denormalization.
2. **Foreign Keys:** Enforce referential integrity at the database level.
3. **Indexes:** Use B-Tree for standard fields; GiST or GIN for arrays and JSONB.
4. **Connection Pooling:** Always use PgBouncer or similar to manage the database connection limit under high load.
