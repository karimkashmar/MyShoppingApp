using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class LoginPage : ContentPage
{
    private LoginViewModel _viewModel;
    // binding viewmodel with page

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
}