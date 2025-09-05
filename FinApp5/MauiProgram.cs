using CommunityToolkit.Maui;
using Controls.UserDialogs.Maui;
using FinApp5.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using System.Net;

namespace FinApp5
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
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


            builder.Services.AddSingleton<ILauncher>(Launcher.Default);
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<VMTransacciones>();



        #if DEBUG
            builder.Logging.AddDebug();
        #endif

            return builder.Build();
        }
    }
}
