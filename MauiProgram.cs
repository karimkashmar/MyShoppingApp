using MyShoppingApp.View;
using MyShoppingApp.ViewModel;
using Microsoft.Extensions.Logging;
using MyShoppingApp.Services;
using CommunityToolkit.Maui;

namespace MyShoppingApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<BaseViewModel>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<RegisterViewModel>();
        builder.Services.AddSingleton<DashboardViewModel>();
        builder.Services.AddSingleton<ShoppingViewModel>();
        builder.Services.AddSingleton<MyOrdersViewModel>();
        builder.Services.AddSingleton<OrderDetailsViewModel>();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<ShoppingPage>();
        builder.Services.AddSingleton<MyOrdersPage>();
        builder.Services.AddSingleton<OrderDetailsPage>();

        builder.Services.AddSingleton<DatabaseService>();




#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
