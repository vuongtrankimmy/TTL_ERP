using TTL.HR.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// standard-api-setup
builder.Services.Configure<TTL.HR.Web.Data.MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Client-side services setup (Using API Repository)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000/") });
builder.Services.AddScoped(typeof(TTL.HR.Shared.Interfaces.IApiRepository<>), typeof(TTL.HR.Shared.Services.Client.ApiRepository<>));
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IEmployeeService, TTL.HR.Shared.Services.Client.EmployeeService>();

builder.Services.AddSingleton(typeof(TTL.HR.Shared.Interfaces.IRepository<>), typeof(TTL.HR.Web.Data.MongoRepository<>));
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers(); // Enable API endpoints

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(TTL.HR.Shared._Imports).Assembly);

app.Run();
