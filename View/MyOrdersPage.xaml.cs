using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class MyOrdersPage : ContentPage
{
    private MyOrdersViewModel _viewModel;

    public MyOrdersPage(MyOrdersViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
}