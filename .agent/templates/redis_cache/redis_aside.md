# Redis Cache-Aside Pattern

**Flow:**
1. Check Redis for data by Key.
2. If hit -> Return data directly.
3. If miss -> Query DB.
4. Set Redis cache (with TTL).
5. Return data.

```csharp
var cacheKey = $"User_{id}";
var cachedData = await _cache.GetStringAsync(cacheKey);

if (cachedData != null) return Deserialize(cachedData);

var data = await _db.Users.FindAsync(id);
await _cache.SetStringAsync(cacheKey, Serialize(data), new DistributedCacheEntryOptions {
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
});
```
