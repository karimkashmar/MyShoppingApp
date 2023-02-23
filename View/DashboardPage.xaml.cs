using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class DashboardPage : ContentPage
{
    private DashboardViewModel _viewModel;
    // binding viewmodel with page
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
}