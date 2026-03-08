# .NET Microservice Template

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Core Services
builder.Services.AddScoped<IMyService, MyService>();
// Configure Database Connection
// builder.Services.AddDbContext<AppDbContext>(...);

var app = builder.MapControllers();

app.Run();
```
