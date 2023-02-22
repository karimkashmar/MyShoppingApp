using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class DashboardPage : ContentPage
{
    private DashboardViewModel _viewModel;
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
}