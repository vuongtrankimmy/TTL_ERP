---
name: ttl-background-tasks
description: Using the internal BackgroundTaskQueue for asynchronous processing.
---

# TTL Background Tasks

For non-critical, long-running tasks that shouldn't block the API response.

## 1. Registration
Register the queue in `Program.cs`:
```csharp
builder.Services.AddBackgroundQueue(capacity: 100);
```

## 2. Queueing a Task
Inject `IBackgroundTaskQueue` and queue a work item.

```csharp
_backgroundTaskQueue.QueueBackgroundWorkItem(async token => 
{
    // Perform long running work
    await _emailService.SendEmailAsync(details);
});
```

## 3. Scoped Services
Inside a background task, you MUST create a new scope to resolve scoped services (like Repositories).

```csharp
_backgroundTaskQueue.QueueBackgroundWorkItem(async token => 
{
    using var scope = _serviceScopeFactory.CreateScope();
    var repository = scope.ServiceProvider.GetRequiredService<IMyRepository>();
    // ...
});
```
