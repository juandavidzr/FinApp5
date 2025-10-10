using CommunityToolkit.Maui;
using Controls.UserDialogs.Maui;
using FinApp5.ViewModels;
using FinAppMaui.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using System.Net;
using FinApp5.Services;
using FinApp5.Views;
using System.Globalization;

namespace FinApp5
{

    public static class MauiProgram
    {

        public static IServiceProvider Services { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
               
            

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()

                .UseUserDialogs(() =>
                {
                    //setup your default styles for dialogs
                    AlertConfig.DefaultBackgroundColor = Colors.Purple;
                #if ANDROID
                    AlertConfig.DefaultMessageFontFamily = "OpenSans-Regular.ttf";
                #else
                    AlertConfig.DefaultMessageFontFamily = "OpenSans-Regular";
                #endif

                    ToastConfig.DefaultCornerRadius = 15;
                })

                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddHttpClient("FinAppApi", client =>
            {
                
                client.BaseAddress = new Uri("http://138.128.171.162:8001/"); // URL API de pruebas
               
            });

            builder.Services.AddScoped<AuthService>();
            //builder.Services.AddScoped<ClienteService>();
            builder.Services.AddTransient<ClienteService>();
            builder.Services.AddTransient<Clientes>();

            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddTransient<VMingresar>();

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<ILauncher>(Launcher.Default);
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<VMTransacciones>();
            builder.Services.AddTransient<MenuPpal>();

            #if DEBUG
                builder.Logging.AddDebug();
#endif

            //System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            var app = builder.Build();

            Services = app.Services;

            return app;

            
        }
    }
}
