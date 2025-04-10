using ClickMe.Models;
using ClickMe.ViewModels;
using ClickMe.Views;
using Microsoft.Extensions.Logging;

namespace ClickMe
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<SpielPage>();
            builder.Services.AddTransient<Bestenliste>();
            builder.Services.AddTransient<BestenlisteViewModel>();
            builder.Services.AddTransient<Optionen>();
            builder.Services.AddTransient<OptionenViewModel>();
            builder.Services.AddTransient<Spieler>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
