using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class LoginPage : ContentPage
{
    private LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
}