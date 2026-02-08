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
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7000") });
            builder.Services.AddScoped<TTL.HR.Shared.Interfaces.IEmployeeService, TTL.HR.Shared.Services.Client.EmployeeService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
