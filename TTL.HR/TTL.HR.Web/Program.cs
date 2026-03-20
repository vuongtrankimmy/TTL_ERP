using ApexCharts;
using TTL.HR.Web.Components;
using TTL.HR.Application.Infrastructure.MockData;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024); // Expand to 10MB for AI high-res images
builder.Services.AddLocalization();

// Mock Data Settings
var useMockData = builder.Configuration.GetValue<bool>("MockDataSettings:Enabled");

Console.WriteLine($"===============================================");
Console.WriteLine($"🚀 TTL HR APPLICATION");
Console.WriteLine($"📡 Mock Data Mode: {(useMockData ? "ENABLED ✅" : "DISABLED ❌")}");
Console.WriteLine($"===============================================\n");

// standard-api-setup
// Client-side services setup (Using API Repository from Application project)
if (useMockData)
{
    // MOCK MODE - Khởi tạo và nạp dữ liệu từ disk (Server-side)
    var mockProvider = new MockDataProvider();
    try
    {
        var mockPath = Path.Combine(builder.Environment.WebRootPath, "MockData");
        if (Directory.Exists(mockPath))
        {
            foreach (var file in Directory.GetFiles(mockPath, "*.json"))
            {
                var collectionName = Path.GetFileNameWithoutExtension(file);
                if (collectionName.Equals("metadata", StringComparison.OrdinalIgnoreCase)) continue;
                mockProvider.AddCollection(collectionName, File.ReadAllText(file));
            }
            mockProvider.SetLoaded(true);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ [MOCK] Không thể nạp dữ liệu từ disk: {ex.Message}");
    }

    builder.Services.AddSingleton(mockProvider);
    builder.Services.AddScoped<HttpClient>(sp =>
    {
        var mockDataProvider = sp.GetRequiredService<MockDataProvider>();
        var handler = new MockHttpMessageHandler(mockDataProvider);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://gateway.tantanloc.com/")
        };
        return client;
    });
}
else
{
    // PRODUCTION MODE - Kết nối API thật
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://gateway.tantanloc.com/") });
}
builder.Services.AddScoped(typeof(TTL.HR.Application.Modules.Common.Interfaces.IApiRepository<>), typeof(TTL.HR.Application.Modules.Common.Services.ApiRepository<>));
builder.Services.AddScoped<TTL.HR.Application.Modules.HumanResource.Interfaces.IEmployeeService, TTL.HR.Application.Modules.HumanResource.Services.EmployeeService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.HumanResource.Interfaces.IEmployeeImportService, TTL.HR.Application.Modules.HumanResource.Services.EmployeeImportService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IMasterDataService, TTL.HR.Application.Modules.Common.Services.MasterDataService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Organization.Interfaces.IDepartmentService, TTL.HR.Application.Modules.Organization.Services.DepartmentService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Organization.Interfaces.IPositionService, TTL.HR.Application.Modules.Organization.Services.PositionService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IAuthService, TTL.HR.Application.Modules.Common.Services.AuthService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.HumanResource.Interfaces.IContractService, TTL.HR.Application.Modules.HumanResource.Services.ContractService>();

// Module Services
builder.Services.AddScoped<TTL.HR.Application.Modules.Dashboard.Interfaces.IDashboardService, TTL.HR.Application.Modules.Dashboard.Services.DashboardService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Recruitment.Interfaces.IRecruitmentService, TTL.HR.Application.Modules.Recruitment.Services.RecruitmentService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Attendance.Interfaces.IAttendanceService, TTL.HR.Application.Modules.Attendance.Services.AttendanceService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Leave.Interfaces.ILeaveService, TTL.HR.Application.Modules.Leave.Services.LeaveService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Training.Interfaces.ITrainingService, TTL.HR.Application.Modules.Training.Services.TrainingService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Payroll.Interfaces.IPayrollService, TTL.HR.Application.Modules.Payroll.Services.PayrollService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Payroll.Interfaces.IBenefitService, TTL.HR.Application.Modules.Payroll.Services.BenefitService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IPermissionService, TTL.HR.Application.Modules.Common.Services.PermissionService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Assets.Interfaces.IAssetService, TTL.HR.Application.Modules.Assets.Services.AssetService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.ISettingsService, TTL.HR.Application.Modules.Common.Services.SettingsService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IFileService, TTL.HR.Application.Modules.Common.Services.FileService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IFormatService, TTL.HR.Application.Modules.Common.Services.FormatService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IAuditService, TTL.HR.Application.Modules.Common.Services.AuditService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IPdfService, TTL.HR.Application.Modules.Common.Services.PdfService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.INavigationService, TTL.HR.Application.Modules.Common.Services.NavigationService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IBankService, TTL.HR.Application.Modules.Common.Services.BankService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.INotificationService, TTL.HR.Application.Modules.Common.Services.NotificationService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IOcrService, TTL.HR.Application.Infrastructure.Services.LocalOcrService>();
builder.Services.AddApexCharts();





// Application Layer
builder.Services.AddScoped<TTL.HR.Application.Modules.Recruitment.IRecruitmentApplication, TTL.HR.Application.Modules.Recruitment.RecruitmentApplication>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Attendance.IAttendanceApplication, TTL.HR.Application.Modules.Attendance.AttendanceApplication>();

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

var supportedCultures = new[] { "vi", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(TTL.HR.Shared._Imports).Assembly);

app.Run();
