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
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IMasterDataService, TTL.HR.Shared.Services.Client.MasterDataService>();
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IDepartmentService, TTL.HR.Shared.Services.Client.DepartmentService>();
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IPositionService, TTL.HR.Shared.Services.Client.PositionService>();
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IDashboardService, TTL.HR.Shared.Services.Client.DashboardService>();
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IRecruitmentService, TTL.HR.Shared.Services.Client.RecruitmentService>();
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IAttendanceService, TTL.HR.Shared.Services.Client.AttendanceService>();
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.ILeaveService, TTL.HR.Shared.Services.Client.LeaveService>();
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.ITrainingService, TTL.HR.Shared.Services.Client.TrainingService>();
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IPayrollService, TTL.HR.Shared.Services.Client.PayrollService>();
builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IAssetService, TTL.HR.Shared.Services.Client.AssetService>();

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
