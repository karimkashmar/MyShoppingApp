using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class ShoppingPage : ContentPage
{
    private ShoppingViewModel _viewModel;

    public ShoppingPage(ShoppingViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
}