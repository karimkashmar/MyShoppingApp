using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}