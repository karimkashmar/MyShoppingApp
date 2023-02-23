using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class RegisterPage : ContentPage
{
    // binding viewmodel with page

    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}