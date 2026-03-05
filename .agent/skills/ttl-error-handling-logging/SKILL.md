---
name: ttl-error-handling-logging
description: Standardized error handling using Global Middleware and structured Logging.
---

# TTL Error Handling & Logging

Follow these standards to ensure consistent error reporting and observability.

## 1. Global Exception Handler
- All APIs MUST use the `GlobalExceptionHandler` middleware registered in `BuildingBlocks`.
- This ensures all errors are wrapped in `ApiResponse<T>`.

## 2. Using ApiException
Throw `ApiException` for business rule violations that should be returned to the client.

```csharp
throw new ApiException("Invalid operation.", HttpStatusCode.BadRequest);
```

## 3. Validation Logic
- DO NOT manually check `ModelState.IsValid`.
- Use **FluentValidation** in the `Application` layer.
- The `ValidationBehavior` will automatically catch validation errors and return a `400 Bad Request` with structured `Errors`.

## 4. Logging Standards
- Inject `ILogger<T>` into your services/handlers.
- Use **Structured Logging**:
    ```csharp
    _logger.LogInformation("Processing order {OrderId} for customer {CustomerId}", orderId, customerId);
    ```
- Critical errors are automatically logged by the `GlobalExceptionHandler`.
