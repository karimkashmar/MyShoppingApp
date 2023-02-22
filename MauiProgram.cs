using MyShoppingApp.View;
using MyShoppingApp.ViewModel;
using Microsoft.Extensions.Logging;
using MyShoppingApp.Services;

namespace MyShoppingApp;

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

        builder.Services.AddSingleton<BaseViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<RegisterViewModel>();
        builder.Services.AddSingleton<DatabaseService>();





#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
