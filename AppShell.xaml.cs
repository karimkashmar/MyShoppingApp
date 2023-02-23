using MyShoppingApp.View;

namespace MyShoppingApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(ShoppingPage), typeof(ShoppingPage));
        Routing.RegisterRoute(nameof(MyOrdersPage), typeof(MyOrdersPage));
        Routing.RegisterRoute(nameof(OrderDetailsPage), typeof(OrderDetailsPage));
    }
}
