using Microsoft.Extensions.Logging;

namespace TTL.HR
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

// standard-client-setup
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://gateway.tantanloc.com/") });
            builder.Services.AddScoped<TTL.HR.Application.Modules.HumanResource.Interfaces.IEmployeeService, TTL.HR.Application.Modules.HumanResource.Services.EmployeeService>();
            builder.Services.AddScoped<TTL.HR.Application.Modules.Organization.Interfaces.IDepartmentService, TTL.HR.Application.Modules.Organization.Services.DepartmentService>();
            builder.Services.AddScoped<TTL.HR.Application.Modules.Organization.Interfaces.IPositionService, TTL.HR.Application.Modules.Organization.Services.PositionService>();
            builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.IMasterDataService, TTL.HR.Application.Modules.Common.Services.MasterDataService>();
            builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.INotificationService, TTL.HR.Application.Modules.Common.Services.NotificationService>();
            builder.Services.AddScoped<TTL.HR.Application.Modules.Common.Interfaces.ISettingsService, TTL.HR.Application.Modules.Common.Services.SettingsService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
