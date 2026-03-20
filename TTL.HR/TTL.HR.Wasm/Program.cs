using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TTL.HR.Shared;
using TTL.HR.Application.Infrastructure.MockData;
using ApexCharts;

// Removed redundant/incorrect namespace using

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Note: Root component is App from the Shared library!
builder.RootComponents.Add<TTL.HR.Shared.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Mock Mode is default for WASM on GitHub Pages
var mockProvider = new MockDataProvider();

// Load mock data from metadata.json
var tempClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
try
{
    var metadataString = await tempClient.GetStringAsync("MockData/metadata.json");
    var metadataObject = Newtonsoft.Json.Linq.JObject.Parse(metadataString);
    var statistics = metadataObject["statistics"] as Newtonsoft.Json.Linq.JArray;
    if (statistics != null)
    {
        foreach (var stat in statistics)
        {
            try
            {
                var statObject = stat as Newtonsoft.Json.Linq.JObject;
                string? collection = statObject?["collection"]?.ToString();
                if (string.IsNullOrEmpty(collection)) continue;
                var json = await tempClient.GetStringAsync($"MockData/{collection}.json");
                mockProvider.AddCollection(collection, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ [MOCK] Lỗi khi nạp collection {collection} từ HTTP: {ex.Message}");
            }
        }
        mockProvider.SetLoaded(true);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ [MOCK] Lỗi khi nạp metadata.json từ HTTP: {ex.Message}");
}

builder.Services.AddSingleton(mockProvider);
builder.Services.AddScoped(sp =>
{
    var mockDataProvider = sp.GetRequiredService<MockDataProvider>();
    
    // For WASM, we just return the custom handler
    var handler = new MockHttpMessageHandler(mockDataProvider);
    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    };
    return client;
});

// Registrar services from Shared/Application
builder.Services.AddScoped(typeof(TTL.HR.Application.Modules.Common.Interfaces.IApiRepository<>), typeof(TTL.HR.Application.Modules.Common.Services.ApiRepository<>));
builder.Services.AddScoped<TTL.HR.Application.Modules.HumanResource.Interfaces.IEmployeeService, TTL.HR.Application.Modules.HumanResource.Services.EmployeeService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.HumanResource.Interfaces.IEmployeeImportService, TTL.HR.Application.Modules.HumanResource.Services.EmployeeImportService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IMasterDataService, TTL.HR.Application.Modules.Common.Services.MasterDataService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Organization.Interfaces.IDepartmentService, TTL.HR.Application.Modules.Organization.Services.DepartmentService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Organization.Interfaces.IPositionService, TTL.HR.Application.Modules.Organization.Services.PositionService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IAuthService, TTL.HR.Application.Modules.Common.Services.AuthService>();
builder.Services.AddScoped<TTL.HR.Application.Modules.HumanResource.Interfaces.IContractService, TTL.HR.Application.Modules.HumanResource.Services.ContractService>();
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
builder.Services.AddApexCharts();

builder.Services.AddScoped<TTL.HR.Application.Modules.Recruitment.IRecruitmentApplication, TTL.HR.Application.Modules.Recruitment.RecruitmentApplication>();
builder.Services.AddScoped<TTL.HR.Application.Modules.Attendance.IAttendanceApplication, TTL.HR.Application.Modules.Attendance.AttendanceApplication>();

// Add localization
builder.Services.AddLocalization();

await builder.Build().RunAsync();
